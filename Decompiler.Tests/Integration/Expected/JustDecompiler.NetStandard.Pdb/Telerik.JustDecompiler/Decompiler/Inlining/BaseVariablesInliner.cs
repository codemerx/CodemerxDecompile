using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Decompiler.Inlining
{
	internal abstract class BaseVariablesInliner
	{
		protected readonly HashSet<VariableDefinition> variablesToNotInline;

		protected readonly HashSet<VariableDefinition> variablesToInline;

		protected readonly MethodSpecificContext methodContext;

		protected readonly IVariableInliner inliner;

		private IVariablesToNotInlineFinder finder;

		public BaseVariablesInliner(MethodSpecificContext methodContext, IVariableInliner inliner, IVariablesToNotInlineFinder finder)
		{
			this.variablesToNotInline = new HashSet<VariableDefinition>();
			this.variablesToInline = new HashSet<VariableDefinition>();
			base();
			this.methodContext = methodContext;
			this.inliner = inliner;
			this.finder = finder;
			return;
		}

		protected void FastRemoveExpressions(IList<Expression> blockExpressions, bool[] isInlined)
		{
			V_0 = 0;
			V_1 = 0;
			while (V_1 < blockExpressions.get_Count())
			{
				if (!isInlined[V_1])
				{
					stackVariable12 = V_0;
					V_0 = stackVariable12 + 1;
					blockExpressions.set_Item(stackVariable12, blockExpressions.get_Item(V_1));
				}
				V_1 = V_1 + 1;
			}
			while (V_0 < V_1)
			{
				stackVariable23 = V_1 - 1;
				V_1 = stackVariable23;
				blockExpressions.RemoveAt(stackVariable23);
			}
			return;
		}

		protected abstract void FindSingleDefineSingleUseVariables();

		private void FindVariablesToNotInline()
		{
			this.variablesToNotInline.UnionWith(this.finder.Find(this.methodContext.get_Expressions().get_BlockExpressions()));
			return;
		}

		protected abstract void InlineInBlocks();

		public void InlineVariables()
		{
			this.FindVariablesToNotInline();
			this.FindSingleDefineSingleUseVariables();
			this.InlineInBlocks();
			return;
		}
	}
}