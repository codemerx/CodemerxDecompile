using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Decompiler.TypeInference
{
	internal class UsedAsTypeHelper
	{
		private readonly TypeSystem typeSystem;

		private readonly MethodSpecificContext methodContext;

		public UsedAsTypeHelper(MethodSpecificContext methodContext)
		{
			base();
			this.methodContext = methodContext;
			this.typeSystem = methodContext.get_Method().get_Module().get_TypeSystem();
			return;
		}

		public TypeReference GetUseExpressionTypeNode(Instruction instruction, Expression instructionExpression, VariableReference variable)
		{
			V_0 = instruction.get_OpCode().get_Code();
			if (V_0 == 112)
			{
				return instruction.get_Operand() as TypeReference;
			}
			if (UsedAsTypeHelper.IsConditionalBranch(V_0))
			{
				return this.typeSystem.get_Boolean();
			}
			if (V_0 == 37)
			{
				return null;
			}
			return this.GetUseExpressionTypeNode(instructionExpression, variable);
		}

		private TypeReference GetUseExpressionTypeNode(Expression expression, VariableReference variable)
		{
			V_1 = expression.get_CodeNodeType();
			if (V_1 > 42)
			{
				if (V_1 - 52 <= 1)
				{
					goto Label0;
				}
				if (V_1 == 57)
				{
					return this.methodContext.get_Method().get_FixedReturnType();
				}
				if (V_1 == 62)
				{
					return (expression as BoxExpression).get_BoxedAs();
				}
			}
			else
			{
				if (V_1 == 6)
				{
					return null;
				}
				switch (V_1 - 19)
				{
					case 0:
					{
						goto Label0;
					}
					case 1:
					case 2:
					case 3:
					case 6:
					case 9:
					case 10:
					case 13:
					{
						break;
					}
					case 4:
					{
						return this.GetUseExpressionTypeNode((expression as UnaryExpression).get_Operand(), variable);
					}
					case 5:
					{
						return this.GetUseInBinaryExpression(expression as BinaryExpression, variable);
					}
					case 7:
					{
						V_0 = expression as VariableReferenceExpression;
						if ((object)V_0.get_Variable() != (object)variable)
						{
							return null;
						}
						return V_0.get_Variable().get_VariableType();
					}
					case 8:
					{
						return (expression as VariableDeclarationExpression).get_Variable().get_VariableType();
					}
					case 11:
					{
						return (expression as FieldReferenceExpression).get_Field().get_DeclaringType();
					}
					case 12:
					case 14:
					{
						return this.typeSystem.get_Object();
					}
					default:
					{
						switch (V_1 - 38)
						{
							case 0:
							{
								return this.GetUseInArrayCreation(expression as ArrayCreationExpression, variable);
							}
							case 1:
							{
								return this.GetUseInArrayIndexer(expression as ArrayIndexerExpression, variable);
							}
							case 2:
							{
								return this.GetUseInObjectCreation(expression as ObjectCreationExpression, variable);
							}
							case 3:
							{
								break;
							}
							case 4:
							{
								goto Label0;
							}
						}
						break;
					}
				}
			}
			throw new ArgumentOutOfRangeException("Expression is not evaluated to any type.");
		Label0:
			return this.GetUseInMethodInvocation(expression as MethodInvocationExpression, variable);
		}

		private TypeReference GetUseInArrayCreation(ArrayCreationExpression arrayCreationExpression, VariableReference variable)
		{
			V_0 = arrayCreationExpression.get_Dimensions().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (V_1 as VariableReferenceExpression == null || (object)(V_1 as VariableReferenceExpression).get_Variable() != (object)variable)
					{
						continue;
					}
					V_2 = this.typeSystem.get_Int32();
					goto Label0;
				}
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
			V_0 = arrayCreationExpression.get_Initializer().get_Expressions().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_3 = V_0.get_Current();
					if (V_3 as VariableReferenceExpression == null || (object)(V_3 as VariableReferenceExpression).get_Variable() != (object)variable)
					{
						continue;
					}
					V_2 = arrayCreationExpression.get_ElementType();
					goto Label0;
				}
				goto Label1;
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
		Label0:
			return V_2;
		Label1:
			throw new ArgumentOutOfRangeException("Expression is not evaluated to any type.");
		}

		private TypeReference GetUseInArrayIndexer(ArrayIndexerExpression arrayIndexerExpression, VariableReference variable)
		{
			V_0 = arrayIndexerExpression.get_Indices().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (V_1 as VariableReferenceExpression == null || (object)(V_1 as VariableReferenceExpression).get_Variable() != (object)variable)
					{
						continue;
					}
					V_2 = this.typeSystem.get_Int32();
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
		Label1:
			return V_2;
		Label0:
			return new TypeReference("System", "Array", this.typeSystem.get_Object().get_Module(), this.typeSystem.get_Object().get_Scope());
		}

		private TypeReference GetUseInBinaryExpression(BinaryExpression binaryExpression, VariableReference variable)
		{
			if (binaryExpression.get_Right().get_CodeNodeType() == 26 && (object)(binaryExpression.get_Right() as VariableReferenceExpression).get_Variable() == (object)variable)
			{
				return binaryExpression.get_Left().get_ExpressionType();
			}
			if (binaryExpression.get_Left() as VariableReferenceExpression != null && (object)(binaryExpression.get_Left() as VariableReferenceExpression).get_Variable() == (object)variable)
			{
				return binaryExpression.get_Right().get_ExpressionType();
			}
			stackVariable11 = this.GetUseExpressionTypeNode(binaryExpression.get_Left(), variable);
			if (stackVariable11 == null)
			{
				dummyVar0 = stackVariable11;
				stackVariable11 = this.GetUseExpressionTypeNode(binaryExpression.get_Right(), variable);
			}
			return stackVariable11;
		}

		private TypeReference GetUseInMethodInvocation(MethodInvocationExpression methodInvocationExpression, VariableReference variable)
		{
			V_0 = null;
			V_1 = methodInvocationExpression.get_Arguments().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (V_2 as VariableReferenceExpression == null || (object)(V_2 as VariableReferenceExpression).get_Variable() != (object)variable)
					{
						continue;
					}
					V_0 = V_2;
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			if (V_0 == null)
			{
				if ((object)(methodInvocationExpression.get_MethodExpression().get_Target() as VariableReferenceExpression).get_Variable() != (object)variable)
				{
					return null;
				}
				return methodInvocationExpression.get_MethodExpression().get_Member().get_DeclaringType();
			}
			V_3 = methodInvocationExpression.get_Arguments().IndexOf(V_0);
			V_4 = methodInvocationExpression.get_MethodExpression().get_Method();
			return V_4.get_Parameters().get_Item(V_3).ResolveParameterType(V_4);
		}

		private TypeReference GetUseInObjectCreation(ObjectCreationExpression objectCreationExpression, VariableReference variable)
		{
			V_0 = null;
			V_1 = objectCreationExpression.get_Arguments().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (V_2 as VariableReferenceExpression == null || (object)(V_2 as VariableReferenceExpression).get_Variable() != (object)variable)
					{
						continue;
					}
					V_0 = V_2;
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			return objectCreationExpression.get_Constructor().get_Parameters().get_Item(objectCreationExpression.get_Arguments().IndexOf(V_0)).ResolveParameterType(objectCreationExpression.get_Constructor());
		}

		private static bool IsConditionalBranch(Code instructionOpCode)
		{
			if (instructionOpCode == 57 || instructionOpCode == 44 || instructionOpCode == 56)
			{
				return true;
			}
			return instructionOpCode == 43;
		}
	}
}