using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Steps;
using Telerik.JustDecompiler.Ast;
using Mono.Cecil.Cil;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Expressions;
using Mono.Cecil;
using System;

namespace Telerik.JustDecompiler.Decompiler.GotoElimination
{
    [Obsolete]
    class AfterGotoCleanupStep : BaseCodeVisitor, IDecompilationStep
    {
        private readonly List<ExpressionStatement> statementsToRemove;
        private List<IfStatement> emptyThenIfs;
        private TypeSystem typeSystem;

        public AfterGotoCleanupStep()
        {
            this.statementsToRemove = new List<ExpressionStatement>();
            this.emptyThenIfs = new List<IfStatement>();
        }

        public BlockStatement Process(DecompilationContext context, BlockStatement body)
        {
            this.typeSystem = context.MethodContext.Method.Module.TypeSystem;

            Visit(body);
            CleanupRedundantAssignments();
            CleanupEmptyIfs(body);
            return body;
        }

        private void CleanupRedundantAssignments()
        {
            foreach (ExpressionStatement statement in statementsToRemove)
            {
                (statement.Parent as BlockStatement).Statements.Remove(statement);
            }
        }
        private void CleanupEmptyIfs(BlockStatement body)
        {
            do
            {
                foreach (IfStatement @if in emptyThenIfs)
                {
                    if (@if.Else == null || @if.Else.Statements.Count == 0)
                    {
                        (@if.Parent as BlockStatement).Statements.Remove(@if);
                    }
                    else
                    {
                        @if.Then = @if.Else;
                        @if.Else = null;
                        Negator.Negate(@if.Condition, typeSystem);
                    }
                }

                emptyThenIfs = new List<IfStatement>();
                Visit(body);

            } while (emptyThenIfs.Count != 0) ;
        }
        public override void VisitExpressionStatement(ExpressionStatement node)
        {
            if (node.Expression is BinaryExpression)
            {
                BinaryExpression binEx = node.Expression as BinaryExpression;
                if (binEx.Operator == BinaryOperator.Assign)
                {
                    if (binEx.Left is VariableReferenceExpression && binEx.Right is VariableReferenceExpression)
                    {
                        VariableReference leftVariable = (binEx.Left as VariableReferenceExpression).Variable;
                        VariableReference rightVariable = (binEx.Right as VariableReferenceExpression).Variable;
                        if (leftVariable == rightVariable)
                        {
                            statementsToRemove.Add(node);
                        }
                    }
                }
            }
        }

        public override void VisitIfStatement(IfStatement node)
        {
            if (node.Then.Statements.Count == 0)
            {
                emptyThenIfs.Add(node);
            }
            base.VisitIfStatement(node);
        }
    }
}
