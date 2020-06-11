using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.JustDecompiler.Ast;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Decompiler.Inlining
{
    class MethodVariablesInliner : BaseVariablesInliner
    {
        protected override void FindSingleDefineSingleUseVariables()
        {
            SingleDefineSingleUseFinder finder = new SingleDefineSingleUseFinder(this.variablesToNotInline);
            foreach (IList<Expression> blockExpressions in methodContext.Expressions.BlockExpressions.Values)
            {
                finder.VisitExpressionsInBlock(blockExpressions);
            }

            variablesToInline.UnionWith(finder.SingleDefineSingleUsageVariables);
        }

        public MethodVariablesInliner(MethodSpecificContext methodContext, IVariablesToNotInlineFinder finder)
            : base(methodContext, new RestrictedVariableInliner(methodContext.Method.Module.TypeSystem), finder)
        {
        }

        private bool IsEnumeratorGetCurrent(Expression expression)
        {
            if (expression.CodeNodeType == CodeNodeType.ExplicitCastExpression)
            {
                expression = (expression as ExplicitCastExpression).Expression;
            }

            return expression.CodeNodeType == CodeNodeType.MethodInvocationExpression && (expression as MethodInvocationExpression).MethodExpression.Method.Name == "get_Current";
        }

        private bool IsQueryInvocation(Expression expression)
        {
            MethodInvocationExpression methodInvoke = expression as MethodInvocationExpression;
            if (methodInvoke == null || methodInvoke.MethodExpression == null)
            {
                return false;
            }

            //resolve only if needed
            return methodInvoke.MethodExpression.Method.DeclaringType.FullName == "System.Linq.Enumerable" && methodInvoke.MethodExpression.MethodDefinition.IsQueryMethod();
        }

        protected override void InlineInBlocks()
        {
            foreach (KeyValuePair<int, IList<Expression>> offsetToExpressionsPair in methodContext.Expressions.BlockExpressions)
            {
                IList<Expression> blockExpressions = offsetToExpressionsPair.Value;
                bool[] isInlined = new bool[blockExpressions.Count];
                bool isConditionalBlock = methodContext.ControlFlowGraph.InstructionToBlockMapping[offsetToExpressionsPair.Key].Successors.Length > 1;
                for (int i = blockExpressions.Count - 2, j = i + 1; i >= 0; i--)
                {
                    BinaryExpression binaryExpression = blockExpressions[i] as BinaryExpression;
                    if (binaryExpression == null || !binaryExpression.IsAssignmentExpression || binaryExpression.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression)
                    {
                        j = i;
                        continue;
                    }

                    VariableDefinition varDef = (binaryExpression.Left as VariableReferenceExpression).Variable.Resolve();
                    if (!variablesToInline.Contains(varDef))
                    {
                        j = i;
                        continue;
                    }

                    Expression value = binaryExpression.Right;
                    if (IsEnumeratorGetCurrent(value) || IsQueryInvocation(value) || varDef.VariableType != null && varDef.VariableType.IsPinned)
                    {
                        j = i;
                        continue;
                    }

                    List<Instruction> instructions = new List<Instruction>(binaryExpression.MappedInstructions);
                    instructions.AddRange(binaryExpression.Left.UnderlyingSameMethodInstructions);

                    ICodeNode result;
                    if (inliner.TryInlineVariable(varDef, value.CloneAndAttachInstructions(instructions), blockExpressions[j],
                        isConditionalBlock && j + 1 == blockExpressions.Count, out result))
                    {
                        blockExpressions[j] = (Expression)result;
                        isInlined[i] = true;
                    }
                    else
                    {
                        j = i;
                    }
                }

                FastRemoveExpressions(blockExpressions, isInlined);
            }
        }

        private class SingleDefineSingleUseFinder : BaseCodeVisitor
        {
            private readonly HashSet<VariableDefinition> singleDefinitionVariables = new HashSet<VariableDefinition>();
            private readonly HashSet<VariableDefinition> singleUsageVariables = new HashSet<VariableDefinition>();
            private readonly HashSet<VariableDefinition> bannedVariables = new HashSet<VariableDefinition>();

            public SingleDefineSingleUseFinder(HashSet<VariableDefinition> variablesToNotInline)
            {
                this.bannedVariables.UnionWith(variablesToNotInline);
            }

            public HashSet<VariableDefinition> SingleDefineSingleUsageVariables
            {
                get
                {
                    return singleUsageVariables;
                }
            }

            public void VisitExpressionsInBlock(IList<Expression> expressions)
            {
                singleDefinitionVariables.Clear();
                foreach (Expression expression in expressions)
                {
                    Visit(expression);
                }
            }

            public override void VisitBinaryExpression(BinaryExpression node)
            {
                if (node.IsAssignmentExpression && node.Left.CodeNodeType == CodeNodeType.VariableReferenceExpression)
                {
                    Visit(node.Right);
                    AddDefinition((node.Left as VariableReferenceExpression).Variable.Resolve());
                }
                else
                {
                    base.VisitBinaryExpression(node);
                }
            }

            public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
            {
                VariableDefinition varDef = node.Variable.Resolve();
                if (!bannedVariables.Contains(varDef))
                {
                    if (singleDefinitionVariables.Remove(varDef))
                    {
                        singleUsageVariables.Add(varDef);
                    }
                    else
                    {
                        singleUsageVariables.Remove(varDef);
                        bannedVariables.Add(varDef);
                    }
                }
            }

            public override void VisitUnaryExpression(UnaryExpression node)
            {
                if ((node.Operator == UnaryOperator.AddressOf || node.Operator == UnaryOperator.AddressReference) && node.Operand.CodeNodeType == CodeNodeType.VariableReferenceExpression)
                {
                    VariableDefinition variableDefinition = (node.Operand as VariableReferenceExpression).Variable.Resolve();
                    if (bannedVariables.Add(variableDefinition))
                    {
                        singleDefinitionVariables.Remove(variableDefinition);
                        singleUsageVariables.Remove(variableDefinition);
                    }
                    return;
                }
                else if (node.Operator == UnaryOperator.AddressDereference && node.Operand.CodeNodeType == CodeNodeType.UnaryExpression)
                {
                    UnaryExpression unaryOperand = node.Operand as UnaryExpression;
                    if (unaryOperand.Operator == UnaryOperator.AddressOf || unaryOperand.Operator == UnaryOperator.AddressReference)
                    {
                        base.Visit(unaryOperand.Operand);
                        return;
                    }
                }
                base.VisitUnaryExpression(node);
            }

            private void AddDefinition(VariableDefinition variable)
            {
                if (!bannedVariables.Contains(variable))
                {
                    singleDefinitionVariables.Add(variable);
                }
            }
        }
    }
}
