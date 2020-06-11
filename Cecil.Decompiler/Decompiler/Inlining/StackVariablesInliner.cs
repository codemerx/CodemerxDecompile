using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Decompiler.DefineUseAnalysis;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast;
using Mono.Cecil;

namespace Telerik.JustDecompiler.Decompiler.Inlining
{
    class StackVariablesInliner : BaseVariablesInliner
    {
        private readonly Dictionary<int, Expression> offsetToExpression;
        private readonly HashSet<VariableDefinition> inlinedOnSecondPass = new HashSet<VariableDefinition>();

        public StackVariablesInliner(MethodSpecificContext methodContext, Dictionary<int, Expression> offsetToExpression, IVariablesToNotInlineFinder finder)
            : base(methodContext, new SimpleVariableInliner(methodContext.Method.Module.TypeSystem), finder)
        {
            this.offsetToExpression = offsetToExpression;
        }

        protected override void FindSingleDefineSingleUseVariables()
        {
            foreach (KeyValuePair<VariableDefinition, StackVariableDefineUseInfo> pair in methodContext.StackData.VariableToDefineUseInfo)
            {
                if (pair.Value.DefinedAt.Count == 1 && pair.Value.UsedAt.Count == 1)
                {
                    if (!this.variablesToNotInline.Contains(pair.Key))
                    {
                        variablesToInline.Add(pair.Key);
                    }
                }
            }
        }

        protected override void InlineInBlocks()
        {
            InlineAssignmentInNextExpression();
            InlineAssignmentInSameBlock();
            InlineConstantVariables();
        }

        private void FixContextAfterInlining(VariableDefinition varDef)
        {
            methodContext.StackData.VariableToDefineUseInfo.Remove(varDef);
        }

