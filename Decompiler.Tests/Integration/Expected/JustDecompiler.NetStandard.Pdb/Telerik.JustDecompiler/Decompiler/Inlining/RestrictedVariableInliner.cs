using Mono.Cecil;
using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler.Inlining
{
	internal class RestrictedVariableInliner : SimpleVariableInliner
	{
		public RestrictedVariableInliner(TypeSystem typeSystem)
		{
			base(typeSystem);
			return;
		}

		protected override ICodeNode GetNewValue(VariableReferenceExpression node)
		{
			return this.value.CloneAndAttachInstructions(node.get_UnderlyingSameMethodInstructions());
		}

		public override ICodeNode VisitMethodInvocationExpression(MethodInvocationExpression node)
		{
			node.set_MethodExpression((MethodReferenceExpression)this.Visit(node.get_MethodExpression()));
			if (this.status != SimpleVariableInliner.InliningResult.NotFound)
			{
				return node;
			}
			V_0 = node.get_MethodExpression().get_Method();
			V_1 = 0;
			while (V_1 < node.get_Arguments().get_Count())
			{
				if (V_0.get_Parameters().get_Item(V_1).get_ParameterType().get_IsByReference())
				{
					if (this.valueHasSideEffects && (new SideEffectsFinder()).HasSideEffectsRecursive(node.get_Arguments().get_Item(V_1)))
					{
						this.status = 2;
						return node;
					}
				}
				else
				{
					node.get_Arguments().set_Item(V_1, (Expression)this.Visit(node.get_Arguments().get_Item(V_1)));
					if (this.status != SimpleVariableInliner.InliningResult.NotFound)
					{
						return node;
					}
				}
				V_1 = V_1 + 1;
			}
			return node;
		}
	}
}