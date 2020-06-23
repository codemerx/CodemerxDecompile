using Mono.Cecil;
using Mono.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler.Inlining
{
	internal class RestrictedVariableInliner : SimpleVariableInliner
	{
		public RestrictedVariableInliner(TypeSystem typeSystem) : base(typeSystem)
		{
		}

		protected override ICodeNode GetNewValue(VariableReferenceExpression node)
		{
			return this.@value.CloneAndAttachInstructions(node.UnderlyingSameMethodInstructions);
		}

		public override ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			node.MethodExpression = (MethodReferenceExpression)this.Visit(node.MethodExpression);
			if (this.status != SimpleVariableInliner.InliningResult.NotFound)
			{
				return node;
			}
			MethodReference method = node.MethodExpression.Method;
			for (int i = 0; i < node.Arguments.Count; i++)
			{
				if (!method.Parameters[i].ParameterType.IsByReference)
				{
					node.Arguments[i] = (Expression)this.Visit(node.Arguments[i]);
					if (this.status != SimpleVariableInliner.InliningResult.NotFound)
					{
						return node;
					}
				}
				else if (this.valueHasSideEffects && (new SideEffectsFinder()).HasSideEffectsRecursive(node.Arguments[i]))
				{
					this.status = SimpleVariableInliner.InliningResult.Abort;
					return node;
				}
			}
			return node;
		}
	}
}