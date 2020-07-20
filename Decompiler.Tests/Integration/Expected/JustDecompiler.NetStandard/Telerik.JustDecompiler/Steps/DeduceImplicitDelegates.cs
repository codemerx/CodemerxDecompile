using Mono.Cecil;
using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class DeduceImplicitDelegates : BaseCodeVisitor, IDecompilationStep
	{
		private DecompilationContext context;

		public DeduceImplicitDelegates()
		{
			base();
			return;
		}

		private bool CanInferTypeOfDelegateCreation(TypeReference type)
		{
			if (type.get_IsGenericInstance())
			{
				return true;
			}
			V_0 = type.Resolve();
			if (V_0 != null && !V_0.get_IsAbstract())
			{
				return true;
			}
			return false;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			this.Visit(body);
			if (this.context.get_MethodContext().get_CtorInvokeExpression() != null)
			{
				this.Visit(this.context.get_MethodContext().get_CtorInvokeExpression());
			}
			return body;
		}

		private void TraverseMethodParameters(MethodReference method, ExpressionCollection arguments)
		{
			V_0 = 0;
			while (V_0 < method.get_Parameters().get_Count())
			{
				if (arguments.get_Item(V_0).get_CodeNodeType() == 21)
				{
					V_1 = (DelegateCreationExpression)arguments.get_Item(V_0);
					if (this.CanInferTypeOfDelegateCreation(method.get_Parameters().get_Item(V_0).get_ParameterType()))
					{
						V_1.set_TypeIsImplicitlyInferable(true);
					}
				}
				V_0 = V_0 + 1;
			}
			return;
		}

		public override void VisitBaseCtorExpression(BaseCtorExpression node)
		{
			this.VisitBaseCtorExpression(node);
			V_0 = node.get_MethodExpression().get_Method();
			this.TraverseMethodParameters(V_0, node.get_Arguments());
			return;
		}

		public override void VisitBinaryExpression(BinaryExpression node)
		{
			this.VisitBinaryExpression(node);
			if (node.get_IsAssignmentExpression() && node.get_Right().get_CodeNodeType() == 21)
			{
				V_0 = (DelegateCreationExpression)node.get_Right();
				if (node.get_Left().get_HasType() && this.CanInferTypeOfDelegateCreation(node.get_Left().get_ExpressionType()))
				{
					V_0.set_TypeIsImplicitlyInferable(true);
				}
			}
			return;
		}

		public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			this.VisitMethodInvocationExpression(node);
			V_0 = node.get_MethodExpression().get_Method();
			this.TraverseMethodParameters(V_0, node.get_Arguments());
			return;
		}

		public override void VisitObjectCreationExpression(ObjectCreationExpression node)
		{
			this.VisitObjectCreationExpression(node);
			V_0 = node.get_Constructor();
			if (V_0 != null)
			{
				this.TraverseMethodParameters(V_0, node.get_Arguments());
			}
			return;
		}

		public override void VisitReturnExpression(ReturnExpression node)
		{
			this.VisitReturnExpression(node);
			if (node.get_Value() == null)
			{
				return;
			}
			if (node.get_Value().get_CodeNodeType() == 21)
			{
				V_0 = (DelegateCreationExpression)node.get_Value();
				if (this.CanInferTypeOfDelegateCreation(this.context.get_MethodContext().get_Method().get_ReturnType()))
				{
					V_0.set_TypeIsImplicitlyInferable(true);
				}
			}
			return;
		}

		public override void VisitThisCtorExpression(ThisCtorExpression node)
		{
			this.VisitThisCtorExpression(node);
			V_0 = node.get_MethodExpression().get_Method();
			this.TraverseMethodParameters(V_0, node.get_Arguments());
			return;
		}
	}
}