using Mono.Cecil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	internal class ArrayInitialisationPattern : BaseInitialisationPattern
	{
		private const uint MaxDefaultValues = 10;

		public ArrayInitialisationPattern(CodePatternsContext patternsContext, TypeSystem ts)
		{
			base(patternsContext, ts);
			return;
		}

		private uint GetAssignmentIndex(Expression assignee)
		{
			if (assignee.get_CodeNodeType() != 39)
			{
				throw new ArgumentOutOfRangeException("Expected ArrayIndexerExpression.");
			}
			stackVariable4 = assignee as ArrayIndexerExpression;
			if (stackVariable4.get_Indices().get_Count() != 1)
			{
				throw new ArgumentOutOfRangeException("Expected one-dimentional array.");
			}
			V_0 = stackVariable4.get_Indices().get_Item(0);
			if (V_0.get_CodeNodeType() != 22)
			{
				throw new IndexOutOfRangeException();
			}
			return this.GetIndexFromLiteralExpression(V_0 as LiteralExpression);
		}

		private uint GetCreatedArraySize(ArrayCreationExpression arrayCreation)
		{
			if (arrayCreation.get_Dimensions().get_Count() != 1)
			{
				throw new ArgumentOutOfRangeException("Expected one dimentional array creation.");
			}
			V_0 = arrayCreation.get_Dimensions().get_Item(0) as LiteralExpression;
			if (V_0 == null)
			{
				throw new ArgumentOutOfRangeException("Expected constant-size array");
			}
			return this.GetIndexFromLiteralExpression(V_0);
		}

		private uint GetIndexFromLiteralExpression(LiteralExpression dimention)
		{
			V_0 = dimention.get_Value();
			if (String.op_Equality(dimention.get_ExpressionType().get_FullName(), "System.Int32"))
			{
				stackVariable80 = (Int32)V_0;
				if (stackVariable80 < 0)
				{
					throw new IndexOutOfRangeException();
				}
				return stackVariable80;
			}
			if (String.op_Equality(dimention.get_ExpressionType().get_FullName(), "System.UInt32"))
			{
				return (UInt32)V_0;
			}
			if (String.op_Equality(dimention.get_ExpressionType().get_FullName(), "System.Int16"))
			{
				stackVariable74 = (Int16)V_0;
				if (stackVariable74 < 0)
				{
					throw new IndexOutOfRangeException();
				}
				return stackVariable74;
			}
			if (String.op_Equality(dimention.get_ExpressionType().get_FullName(), "System.UInt16"))
			{
				return (UInt16)V_0;
			}
			if (String.op_Equality(dimention.get_ExpressionType().get_FullName(), "System.Int8"))
			{
				stackVariable68 = (SByte)V_0;
				if (stackVariable68 < 0)
				{
					throw new IndexOutOfRangeException();
				}
				return stackVariable68;
			}
			if (String.op_Equality(dimention.get_ExpressionType().get_FullName(), "System.UInt8"))
			{
				return (Byte)V_0;
			}
			if (String.op_Equality(dimention.get_ExpressionType().get_FullName(), "System.Int64"))
			{
				V_1 = (Int64)V_0;
				if (V_1 < (long)0 || V_1 > (ulong)-1)
				{
					throw new IndexOutOfRangeException();
				}
				return (uint)V_1;
			}
			if (!String.op_Equality(dimention.get_ExpressionType().get_FullName(), "System.UInt64"))
			{
				throw new IndexOutOfRangeException();
			}
			V_2 = (UInt64)V_0;
			if (V_2 < (long)0 || V_2 > (ulong)-1)
			{
				throw new IndexOutOfRangeException();
			}
			return (uint)V_2;
		}

		private Expression GetTypeDefaultLiteralExpression(TypeReference arrayElementType)
		{
			if (String.op_Equality(arrayElementType.get_FullName(), "System.Boolean"))
			{
				return new LiteralExpression(false, this.typeSystem, null);
			}
			if (arrayElementType.IsIntegerType())
			{
				return new LiteralExpression((object)0, this.typeSystem, null);
			}
			V_0 = arrayElementType.Resolve();
			if (V_0 == null)
			{
				return new DefaultObjectExpression(arrayElementType, null);
			}
			if (V_0 != null && V_0.get_IsEnum())
			{
				return EnumHelper.GetEnumExpression(V_0, new LiteralExpression((object)0, this.typeSystem, null), this.typeSystem);
			}
			if (V_0.get_IsValueType())
			{
				return new DefaultObjectExpression(arrayElementType, null);
			}
			return new LiteralExpression(null, this.typeSystem, null);
		}

		private bool IsArrayElementAssignment(BinaryExpression assignment, Expression assignee)
		{
			if (assignment == null || !assignment.get_IsAssignmentExpression())
			{
				return false;
			}
			if (assignment.get_Left().get_CodeNodeType() != 39)
			{
				return false;
			}
			V_0 = assignment.get_Left() as ArrayIndexerExpression;
			if (V_0.get_Indices().get_Count() != 1 || V_0.get_Indices().get_Item(0).get_CodeNodeType() != 22)
			{
				return false;
			}
			if (!this.CompareTargets(assignee, V_0.get_Target()))
			{
				return false;
			}
			return true;
		}

		protected override bool TryMatchInternal(StatementCollection statements, int startIndex, out Statement result, out int replacedStatementsCount)
		{
			result = null;
			replacedStatementsCount = 0;
			if (!this.TryGetArrayCreation(statements, startIndex, out V_0, out V_1))
			{
				return false;
			}
			if (V_0.get_Initializer() != null && V_0.get_Initializer().get_Expressions().get_Count() != 0)
			{
				return false;
			}
			V_2 = this.GetCreatedArraySize(V_0);
			V_3 = new Dictionary<uint, Expression>();
			V_4 = (long)-1;
			V_7 = startIndex + 1;
			while (V_7 < statements.get_Count() && this.TryGetNextExpression(statements.get_Item(V_7), out V_8))
			{
				V_9 = (statements.get_Item(V_7) as ExpressionStatement).get_Expression() as BinaryExpression;
				if (!this.IsArrayElementAssignment(V_9, V_1))
				{
					break;
				}
				V_10 = this.GetAssignmentIndex(V_9.get_Left());
				if (V_10 >= V_2 || (ulong)V_10 <= V_4)
				{
					break;
				}
				V_11 = new List<Instruction>(V_9.get_Left().get_UnderlyingSameMethodInstructions());
				V_11.AddRange(V_9.get_MappedInstructions());
				V_3.set_Item(V_10, V_9.get_Right().CloneAndAttachInstructions(V_11));
				V_4 = (ulong)V_10;
				V_7 = V_7 + 1;
			}
			if (V_3.get_Count() == 0 || (ulong)V_2 - (long)V_3.get_Count() > (long)10)
			{
				return false;
			}
			V_5 = V_0.get_ElementType();
			V_6 = new ExpressionCollection();
			V_12 = 0;
			while (V_12 < V_2)
			{
				if (V_3.ContainsKey(V_12))
				{
					V_13 = V_3.get_Item(V_12);
				}
				else
				{
					V_13 = this.GetTypeDefaultLiteralExpression(V_5);
				}
				V_6.Add(V_13);
				V_12 = V_12 + 1;
			}
			V_0.set_Initializer(new InitializerExpression(V_6, 2));
			result = statements.get_Item(startIndex);
			replacedStatementsCount = V_3.get_Count() + 1;
			return true;
		}
	}
}