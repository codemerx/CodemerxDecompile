using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.Inlining;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
    class VariableInliningPattern : CommonPatterns, ICodePattern
    {
        private readonly MethodSpecificContext methodContext;
        private readonly RestrictedVariableInliner inliner;
        private IVariablesToNotInlineFinder finder;
        private SimpleDereferencer dereferencer;

        public VariableInliningPattern(CodePatternsContext patternsContext, MethodSpecificContext methodContext, IVariablesToNotInlineFinder finder)
            : base(patternsContext, methodContext.Method.Module.TypeSystem)
        {
            this.methodContext = methodContext;
            this.inliner = new RestrictedVariableInliner(typeSystem);
            this.finder = finder;
            this.dereferencer = new SimpleDereferencer();
        }

        public bool TryMatch(StatementCollection statements, out int startIndex, out Statement result, out int replacedStatementsCount)
        {
            replacedStatementsCount = 0;
            startIndex = -1;
            result = null;
            bool inlinedSuccessfully = false;

            if(statements.Count == 0)
            {
                return false;
            }

            HashSet<VariableDefinition> markedForRemoval = new HashSet<VariableDefinition>();

            List<int> positionsToInline = GetStatementsToInline(statements);

            for (int i = positionsToInline.Count - 1; i >= 0; i--)
            {
                int index = positionsToInline[i];

                ExpressionStatement defineExpression = statements[index] as ExpressionStatement;
                VariableDefinition variable = ((defineExpression.Expression as BinaryExpression).Left as VariableReferenceExpression).Variable.Resolve();

                if (index == statements.Count - 1 || !string.IsNullOrEmpty(defineExpression.Label))
                {
                    markedForRemoval.Add(variable);
                    continue;
                }

                List<Instruction> instructions = new List<Instruction>(defineExpression.Expression.MappedInstructions);
                instructions.AddRange((defineExpression.Expression as BinaryExpression).Left.UnderlyingSameMethodInstructions);
                Expression value = (defineExpression.Expression as BinaryExpression).Right.CloneAndAttachInstructions(instructions);
                
                ICodeNode resultNode;
                if (inliner.TryInlineVariable(variable, value, statements[index + 1], ShouldInlineAggressively(variable), out resultNode))
                {
                    statements.RemoveAt(index);
                    inlinedSuccessfully = true;
                    markedForRemoval.Add(variable);
                    methodContext.RemoveVariable(variable);
                    // the statement containing the inlined variable is not at index, since we removed the variable declaration statement
                    statements[index] = (Statement)this.dereferencer.Visit(statements[index]);
                }
            }

            foreach (VariableDefinition variable in markedForRemoval)
            {
                patternsContext.VariableToSingleAssignmentMap.Remove(variable);
                patternsContext.VariableToDefineUseCountContext.Remove(variable);
            }

            return inlinedSuccessfully;
        }

        private List<int> GetStatementsToInline(StatementCollection statements)
        {
            List<int> result = new List<int>();
            
            HashSet<VariableDefinition> variablesToNotInline = this.finder.Find(statements);

            BlockStatement parent = (BlockStatement)statements[0].Parent;
            if (parent == null)
            {
                throw new NullReferenceException("parent");
            }

            foreach (KeyValuePair<VariableDefinition, DefineUseCount> pair in patternsContext.VariableToDefineUseCountContext)
            {
                if (pair.Value.DefineCount != 1 || pair.Value.UseCount != 1)
                {
                    continue;
                }

                if (variablesToNotInline.Contains(pair.Key))
                {
                    continue;
                }

                ExpressionStatement defineExpression = patternsContext.VariableToSingleAssignmentMap[pair.Key];
                if (defineExpression.Parent != parent)
                {
                    continue;
                }

                int index = statements.IndexOf(defineExpression);
                if (index == -1)
                {
                    throw new IndexOutOfRangeException("index");
                }

                result.Add(index);
            }

            result.Sort();
            return result;
        }

        protected virtual bool ShouldInlineAggressively(VariableDefinition variable)
        {
            return methodContext.StackData.VariableToDefineUseInfo.ContainsKey(variable);
        }
    }
}
