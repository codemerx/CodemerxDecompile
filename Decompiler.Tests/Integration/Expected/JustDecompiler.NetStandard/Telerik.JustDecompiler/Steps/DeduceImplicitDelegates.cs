using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.ObjectModel;
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
		}

		private bool CanInferTypeOfDelegateCreation(TypeReference type)
		{
			if (type.IsGenericInstance)
			{
				return true;
			}
			TypeDefinition typeDefinition = type.Resolve();
			if (typeDefinition != null && !typeDefinition.IsAbstract)
			{
				return true;
			}
			return false;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			this.Visit(body);
			if (this.context.MethodContext.CtorInvokeExpression != null)
			{
				this.Visit(this.context.MethodContext.CtorInvokeExpression);
			}
			return body;
		}

		private void TraverseMethodParameters(MethodReference method, ExpressionCollection arguments)
		{
			for (int i = 0; i < method.Parameters.Count; i++)
			{
				if (arguments[i].CodeNodeType == CodeNodeType.DelegateCreationExpression)
				{
					DelegateCreationExpression item = (DelegateCreationExpression)arguments[i];
					if (this.CanInferTypeOfDelegateCreation(method.Parameters[i].ParameterType))
					{
						item.TypeIsImplicitlyInferable = true;
					}
				}
			}
		}

		public override void VisitBaseCtorExpression(BaseCtorExpression node)
		{
			base.VisitBaseCtorExpression(node);
			this.TraverseMethodParameters(node.MethodExpression.Method, node.Arguments);
		}

		public override void VisitBinaryExpression(BinaryExpression node)
		{
			base.VisitBinaryExpression(node);
			if (node.IsAssignmentExpression && node.Right.CodeNodeType == CodeNodeType.DelegateCreationExpression)
			{
				DelegateCreationExpression right = (DelegateCreationExpression)node.Right;
				if (node.Left.HasType && this.CanInferTypeOfDelegateCreation(node.Left.ExpressionType))
				{
					right.TypeIsImplicitlyInferable = true;
				}
			}
		}

		public override void VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			base.VisitMethodInvocationExpression(node);
			this.TraverseMethodParameters(node.MethodExpression.Method, node.Arguments);
		}

		public override void VisitObjectCreationExpression(ObjectCreationExpression node)
		{
			base.VisitObjectCreationExpression(node);
			MethodReference constructor = node.Constructor;
			if (constructor != null)
			{
				this.TraverseMethodParameters(constructor, node.Arguments);
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
				DelegateCreationExpression value = (DelegateCreationExpression)node.Value;
				if (this.CanInferTypeOfDelegateCreation(this.context.MethodContext.Method.ReturnType))
				{
					value.TypeIsImplicitlyInferable = true;
				}
			}
		}

		public override void VisitThisCtorExpression(ThisCtorExpression node)
		{
			base.VisitThisCtorExpression(node);
			this.TraverseMethodParameters(node.MethodExpression.Method, node.Arguments);
		}
	}
}