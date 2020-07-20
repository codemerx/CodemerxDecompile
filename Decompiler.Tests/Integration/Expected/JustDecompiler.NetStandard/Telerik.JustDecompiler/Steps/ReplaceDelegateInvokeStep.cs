using Mono.Cecil;
using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Steps
{
	public class ReplaceDelegateInvokeStep
	{
		private readonly BaseCodeTransformer codeTransformer;

		public ReplaceDelegateInvokeStep(BaseCodeTransformer codeTransformer)
		{
			base();
			this.codeTransformer = codeTransformer;
			return;
		}

		private bool IsDelegateInvokeMethod(MethodReference methodReference)
		{
			if (methodReference == null)
			{
				return false;
			}
			if (String.op_Inequality(methodReference.get_Name(), "Invoke"))
			{
				return false;
			}
			V_0 = methodReference.get_DeclaringType().Resolve();
			if (V_0 != null && V_0.get_BaseType() != null && String.op_Equality(V_0.get_BaseType().get_FullName(), "System.MulticastDelegate"))
			{
				return true;
			}
			return false;
		}

		public ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			if (node.get_MethodExpression().get_CodeNodeType() == 20)
			{
				V_0 = node.get_MethodExpression();
				V_1 = V_0.get_Method();
				if (this.IsDelegateInvokeMethod(V_1))
				{
					V_2 = (ExpressionCollection)this.codeTransformer.Visit(node.get_Arguments());
					return new DelegateInvokeExpression(V_0.get_Target(), V_2, V_1, node.get_InvocationInstructions());
				}
			}
			return null;
		}
	}
}