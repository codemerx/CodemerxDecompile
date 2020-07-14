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
			this.codeTransformer = codeTransformer;
		}

		private bool IsDelegateInvokeMethod(MethodReference methodReference)
		{
			if (methodReference == null)
			{
				return false;
			}
			if (methodReference.get_Name() != "Invoke")
			{
				return false;
			}
			TypeDefinition typeDefinition = methodReference.get_DeclaringType().Resolve();
			if (typeDefinition != null && typeDefinition.get_BaseType() != null && typeDefinition.get_BaseType().get_FullName() == "System.MulticastDelegate")
			{
				return true;
			}
			return false;
		}

		public ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			if (node.MethodExpression.CodeNodeType == CodeNodeType.MethodReferenceExpression)
			{
				MethodReferenceExpression methodExpression = node.MethodExpression;
				MethodReference method = methodExpression.Method;
				if (this.IsDelegateInvokeMethod(method))
				{
					ExpressionCollection expressionCollection = (ExpressionCollection)this.codeTransformer.Visit(node.Arguments);
					return new DelegateInvokeExpression(methodExpression.Target, expressionCollection, method, node.InvocationInstructions);
				}
			}
			return null;
		}
	}
}