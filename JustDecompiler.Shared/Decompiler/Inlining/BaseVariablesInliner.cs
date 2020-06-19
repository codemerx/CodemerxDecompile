using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler.Inlining
{
    abstract class BaseVariablesInliner
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

        public void InlineVariables()
        {
            FindVariablesToNotInline();
            FindSingleDefineSingleUseVariables();
            InlineInBlocks();
        }

        protected abstract void FindSingleDefineSingleUseVariables();

        protected abstract void InlineInBlocks();

        protected void FastRemoveExpressions(IList<Expression> blockExpressions, bool[] isInlined)
        {
            int first, second;
            for (first = 0, second = 0; second < blockExpressions.Count; second++)
            {
                if (!isInlined[second])
                {
                    blockExpressions[first++] = blockExpressions[second];
                }
            }

            while (first < second)
            {
                blockExpressions.RemoveAt(--second);
            }
        }

        private void FindVariablesToNotInline()
        {
            this.variablesToNotInline.UnionWith(this.finder.Find(this.methodContext.Expressions.BlockExpressions));
        }
    }
}
