using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class FixBinaryExpressionsStep : BaseCodeTransformer, IDecompilationStep
	{
		private readonly TypeSystem typeSystem;

		public FixBinaryExpressionsStep(TypeSystem typeSystem)
		{
			base();
			this.typeSystem = typeSystem;
			return;
		}

		private bool CheckForOverloadedEqualityOperators(Expression expression, out TypeReference unresolvedReference)
		{
			unresolvedReference = null;
			V_1 = Extensions.ResolveToOverloadedEqualityOperator(expression.get_ExpressionType(), out V_0);
			if (V_1.get_HasValue())
			{
				return V_1.get_Value();
			}
			unresolvedReference = V_0;
			return true;
		}

		private ICodeNode FixBranchingExpression(Expression expression, Instruction branch)
		{
			if (branch.get_OpCode().get_Code() == 57)
			{
				stackVariable5 = true;
			}
			else
			{
				V_5 = branch.get_OpCode();
				stackVariable5 = V_5.get_Code() == 44;
			}
			V_0 = stackVariable5;
			V_1 = expression.get_ExpressionType();
			V_3 = 9;
			stackVariable10 = new Instruction[1];
			stackVariable10[0] = branch;
			V_4 = stackVariable10;
			if (V_0)
			{
				V_3 = 10;
			}
			if (String.op_Equality(V_1.get_Name(), "Boolean") || V_1.get_Name().Contains("Boolean "))
			{
				if (V_0)
				{
					V_6 = expression;
				}
				else
				{
					V_6 = Negator.Negate(expression, this.typeSystem);
				}
				if (expression as SafeCastExpression == null)
				{
					V_7 = new UnaryExpression(11, V_6, V_4);
				}
				else
				{
					V_7 = new BinaryExpression(V_3, expression, this.GetLiteralExpression(false, null), this.typeSystem, V_4, false);
				}
				return V_7;
			}
			if (String.op_Equality(V_1.get_Name(), "Char"))
			{
				V_2 = new BinaryExpression(V_3, expression, this.GetLiteralExpression('\0', null), this.typeSystem, V_4, false);
				V_2.set_ExpressionType(this.typeSystem.get_Boolean());
			}
			if (V_1.get_IsPrimitive())
			{
				V_2 = new BinaryExpression(V_3, expression, this.GetLiteralExpression(0, null), this.typeSystem, V_4, false);
				V_2.set_ExpressionType(this.typeSystem.get_Boolean());
			}
			else
			{
				V_8 = V_1.Resolve();
				if (V_8 == null || !V_8.get_IsEnum() || V_1.get_IsArray())
				{
					V_2 = new BinaryExpression(V_3, expression, this.GetLiteralExpression(null, null), this.typeSystem, V_4, false);
					V_2.set_ExpressionType(this.typeSystem.get_Boolean());
				}
				else
				{
					V_9 = null;
					V_10 = V_8.get_Fields().GetEnumerator();
					try
					{
						while (V_10.MoveNext())
						{
							V_11 = V_10.get_Current();
							if (V_11.get_Constant() == null || V_11.get_Constant().get_Value() == null || !V_11.get_Constant().get_Value().Equals(0))
							{
								continue;
							}
							V_9 = V_11;
							goto Label0;
						}
					}
					finally
					{
						V_10.Dispose();
					}
				Label0:
					if (V_9 != null)
					{
						V_2 = new BinaryExpression(V_3, expression, new EnumExpression(V_9, null), this.typeSystem, V_4, false);
						V_2.set_ExpressionType(this.typeSystem.get_Boolean());
					}
					else
					{
						V_2 = new BinaryExpression(V_3, expression, this.GetLiteralExpression(0, null), this.typeSystem, V_4, false);
						V_2.set_ExpressionType(this.typeSystem.get_Boolean());
					}
				}
			}
			return V_2;
		}

		private ICodeNode FixEqualityComparisonExpression(BinaryExpression expression)
		{
			if (expression.get_Right() as LiteralExpression == null)
			{
				return expression;
			}
			V_0 = this.GetElementType(expression.get_Left().get_ExpressionType());
			if (String.op_Inequality(V_0.get_FullName(), this.typeSystem.get_Boolean().get_FullName()))
			{
				V_2 = V_0.Resolve();
				if (V_0 != null && !V_0.get_IsPrimitive() && V_2 != null && !V_2.get_IsEnum())
				{
					expression.set_Right(this.GetLiteralExpression(null, null));
				}
				return expression;
			}
			V_1 = expression.get_Right() as LiteralExpression;
			if (!V_1.get_Value().Equals(0) && !V_1.get_Value().Equals(null))
			{
				return expression.get_Left();
			}
			return new UnaryExpression(1, expression.get_Left(), null);
		}

		private TypeReference GetElementType(TypeReference type)
		{
			if (type as IModifierType != null)
			{
				return (type as IModifierType).get_ElementType();
			}
			if (type as ByReferenceType == null)
			{
				return type;
			}
			return (type as ByReferenceType).get_ElementType();
		}

		private LiteralExpression GetLiteralExpression(object value, IEnumerable<Instruction> instructions)
		{
			return new LiteralExpression(value, this.typeSystem, instructions);
		}

		private bool IsArithmeticOperator(BinaryOperator operator)
		{
			if (operator == 1 || operator == 3 || operator == 5)
			{
				return true;
			}
			return operator == 7;
		}

		private bool IsBooleanAssignmentOperator(BinaryOperator operator)
		{
			if (operator == 26 || operator == 28 || operator == 29)
			{
				return true;
			}
			return operator == 30;
		}

		private bool IsFalse(Expression expression)
		{
			V_0 = expression as LiteralExpression;
			if (V_0 == null)
			{
				return false;
			}
			if (!V_0.get_Value().Equals(false) && !V_0.get_Value().Equals(0))
			{
				return false;
			}
			return true;
		}

		private bool NeedsPointerCast(BinaryExpression expression)
		{
			if (!expression.get_Left().get_ExpressionType().get_IsPointer() && !expression.get_Left().get_ExpressionType().get_IsByReference())
			{
				return false;
			}
			return String.op_Inequality(expression.get_Left().get_ExpressionType().GetElementType().get_FullName(), expression.get_Right().get_ExpressionType().GetElementType().get_FullName());
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			V_0 = context.get_MethodContext();
			V_1 = V_0.get_Expressions().get_BlockExpressions().get_Keys().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_3 = V_0.get_Expressions().get_BlockExpressions().get_Item(V_2);
					V_6 = V_0.get_ControlFlowGraph().get_InstructionToBlockMapping().get_Item(V_2).get_Last().get_OpCode();
					V_4 = V_6.get_Code();
					if (V_4 == 57 || V_4 == 44 || V_4 == 56)
					{
						stackVariable27 = true;
					}
					else
					{
						stackVariable27 = V_4 == 43;
					}
					V_5 = stackVariable27;
					V_7 = 0;
					while (V_7 < V_3.get_Count())
					{
						V_3.set_Item(V_7, (Expression)this.Visit(V_3.get_Item(V_7)));
						V_7 = V_7 + 1;
					}
					if (!V_5)
					{
						continue;
					}
					V_3.set_Item(V_3.get_Count() - 1, (Expression)this.FixBranchingExpression(V_3.get_Item(V_3.get_Count() - 1), V_0.get_ControlFlowGraph().get_InstructionToBlockMapping().get_Item(V_2).get_Last()));
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			return body;
		}

		private Expression SimplifyBooleanComparison(BinaryExpression expression)
		{
			V_0 = this.IsFalse(expression.get_Right());
			if (!V_0 || expression.get_Operator() != 9 && V_0 || expression.get_Operator() != 10)
			{
				return expression;
			}
			V_1 = new List<Instruction>(expression.get_MappedInstructions());
			V_1.AddRange(expression.get_Right().get_UnderlyingSameMethodInstructions());
			return Negator.Negate(expression.get_Left(), this.typeSystem).CloneAndAttachInstructions(V_1);
		}

		public override ICodeNode VisitBinaryExpression(BinaryExpression expression)
		{
			expression.set_Left((Expression)this.Visit(expression.get_Left()));
			expression.set_Right((Expression)this.Visit(expression.get_Right()));
			V_0 = expression.get_Left().get_ExpressionType();
			V_0 = this.GetElementType(V_0);
			if (V_0 != null)
			{
				if (String.op_Equality(V_0.get_FullName(), this.typeSystem.get_Char().get_FullName()))
				{
					if (expression.get_Right().get_CodeNodeType() == 22)
					{
						if (this.IsArithmeticOperator(expression.get_Operator()))
						{
							expression.set_ExpressionType(this.typeSystem.get_Char());
							return expression;
						}
						if (expression.get_Right().get_HasType())
						{
							V_2 = this.GetElementType(expression.get_Right().get_ExpressionType());
							if (String.op_Equality(V_0.get_FullName(), V_2.get_FullName()))
							{
								return expression;
							}
						}
						V_1 = expression.get_Right() as LiteralExpression;
						expression.set_Right(this.GetLiteralExpression((char)((Int32)V_1.get_Value()), V_1.get_MappedInstructions()));
					}
					if (String.op_Inequality(this.GetElementType(expression.get_Right().get_ExpressionType()).get_FullName(), this.typeSystem.get_Char().get_FullName()))
					{
						if (expression.get_Right().get_CodeNodeType() != 31 || !String.op_Equality(expression.get_Right().get_ExpressionType().get_FullName(), this.typeSystem.get_UInt16().get_FullName()))
						{
							expression.set_Right(new ExplicitCastExpression(expression.get_Right(), this.typeSystem.get_Char(), null));
						}
						else
						{
							((ExplicitCastExpression)expression.get_Right()).set_TargetType(this.typeSystem.get_Char());
						}
					}
				}
				if (String.op_Equality(V_0.get_FullName(), this.typeSystem.get_Boolean().get_FullName()) && expression.get_Right().get_CodeNodeType() == 22)
				{
					if (expression.get_Operator() == 9 || expression.get_Operator() == 10 || this.IsBooleanAssignmentOperator(expression.get_Operator()))
					{
						V_3 = expression.get_Right() as LiteralExpression;
						V_4 = true;
						if (V_3.get_Value() == null || V_3.get_Value().Equals(0) || V_3.get_Value().Equals(false) || V_3.get_Value().Equals(null))
						{
							V_4 = false;
						}
						expression.set_Right(this.GetLiteralExpression(V_4, V_3.get_MappedInstructions()));
					}
					if (expression.get_Operator() == 9 || expression.get_Operator() == 10)
					{
						return this.SimplifyBooleanComparison(expression);
					}
				}
			}
			if (expression.get_Operator() == 9 || expression.get_Operator() == 10)
			{
				V_5 = this.GetElementType(expression.get_Right().get_ExpressionType());
				if (V_5 != null && V_0 != null && String.op_Inequality(V_5.get_FullName(), V_0.get_FullName()))
				{
					return this.FixEqualityComparisonExpression(expression);
				}
			}
			if (expression.get_IsAssignmentExpression())
			{
				if (!this.NeedsPointerCast(expression))
				{
					if (expression.get_Left().get_HasType() && expression.get_Left().get_ExpressionType().get_IsByReference() || expression.get_Left().get_ExpressionType().get_IsPointer() || expression.get_Left().get_ExpressionType().get_IsArray() || !expression.get_Left().get_ExpressionType().get_IsPrimitive())
					{
						V_6 = expression.get_Left().get_ExpressionType().Resolve();
						if (V_6 != null && !V_6.get_IsEnum() && expression.get_Right() as LiteralExpression != null)
						{
							V_7 = expression.get_Right() as LiteralExpression;
							if (V_7.get_Value() != null && V_7.get_Value().Equals(0))
							{
								expression.set_Right(new LiteralExpression(null, this.typeSystem, expression.get_Right().get_UnderlyingSameMethodInstructions()));
							}
						}
					}
				}
				else
				{
					if (expression.get_Right().get_CodeNodeType() != 45)
					{
						expression.set_Right(new ExplicitCastExpression(expression.get_Right(), expression.get_Left().get_ExpressionType(), null));
					}
					else
					{
						expression.get_Right().set_ExpressionType(expression.get_Left().get_ExpressionType());
					}
				}
			}
			if (expression.get_Operator() == 15 && expression.get_MappedInstructions().Count<Instruction>() == 1 && expression.get_MappedInstructions().First<Instruction>().get_OpCode().get_Code() == 194)
			{
				V_9 = null;
				if (expression.get_Right().get_CodeNodeType() != 22)
				{
					if (expression.get_Right().get_CodeNodeType() == 31)
					{
						V_10 = expression.get_Right() as ExplicitCastExpression;
						if (V_10.get_Expression().get_CodeNodeType() == 22)
						{
							V_9 = V_10.get_Expression() as LiteralExpression;
						}
					}
				}
				else
				{
					V_9 = expression.get_Right() as LiteralExpression;
				}
				if (V_9 != null && V_9.get_Value() == null || V_9.get_Value().Equals(0))
				{
					expression.set_Operator(10);
				}
			}
			if (expression.get_IsObjectComparison())
			{
				V_11 = expression.get_Left();
				V_12 = expression.get_Right();
				if (this.CheckForOverloadedEqualityOperators(expression.get_Left(), out V_13) && this.CheckForOverloadedEqualityOperators(expression.get_Right(), out V_14))
				{
					expression.set_Left(new ExplicitCastExpression(V_11, V_11.get_ExpressionType().get_Module().get_TypeSystem().get_Object(), null, V_13));
					expression.set_Right(new ExplicitCastExpression(V_12, V_12.get_ExpressionType().get_Module().get_TypeSystem().get_Object(), null, V_14));
				}
			}
			return expression;
		}
	}
}