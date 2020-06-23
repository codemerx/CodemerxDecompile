using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Steps;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	internal class ArrayInitialisationPattern : BaseInitialisationPattern
	{
		private const uint MaxDefaultValues = 10;

		public ArrayInitialisationPattern(CodePatternsContext patternsContext, TypeSystem ts) : base(patternsContext, ts)
		{
		}

		private uint GetAssignmentIndex(Expression assignee)
		{
			if (assignee.CodeNodeType != CodeNodeType.ArrayIndexerExpression)
			{
				throw new ArgumentOutOfRangeException("Expected ArrayIndexerExpression.");
			}
			ArrayIndexerExpression arrayIndexerExpression = assignee as ArrayIndexerExpression;
			if (arrayIndexerExpression.Indices.Count != 1)
			{
				throw new ArgumentOutOfRangeException("Expected one-dimentional array.");
			}
			Expression item = arrayIndexerExpression.Indices[0];
			if (item.CodeNodeType != CodeNodeType.LiteralExpression)
			{
				throw new IndexOutOfRangeException();
			}
			return this.GetIndexFromLiteralExpression(item as LiteralExpression);
		}

		private uint GetCreatedArraySize(ArrayCreationExpression arrayCreation)
		{
			if (arrayCreation.Dimensions.Count != 1)
			{
				throw new ArgumentOutOfRangeException("Expected one dimentional array creation.");
			}
			LiteralExpression item = arrayCreation.Dimensions[0] as LiteralExpression;
			if (item == null)
			{
				throw new ArgumentOutOfRangeException("Expected constant-size array");
			}
			return this.GetIndexFromLiteralExpression(item);
		}

		private uint GetIndexFromLiteralExpression(LiteralExpression dimention)
		{
			object value = dimention.Value;
			if (dimention.ExpressionType.FullName == "System.Int32")
			{
				Int32 num = (Int32)value;
				if (num < 0)
				{
					throw new IndexOutOfRangeException();
				}
				return (uint)num;
			}
			if (dimention.ExpressionType.FullName == "System.UInt32")
			{
				return (UInt32)value;
			}
			if (dimention.ExpressionType.FullName == "System.Int16")
			{
				Int16 num1 = (Int16)value;
				if (num1 < 0)
				{
					throw new IndexOutOfRangeException();
				}
				return (uint)num1;
			}
			if (dimention.ExpressionType.FullName == "System.UInt16")
			{
				return (UInt16)value;
			}
			if (dimention.ExpressionType.FullName == "System.Int8")
			{
				SByte num2 = (SByte)value;
				if (num2 < 0)
				{
					throw new IndexOutOfRangeException();
				}
				return (uint)num2;
			}
			if (dimention.ExpressionType.FullName == "System.UInt8")
			{
				return (Byte)value;
			}
			if (dimention.ExpressionType.FullName == "System.Int64")
			{
				long num3 = (Int64)value;
				if (num3 < (long)0 || num3 > (ulong)-1)
				{
					throw new IndexOutOfRangeException();
				}
				return (uint)num3;
			}
			if (dimention.ExpressionType.FullName != "System.UInt64")
			{
				throw new IndexOutOfRangeException();
			}
			ulong num4 = (UInt64)value;
			if (num4 < (long)0 || num4 > (ulong)-1)
			{
				throw new IndexOutOfRangeException();
			}
			return (uint)num4;
		}

		private Expression GetTypeDefaultLiteralExpression(TypeReference arrayElementType)
		{
			if (arrayElementType.FullName == "System.Boolean")
			{
				return new LiteralExpression(false, this.typeSystem, null);
			}
			if (arrayElementType.IsIntegerType())
			{
				return new LiteralExpression((object)0, this.typeSystem, null);
			}
			TypeDefinition typeDefinition = arrayElementType.Resolve();
			if (typeDefinition == null)
			{
				return new DefaultObjectExpression(arrayElementType, null);
			}
			if (typeDefinition != null && typeDefinition.IsEnum)
			{
				return EnumHelper.GetEnumExpression(typeDefinition, new LiteralExpression((object)0, this.typeSystem, null), this.typeSystem);
			}
			if (typeDefinition.IsValueType)
			{
				return new DefaultObjectExpression(arrayElementType, null);
			}
			return new LiteralExpression(null, this.typeSystem, null);
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
			ArrayIndexerExpression left = assignment.Left as ArrayIndexerExpression;
			if (left.Indices.Count != 1 || left.Indices[0].CodeNodeType != CodeNodeType.LiteralExpression)
			{
				return false;
			}
			if (!base.CompareTargets(assignee, left.Target))
			{
				return false;
			}
			return true;
		}

		protected override bool TryMatchInternal(StatementCollection statements, int startIndex, out Statement result, out int replacedStatementsCount)
		{
			ArrayCreationExpression initializerExpression;
			Expression expression;
			Expression expression1;
			Expression expression2;
			result = null;
			replacedStatementsCount = 0;
			if (!base.TryGetArrayCreation(statements, startIndex, out initializerExpression, out expression))
			{
				return false;
			}
			if (initializerExpression.Initializer != null && initializerExpression.Initializer.Expressions.Count != 0)
			{
				return false;
			}
			uint createdArraySize = this.GetCreatedArraySize(initializerExpression);
			Dictionary<uint, Expression> nums = new Dictionary<uint, Expression>();
			long num = (long)-1;
			for (int i = startIndex + 1; i < statements.Count && base.TryGetNextExpression(statements[i], out expression1); i++)
			{
				BinaryExpression binaryExpression = (statements[i] as ExpressionStatement).Expression as BinaryExpression;
				if (!this.IsArrayElementAssignment(binaryExpression, expression))
				{
					break;
				}
				uint assignmentIndex = this.GetAssignmentIndex(binaryExpression.Left);
				if (assignmentIndex >= createdArraySize || (ulong)assignmentIndex <= num)
				{
					break;
				}
				List<Instruction> instructions = new List<Instruction>(binaryExpression.Left.UnderlyingSameMethodInstructions);
				instructions.AddRange(binaryExpression.MappedInstructions);
				nums[assignmentIndex] = binaryExpression.Right.CloneAndAttachInstructions(instructions);
				num = (long)assignmentIndex;
			}
			if (nums.Count == 0 || (ulong)createdArraySize - (long)nums.Count > (long)10)
			{
				return false;
			}
			TypeReference elementType = initializerExpression.ElementType;
			ExpressionCollection expressionCollection = new ExpressionCollection();
			for (uint j = 0; j < createdArraySize; j++)
			{
				expression2 = (nums.ContainsKey(j) ? nums[j] : this.GetTypeDefaultLiteralExpression(elementType));
				expressionCollection.Add(expression2);
			}
			initializerExpression.Initializer = new InitializerExpression(expressionCollection, InitializerType.ArrayInitializer);
			result = statements[startIndex];
			replacedStatementsCount = nums.Count + 1;
			return true;
		}
	}
}