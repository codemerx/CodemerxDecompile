using System;
using System.Collections.Generic;
using Mono.Cecil;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps
{
	class DeduceImplicitDelegates : BaseCodeVisitor, IDecompilationStep
	{
		private DecompilationContext context;

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			Visit(body);
            if(this.context.MethodContext.CtorInvokeExpression != null)
            {
                Visit(this.context.MethodContext.CtorInvokeExpression);
            }
			return body;
		}

		public override void VisitBinaryExpression(BinaryExpression node)
		{
			base.VisitBinaryExpression(node);
			if (node.IsAssignmentExpression && node.Right.CodeNodeType == CodeNodeType.DelegateCreationExpression)
			{
				DelegateCreationExpression theDelegateCreation = (DelegateCreationExpression)node.Right;
				if (node.Left.HasType)
				{
					TypeReference leftType = node.Left.ExpressionType;
					if (CanInferTypeOfDelegateCreation(leftType)) 
					{
						theDelegateCreation.TypeIsImplicitlyInferable = true;
					}
				}
			}
		}
  
		private bool CanInferTypeOfDelegateCreation(TypeReference type)
		{
			if (type.IsGenericInstance)
			{
				return true;
			}
			TypeDefinition resolvedLeftType = type.Resolve();
			if (resolvedLeftType != null && !resolvedLeftType.IsAbstract)
			{
				return true;
			}
			return false;
		}

		public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			base.VisitMethodInvocationExpression(node);
			MethodReference method = node.MethodExpression.Method;
			TraverseMethodParameters(method, node.Arguments);
		}
  
		private void TraverseMethodParameters(MethodReference method, ExpressionCollection arguments)
		{
			for (int i = 0; i < method.Parameters.Count; i++)
			{
				if (arguments[i].CodeNodeType == CodeNodeType.DelegateCreationExpression)
				{
					DelegateCreationExpression theDelegateCreation = (DelegateCreationExpression)arguments[i];
					ParameterDefinition parameter = method.Parameters[i];
					if (CanInferTypeOfDelegateCreation(parameter.ParameterType))
					{
						theDelegateCreation.TypeIsImplicitlyInferable = true;
					}
				}
			}
		}

		public override void VisitReturnExpression(ReturnExpression node)
		{
			base.VisitReturnExpression(node);
			if (node.Value == null)
			{
				return;
			}
			if (node.Value.CodeNodeType == CodeNodeType.DelegateCreationExpression)
			{
				DelegateCreationExpression theDelegate = (DelegateCreationExpression)node.Value;
				TypeReference returnType = context.MethodContext.Method.ReturnType;
				if (CanInferTypeOfDelegateCreation(returnType))
				{
					theDelegate.TypeIsImplicitlyInferable = true;
				}
			}
		}

		public override void VisitObjectCreationExpression(ObjectCreationExpression node)
		{
			base.VisitObjectCreationExpression(node);
			MethodReference ctorMethod = node.Constructor;
			if (ctorMethod != null)
			{
				TraverseMethodParameters(ctorMethod, node.Arguments);
			}
		}

		public override void VisitBaseCtorExpression(BaseCtorExpression node)
		{
			base.VisitBaseCtorExpression(node);
			MethodReference baseCtor = node.MethodExpression.Method;
			TraverseMethodParameters(baseCtor, node.Arguments);
		}

		public override void VisitThisCtorExpression(ThisCtorExpression node)
		{
			base.VisitThisCtorExpression(node);
			MethodReference thisCtor = node.MethodExpression.Method;
			TraverseMethodParameters(thisCtor, node.Arguments);
		}
	}
}
