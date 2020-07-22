using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	class ArrayInitialisationPattern : BaseInitialisationPattern
	{
		private const uint MaxDefaultValues = 10;

		public ArrayInitialisationPattern(CodePatternsContext patternsContext, TypeSystem ts)
			: base(patternsContext, ts)
		{
		}

		// int[] arr = new int[] { 1, 2 };
		// 
		// ==
		// 
		// int[] arr = new int[2];
		// arr[0] = 1;
		// arr[1] = 2;

		protected override bool TryMatchInternal(StatementCollection statements, int startIndex, out Statement result, out int replacedStatementsCount)
		{
			result = null;
			replacedStatementsCount = 0;

			ArrayCreationExpression arrayCreation;
			Expression assignee;
			if (!TryGetArrayCreation(statements, startIndex, out arrayCreation, out assignee))
			{
				return false;
			}

			if (arrayCreation.Initializer != null && arrayCreation.Initializer.Expressions.Count != 0)
			{
				return false;
			}

			uint arraySize = GetCreatedArraySize(arrayCreation);
			Dictionary<uint, Expression> inlinedExpressions = new Dictionary<uint, Expression>();
			
			long lastElementIndex = -1;
			for (int i = startIndex + 1; i < statements.Count; i++)
			{
				Expression expression;
				if (!TryGetNextExpression(statements[i], out expression))
				{
					break;
				}

				BinaryExpression assignment = (statements[i] as ExpressionStatement).Expression as BinaryExpression;

				if (!IsArrayElementAssignment(assignment, assignee))
				{
					break;
				}

				uint currentElementIndex = GetAssignmentIndex(assignment.Left);
				if (currentElementIndex >= arraySize)
				{
					// This happens in System.String System.Runtime.InteropServices.CustomMarshalers.Resource::GetString(System.String) method
					// from CustomMarshalers.dll (.NET 2 and .NET 4 tests)
					break;
				}

				// Preserve order of initialisation
				if (currentElementIndex <= lastElementIndex)
				{
					break;
				}

				List<Instruction> instructions = new List<Instruction>(assignment.Left.UnderlyingSameMethodInstructions);
				instructions.AddRange(assignment.MappedInstructions);
				inlinedExpressions[currentElementIndex] = assignment.Right.CloneAndAttachInstructions(instructions);

				lastElementIndex = currentElementIndex;
			}
			if (inlinedExpressions.Count == 0 || (arraySize - inlinedExpressions.Count) > MaxDefaultValues)
			{
				return false;
			}

			TypeReference arrayElementType = arrayCreation.ElementType;
			ExpressionCollection expressions = new ExpressionCollection();
			for (uint i = 0; i < arraySize; i++)
			{
				Expression currentExpression;
				if (!inlinedExpressions.ContainsKey(i))
				{
					currentExpression = GetTypeDefaultLiteralExpression(arrayElementType);
				}
				else
				{
					currentExpression = inlinedExpressions[i];
				}
				expressions.Add(currentExpression);
			}
			arrayCreation.Initializer = new InitializerExpression(expressions, InitializerType.ArrayInitializer);

			result = statements[startIndex];
			replacedStatementsCount = inlinedExpressions.Count + 1;
			return true;
		}

		private Expression GetTypeDefaultLiteralExpression(TypeReference arrayElementType)
		{
			if (arrayElementType.FullName == "System.Boolean")
			{
				return new LiteralExpression(false, typeSystem, null);
			}
			if (arrayElementType.IsIntegerType())
			{
				return new LiteralExpression(0, typeSystem, null);
			}
			TypeDefinition arrayElementTypeDef = arrayElementType.Resolve();
			if (arrayElementTypeDef == null)
			{
				return new DefaultObjectExpression(arrayElementType, null);
			}
			if (arrayElementTypeDef != null && arrayElementTypeDef.IsEnum)
			{
				return EnumHelper.GetEnumExpression(arrayElementTypeDef, new LiteralExpression(0, typeSystem, null), typeSystem);
			}
			if (!arrayElementTypeDef.IsValueType)
			{
				return new LiteralExpression(null, typeSystem, null);
			}
			return new DefaultObjectExpression(arrayElementType, null);
		}

		private uint GetAssignmentIndex(Expression assignee)
		{
			if (assignee.CodeNodeType != CodeNodeType.ArrayIndexerExpression)
			{
				throw new ArgumentOutOfRangeException("Expected ArrayIndexerExpression.");
			}

			ArrayIndexerExpression arrayIndexer = assignee as ArrayIndexerExpression;

			if (arrayIndexer.Indices.Count != 1)
			{
				throw new ArgumentOutOfRangeException("Expected one-dimentional array.");
			}

			Expression index = arrayIndexer.Indices[0];
			if (index.CodeNodeType != CodeNodeType.LiteralExpression)
			{
				throw new IndexOutOfRangeException();
			}
			return GetIndexFromLiteralExpression(index as LiteralExpression);
		}

		private bool IsArrayElementAssignment(BinaryExpression assignment, Expression assignee)
		{
			if (assignment == null || !assignment.IsAssignmentExpression)
			{
				return false;
			}
			if (assignment.Left.CodeNodeType != CodeNodeType.ArrayIndexerExpression)
			{
				return false;
			}
			ArrayIndexerExpression arrayIndexer = assignment.Left as ArrayIndexerExpression;

			if (arrayIndexer.Indices.Count != 1 || arrayIndexer.Indices[0].CodeNodeType != CodeNodeType.LiteralExpression)
			{
				return false;
			}

			if (!CompareTargets(assignee, arrayIndexer.Target))
			{
				return false;
			}
			return true;
		}

		private uint GetCreatedArraySize(ArrayCreationExpression arrayCreation)
		{
			if (arrayCreation.Dimensions.Count != 1)
			{
				throw new ArgumentOutOfRangeException("Expected one dimentional array creation.");
			}
			LiteralExpression dimention = arrayCreation.Dimensions[0] as LiteralExpression;
			if (dimention == null)
			{
				throw new ArgumentOutOfRangeException("Expected constant-size array");
			}

			return GetIndexFromLiteralExpression(dimention);
		}

		private uint GetIndexFromLiteralExpression(LiteralExpression dimention)
		{
			object boxedValue = dimention.Value;

			if (dimention.ExpressionType.FullName == "System.Int32")
			{
				int value = (int)boxedValue;
				if (value < 0)
				{
					throw new IndexOutOfRangeException();
				}
				return (uint)value;
			}
			if (dimention.ExpressionType.FullName == "System.UInt32")
			{
				return (uint)boxedValue;
			}

			if (dimention.ExpressionType.FullName == "System.Int16")
			{
				short value = (short)boxedValue;
				if (value < 0)
				{
					throw new IndexOutOfRangeException();
				}
				return (uint)value;
			}

			if (dimention.ExpressionType.FullName == "System.UInt16")
			{
				return (ushort)boxedValue;
			}

			if (dimention.ExpressionType.FullName == "System.Int8")
			{
				sbyte value = (sbyte)boxedValue;
				if (value < 0)
				{
					throw new IndexOutOfRangeException();
				}
				return (uint)value;
			}

			if (dimention.ExpressionType.FullName == "System.UInt8")
			{
				return (byte)boxedValue;
			}

			if (dimention.ExpressionType.FullName == "System.Int64")
			{
				long value = (long)boxedValue;
				if (value < 0 || value > uint.MaxValue)
				{
					throw new IndexOutOfRangeException();
				}
				return (uint)value;
			}

			if (dimention.ExpressionType.FullName == "System.UInt64")
			{
				ulong value = (ulong)boxedValue;
				if (value < 0 || value > uint.MaxValue)
				{
					throw new IndexOutOfRangeException();
				}
				return (uint)value;
			}
			throw new IndexOutOfRangeException();
		}
	}
}
