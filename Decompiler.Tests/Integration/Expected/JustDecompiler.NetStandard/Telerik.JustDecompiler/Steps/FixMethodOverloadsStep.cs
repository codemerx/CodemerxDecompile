using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class FixMethodOverloadsStep : BaseCodeVisitor, IDecompilationStep
	{
		private DecompilationContext context;

		public FixMethodOverloadsStep()
		{
			base();
			return;
		}

		private bool ArgumentsMatchParameters(Collection<ParameterDefinition> parameters, ExpressionCollection arguments)
		{
			V_0 = 0;
			while (V_0 < parameters.get_Count())
			{
				V_1 = parameters.get_Item(V_0).get_ParameterType().Resolve();
				V_2 = arguments.get_Item(V_0);
				if (!V_2.get_HasType())
				{
					return true;
				}
				V_3 = V_2.get_ExpressionType().Resolve();
				if (V_1 == null || V_3 == null)
				{
					return true;
				}
				if (V_2.get_CodeNodeType() != 22 || ((LiteralExpression)V_2).get_Value() != null || V_1.get_IsValueType() && String.op_Inequality(V_1.get_FullName(), V_3.get_FullName()) && !this.IsTypeDescendantOf(V_3, V_1))
				{
					return false;
				}
				V_0 = V_0 + 1;
			}
			return true;
		}

		private void FixArguments(MethodReference method, ExpressionCollection arguments)
		{
			V_0 = method.get_DeclaringType().Resolve();
			if (V_0 == null)
			{
				return;
			}
			V_1 = this.GetSameNameMethods(V_0, method, arguments);
			if (V_1.get_Count() > 0)
			{
				V_2 = 0;
				while (V_2 < arguments.get_Count())
				{
					V_3 = method.get_Parameters().get_Item(V_2).ResolveParameterType(method);
					if (arguments.get_Item(V_2).get_HasType() && !String.op_Equality(arguments.get_Item(V_2).get_ExpressionType().get_FullName(), V_3.get_FullName()) && this.ShouldAddCast(arguments.get_Item(V_2), V_1, V_2, V_3))
					{
						arguments.set_Item(V_2, new ExplicitCastExpression(arguments.get_Item(V_2), V_3, null));
					}
					V_2 = V_2 + 1;
				}
			}
			return;
		}

		private List<MethodDefinition> GetSameNameMethods(TypeDefinition declaringTypeDefinition, MethodReference method, ExpressionCollection arguments)
		{
			V_0 = new List<MethodDefinition>();
			V_1 = method.Resolve();
			if (V_1 == null)
			{
				return V_0;
			}
			V_2 = declaringTypeDefinition.get_Methods().GetEnumerator();
			try
			{
				while (V_2.MoveNext())
				{
					V_3 = V_2.get_Current();
					if (String.op_Inequality(V_3.get_Name(), method.get_Name()) || V_3.get_HasParameters() != method.get_HasParameters() || V_3.get_Parameters().get_Count() != method.get_Parameters().get_Count() || (object)V_3 == (object)V_1 || V_3.get_HasGenericParameters() != V_1.get_HasGenericParameters() || !this.ArgumentsMatchParameters(V_3.get_Parameters(), arguments))
					{
						continue;
					}
					V_0.Add(V_3);
				}
			}
			finally
			{
				V_2.Dispose();
			}
			return V_0;
		}

		private bool IsTypeDescendantOf(TypeReference descendant, TypeReference ancestor)
		{
			if (descendant.get_IsGenericParameter())
			{
				return true;
			}
			V_0 = descendant.Resolve();
			if (V_0 == null)
			{
				return false;
			}
			if (descendant.get_IsArray() == ancestor.get_IsArray())
			{
				while (V_0 != null)
				{
					if (V_0.get_BaseType() != null && String.op_Equality(V_0.get_BaseType().get_FullName(), ancestor.get_FullName()))
					{
						return true;
					}
					if (V_0.get_HasInterfaces())
					{
						V_1 = V_0.get_Interfaces().GetEnumerator();
						try
						{
							while (V_1.MoveNext())
							{
								if (!String.op_Equality(V_1.get_Current().get_FullName(), ancestor.get_FullName()))
								{
									continue;
								}
								V_2 = true;
								goto Label0;
							}
						}
						finally
						{
							V_1.Dispose();
						}
					}
					if (V_0.get_BaseType() == null)
					{
						stackVariable16 = null;
					}
					else
					{
						stackVariable16 = V_0.get_BaseType().Resolve();
					}
					V_0 = stackVariable16;
				}
				return false;
			}
			else
			{
				V_1 = V_0.get_Interfaces().GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						if (!String.op_Equality(V_1.get_Current().get_FullName(), ancestor.get_FullName()))
						{
							continue;
						}
						V_2 = true;
						goto Label0;
					}
				}
				finally
				{
					V_1.Dispose();
				}
				return false;
			}
		Label0:
			return V_2;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			this.VisitBlockStatement(body);
			return body;
		}

		private bool ShouldAddCast(Expression argument, List<MethodDefinition> sameNameMethods, int argumentIndex, TypeReference calledMethodParamType)
		{
			if (!argument.get_HasType())
			{
				return true;
			}
			V_0 = argument.get_ExpressionType();
			V_1 = sameNameMethods.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current().get_Parameters().get_Item(argumentIndex).get_ParameterType();
					if (this.IsTypeDescendantOf(calledMethodParamType, V_2) || String.op_Equality(calledMethodParamType.get_FullName(), V_2.get_FullName()))
					{
						continue;
					}
					if (this.IsTypeDescendantOf(V_0, V_2) || String.op_Equality(V_0.get_FullName(), V_2.get_FullName()))
					{
						V_3 = true;
						goto Label1;
					}
					else
					{
						if (argument.get_CodeNodeType() != 22 || ((LiteralExpression)argument).get_Value() != null || V_2.get_IsValueType())
						{
							continue;
						}
						V_3 = true;
						goto Label1;
					}
				}
				goto Label0;
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
		Label1:
			return V_3;
		Label0:
			return false;
		}

		public override void VisitBaseCtorExpression(BaseCtorExpression node)
		{
			this.VisitBaseCtorExpression(node);
			V_0 = node.get_MethodExpression().get_Method();
			if (node.get_Arguments().get_Count() > 0)
			{
				this.FixArguments(V_0, node.get_Arguments());
			}
			return;
		}

		public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			this.VisitMethodInvocationExpression(node);
			if (node.get_MethodExpression().get_CodeNodeType() == 20)
			{
				this.FixArguments(node.get_MethodExpression().get_Method(), node.get_Arguments());
			}
			return;
		}

		public override void VisitObjectCreationExpression(ObjectCreationExpression node)
		{
			this.VisitObjectCreationExpression(node);
			V_0 = node.get_Constructor();
			if (V_0 != null)
			{
				this.FixArguments(V_0, node.get_Arguments());
			}
			return;
		}

		public override void VisitThisCtorExpression(ThisCtorExpression node)
		{
			this.VisitThisCtorExpression(node);
			V_0 = node.get_MethodExpression().get_Method();
			if (node.get_Arguments().get_Count() > 0)
			{
				this.FixArguments(V_0, node.get_Arguments());
			}
			return;
		}
	}
}