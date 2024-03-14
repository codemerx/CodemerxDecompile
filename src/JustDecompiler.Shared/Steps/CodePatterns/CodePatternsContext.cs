using System;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
    class CodePatternsContext
    {
        public Dictionary<VariableDefinition, ExpressionStatement> VariableToSingleAssignmentMap { get; private set; }
        public Dictionary<VariableDefinition, DefineUseCount> VariableToDefineUseCountContext { get; private set; }

        private CodePatternsContext()
        {
            this.VariableToSingleAssignmentMap = new Dictionary<VariableDefinition, ExpressionStatement>();
            this.VariableToDefineUseCountContext = new Dictionary<VariableDefinition, DefineUseCount>();
        }

        public CodePatternsContext(BlockStatement body) : this()
        {
            VariableDefineUseCounter counter = new VariableDefineUseCounter(this);
            counter.Visit(body);
        }

        public CodePatternsContext(StatementCollection statements) : this()
        {
            VariableDefineUseCounter counter = new VariableDefineUseCounter(this);
            counter.Visit(statements);
        }

        private class VariableDefineUseCounter : BaseCodeVisitor
        {
            private readonly HashSet<VariableDefinition> bannedVariables = new HashSet<VariableDefinition>();
            private readonly CodePatternsContext patternContext;

            public VariableDefineUseCounter(CodePatternsContext patternContext)
            {
                this.patternContext = patternContext;
            }

            public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
            {
                AddUsage(node.Variable.Resolve());
            }

            public override void VisitVariableDeclarationExpression(VariableDeclarationExpression node)
            {
                RemoveVariable(node.Variable.Resolve());
            }

            public override void VisitExpressionStatement(ExpressionStatement node)
            {
                if (node.IsAssignmentStatement() && node.Parent.CodeNodeType == CodeNodeType.BlockStatement)
                {
                    BinaryExpression assignment = node.Expression as BinaryExpression;
                    if (assignment.Left.CodeNodeType == CodeNodeType.VariableReferenceExpression)
                    {
                        Visit(assignment.Right);
                        AddDefinition((assignment.Left as VariableReferenceExpression).Variable.Resolve(), node);
                        return;
                    }
                }
                base.VisitExpressionStatement(node);
            }

            public override void VisitBinaryExpression(Ast.Expressions.BinaryExpression node)
            {
                if (node.IsAssignmentExpression && node.Left.CodeNodeType == CodeNodeType.VariableReferenceExpression)
                {
                    RemoveVariable((node.Left as VariableReferenceExpression).Variable.Resolve());
                }
                base.VisitBinaryExpression(node);
            }

            public override void VisitUnaryExpression(UnaryExpression node)
            {
                if ((node.Operator == UnaryOperator.AddressOf || node.Operator == UnaryOperator.AddressReference) &&
                    node.Operand.CodeNodeType == CodeNodeType.VariableReferenceExpression)
                {
                    RemoveVariable((node.Operand as VariableReferenceExpression).Variable.Resolve());
                }
                else
                {
                    base.VisitUnaryExpression(node);
                }
            }

            private void RemoveVariable(VariableDefinition variable)
            {
                if (bannedVariables.Add(variable))
                {
                    patternContext.VariableToDefineUseCountContext.Remove(variable);
                    patternContext.VariableToSingleAssignmentMap.Remove(variable);
                }
            }

            private void AddDefinition(VariableDefinition variable, ExpressionStatement expressionStatement)
            {
                if (!bannedVariables.Contains(variable))
                {
                    DefineUseCount defineUseCount;
                    if (patternContext.VariableToDefineUseCountContext.TryGetValue(variable, out defineUseCount))
                    {
                        defineUseCount.DefineCount++;
                        patternContext.VariableToSingleAssignmentMap.Remove(variable);
                    }
                    else
                    {
                        DefineUseCount newEntry = new DefineUseCount();
                        newEntry.DefineCount++;
                        patternContext.VariableToDefineUseCountContext.Add(variable, newEntry);
                        patternContext.VariableToSingleAssignmentMap.Add(variable, expressionStatement);
                    }
                }
            }

            private void AddUsage(VariableDefinition variable)
            {
                DefineUseCount defineUseCount;
                if (patternContext.VariableToDefineUseCountContext.TryGetValue(variable, out defineUseCount))
                {
                    defineUseCount.UseCount++;
                }
                else
                {
                    RemoveVariable(variable);
                }
            }
        }
    }
}
