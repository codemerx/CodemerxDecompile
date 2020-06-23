using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Decompiler.Inlining
{
	internal abstract class BaseVariablesInliner
	{
		protected readonly HashSet<VariableDefinition> variablesToNotInline = new HashSet<VariableDefinition>();

		protected readonly HashSet<VariableDefinition> variablesToInline = new HashSet<VariableDefinition>();

		protected readonly MethodSpecificContext methodContext;

		protected readonly IVariableInliner inliner;

		private IVariablesToNotInlineFinder finder;

		public BaseVariablesInliner(MethodSpecificContext methodContext, IVariableInliner inliner, IVariablesToNotInlineFinder finder)
		{
			this.methodContext = methodContext;
			this.inliner = inliner;
			this.finder = finder;
		}

		protected void FastRemoveExpressions(IList<Expression> blockExpressions, bool[] isInlined)
		{
			int i;
			int num = 0;
			for (i = 0; i < blockExpressions.Count; i++)
			{
				if (!isInlined[i])
				{
					int item = num;
					num = item + 1;
					blockExpressions[item] = blockExpressions[i];
				}
			}
			while (num < i)
			{
				int num1 = i - 1;
				i = num1;
				blockExpressions.RemoveAt(num1);
			}
		}

		protected abstract void FindSingleDefineSingleUseVariables();

		private void FindVariablesToNotInline()
		{
			this.variablesToNotInline.UnionWith(this.finder.Find(this.methodContext.Expressions.BlockExpressions));
		}

		protected abstract void InlineInBlocks();

		public void InlineVariables()
		{
			this.FindVariablesToNotInline();
			this.FindSingleDefineSingleUseVariables();
			this.InlineInBlocks();
		}
	}
}