        private void InlineAssignmentInNextExpression()
        {
            foreach (KeyValuePair<int, IList<Expression>> offsetToExpressionsPair in methodContext.Expressions.BlockExpressions)
            {
                IList<Expression> blockExpressions = offsetToExpressionsPair.Value;
                bool[] isInlined = new bool[blockExpressions.Count];
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
                    ICodeNode result;
                    if (inliner.TryInlineVariable(varDef, value, blockExpressions[j], true, out result))
                    {
                        FixContextAfterInlining(varDef);
                        variablesToInline.Remove(varDef);

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

        private void InlineAssignmentInSameBlock()
        {
            foreach (KeyValuePair<int, IList<Expression>> offsetToExpressionsPair in methodContext.Expressions.BlockExpressions)
            {
                IList<Expression> blockExpressions = offsetToExpressionsPair.Value;
                for (int i = 0; i < blockExpressions.Count - 1; i++)
                {
                    BinaryExpression binaryExpression = blockExpressions[i] as BinaryExpression;
                    if (binaryExpression == null || !binaryExpression.IsAssignmentExpression || binaryExpression.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression)
                    {
                        continue;
                    }

                    VariableDefinition varDef = (binaryExpression.Left as VariableReferenceExpression).Variable.Resolve();
                    if (!variablesToInline.Contains(varDef))
                    {
                        continue;
                    }

                    Expression value = binaryExpression.Right;
                    SideEffectsFinder sideEffectsFinder = new SideEffectsFinder();
                    bool valueHasSideEffects = sideEffectsFinder.HasSideEffectsRecursive(value);
                    VariablesArgumentsAndFieldsFinder variableFinder = new VariablesArgumentsAndFieldsFinder();
                    variableFinder.Visit(value);
                    VariableReferenceFinder referenceFinder = new VariableReferenceFinder(variableFinder.Variables, variableFinder.Parameters);

                    for (int j = i + 1; j < blockExpressions.Count; j++)
                    {
                        ICodeNode result;
                        if (inliner.TryInlineVariable(varDef, value, blockExpressions[j], true, out result))
                        {
                            FixContextAfterInlining(varDef);
                            variablesToInline.Remove(varDef);

                            blockExpressions[j] = (Expression)result;
                            blockExpressions.RemoveAt(i);
                            i -= i > 0 ? 2 : 1;
                            break;
                        }
                        else if (valueHasSideEffects && sideEffectsFinder.HasSideEffectsRecursive(blockExpressions[j]))
                        {
                            break;
                        }
                        else if (referenceFinder.ContainsReference(blockExpressions[j]))
                        {
                            break;
                        }
                        else if (blockExpressions[j].CodeNodeType == CodeNodeType.BinaryExpression && (blockExpressions[j] as BinaryExpression).IsAssignmentExpression)
                        {
                            Expression assigned = (blockExpressions[j] as BinaryExpression).Left;
                            if ((assigned.CodeNodeType == CodeNodeType.ArgumentReferenceExpression &&
                                variableFinder.Parameters.Contains((assigned as ArgumentReferenceExpression).Parameter.Resolve())) ||
                                (assigned.CodeNodeType == CodeNodeType.VariableReferenceExpression &&
                                variableFinder.Variables.Contains((assigned as VariableReferenceExpression).Variable.Resolve())) ||
                                (assigned.CodeNodeType == CodeNodeType.FieldReferenceExpression &&
                                variableFinder.Fields.Contains((assigned as FieldReferenceExpression).Field.Resolve())))
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

        private void InlineConstantVariables()
        {
            Dictionary<Expression, Expression> oldToNewValueMap = new Dictionary<Expression, Expression>();
            foreach (VariableDefinition varDef in variablesToInline)
            {
                StackVariableDefineUseInfo defineUseInfo = methodContext.StackData.VariableToDefineUseInfo[varDef];
                int usageOffset = defineUseInfo.UsedAt.First();
                Expression containingExpression;
                if (!offsetToExpression.TryGetValue(usageOffset, out containingExpression))
                {
                    continue;
                }

                Expression value = offsetToExpression[defineUseInfo.DefinedAt.First()];
                if (!ConstantDeterminator.IsConstantExpression(value))
                {
                    continue;
                }

                ICodeNode result;
                if (inliner.TryInlineVariable(varDef, value, containingExpression, true, out result))
                {
                    inlinedOnSecondPass.Add(varDef);
                    FixContextAfterInlining(varDef);

                    if (containingExpression != result)
                    {
                        oldToNewValueMap.Add(containingExpression, (Expression)result);
                    }
                }
            }

            FixBlockExpressions(oldToNewValueMap);
        }

        private void FixBlockExpressions(Dictionary<Expression, Expression> oldToNewValueMap)
        {
            Expression newValue;

            foreach (KeyValuePair<int, IList<Expression>> offsetToExpressionsPair in methodContext.Expressions.BlockExpressions)
            {
                IList<Expression> blockExpressions = offsetToExpressionsPair.Value;
                for (int i = 0; i < blockExpressions.Count; i++)
                {
                    Expression expression = blockExpressions[i];
                    if (expression.CodeNodeType == CodeNodeType.BinaryExpression)
                    {
                        BinaryExpression binaryExpression = expression as BinaryExpression;
                        if (binaryExpression.IsAssignmentExpression &&
                            binaryExpression.Left.CodeNodeType == CodeNodeType.VariableReferenceExpression &&
                            inlinedOnSecondPass.Contains((binaryExpression.Left as VariableReferenceExpression).Variable.Resolve()))
                        {
                            blockExpressions.RemoveAt(i--);
                            continue;
                        }

                        if (oldToNewValueMap.TryGetValue(binaryExpression.Right, out newValue))
                        {
                            binaryExpression.Right = newValue;
                        }
                    }

                    if (oldToNewValueMap.TryGetValue(blockExpressions[i], out newValue))
                    {
                        blockExpressions[i] = newValue;
                    }
                }
            }
        }

        private class ConstantDeterminator : BaseCodeVisitor
        {
            private bool isConstant;

            public static bool IsConstantExpression(Expression expression)
            {
                ConstantDeterminator determinator = new ConstantDeterminator();
                determinator.isConstant = true;
                determinator.Visit(expression);
                return determinator.isConstant;
            }

            private ConstantDeterminator()
            {
            }

            public override void Visit(ICodeNode node)
            {
                if (!isConstant)
                {
                    return;
                }

                switch (node.CodeNodeType)
                {
                    case CodeNodeType.UnaryExpression:
                        UnaryExpression unaryNode = node as UnaryExpression;
                        switch (unaryNode.Operator)
                        {
                            case UnaryOperator.Negate:
                            case UnaryOperator.LogicalNot:
                            case UnaryOperator.BitwiseNot:
                            case UnaryOperator.UnaryPlus:
                            case UnaryOperator.None:
                                break;
                            case UnaryOperator.AddressDereference:
                                break;
                            case UnaryOperator.AddressReference:
                            case UnaryOperator.AddressOf:
                                return;
                            default:
                                isConstant = false;
                                return;
                        }
                        break;

                    case CodeNodeType.BinaryExpression:
                        BinaryExpression binaryExpression = node as BinaryExpression;
                        isConstant = !binaryExpression.IsChecked && binaryExpression.Operator != BinaryOperator.Divide && binaryExpression.Operator != BinaryOperator.Modulo;
                        if (!isConstant)
                        {
                            return;
                        }
                        break;
                    case CodeNodeType.SafeCastExpression:
                    case CodeNodeType.TypeOfExpression:
                        break;

                    case CodeNodeType.ArgumentReferenceExpression:
                        //TODO: Can be improved
                        isConstant = false;
                        return;

                    case CodeNodeType.ThisReferenceExpression:
                        //TODO: Value types
                        return;

                    case CodeNodeType.BaseReferenceExpression:
                    case CodeNodeType.TypeReferenceExpression:
                    case CodeNodeType.MethodReferenceExpression:
                    case CodeNodeType.LiteralExpression:
                        return;

                    default:
                        isConstant = false;
                        return;
                }

                base.Visit(node);
            }
        }

        private class VariablesArgumentsAndFieldsFinder : BaseCodeVisitor
        {
            public HashSet<VariableDefinition> Variables { get; private set; }
            public HashSet<ParameterDefinition> Parameters { get; private set; }
            public HashSet<FieldDefinition> Fields { get; private set; }

            public VariablesArgumentsAndFieldsFinder()
            {
                this.Variables = new HashSet<VariableDefinition>();
                this.Parameters = new HashSet<ParameterDefinition>();
                this.Fields = new HashSet<FieldDefinition>();
            }

            public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
            {
                this.Variables.Add(node.Variable.Resolve());
            }

            public override void VisitArgumentReferenceExpression(ArgumentReferenceExpression node)
            {
                this.Parameters.Add(node.Parameter.Resolve());
            }

            public override void VisitFieldReferenceExpression(FieldReferenceExpression node)
            {
                this.Fields.Add(node.Field.Resolve());
            }
        }

        private class VariableReferenceFinder : BaseCodeVisitor
        {
            private readonly HashSet<VariableDefinition> variables;
            private readonly HashSet<ParameterDefinition> parameters;
            private bool containsReference;

            public VariableReferenceFinder(HashSet<VariableDefinition> variables, HashSet<ParameterDefinition> parameters)
            {
                this.variables = variables;
                this.parameters = parameters;
            }

            public bool ContainsReference(Expression expression)
            {
                containsReference = false;
                Visit(expression);
                return containsReference;
            }

            public override void Visit(ICodeNode node)
            {
                if (containsReference)
                {
                    return;
                }
                base.Visit(node);
            }

            public override void VisitUnaryExpression(UnaryExpression node)
            {
                if (node.Operator == UnaryOperator.AddressOf || node.Operator == UnaryOperator.AddressReference)
                {
                    if (node.Operand.CodeNodeType == CodeNodeType.VariableReferenceExpression &&
                        variables.Contains((node.Operand as VariableReferenceExpression).Variable.Resolve()) ||
                        node.Operand.CodeNodeType == CodeNodeType.ArgumentReferenceExpression &&
                        parameters.Contains((node.Operand as ArgumentReferenceExpression).Parameter.Resolve()))
                    {
                        containsReference = true;
                        return;
                    }
                }
                base.VisitUnaryExpression(node);
            }
        }
    }
}
