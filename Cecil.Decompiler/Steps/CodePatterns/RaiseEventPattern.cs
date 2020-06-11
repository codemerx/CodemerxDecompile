using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
    class RaiseEventPattern : ICodePattern
    {
        public bool TryMatch(StatementCollection statements, out int startIndex, out Statement result, out int replacedStatementsCount)
        {
            startIndex = 0;
            result = null;
            replacedStatementsCount = 0;

            for (int i = 0; i < statements.Count - 1; i++)
            {
                bool isMatching = TryMatchInternal(statements, i, out result);
                if (isMatching)
                {
                    startIndex = i;
                    replacedStatementsCount = 2;

                    return true;
                }
            }

            return false;
        }

        private bool TryMatchInternal(StatementCollection statements, int startIndex, out Statement result)
        {
            result = null;

            if (startIndex + 1 >= statements.Count)
            {
                return false;
            }

            if (statements[startIndex].CodeNodeType != CodeNodeType.ExpressionStatement ||
                statements[startIndex + 1].CodeNodeType != CodeNodeType.IfStatement)
            {
                return false;
            }

            ExpressionStatement eventVariableAssignmentStatement = (statements[startIndex] as ExpressionStatement);
            if (eventVariableAssignmentStatement.Expression.CodeNodeType != CodeNodeType.BinaryExpression)
            {
                return false;
            }

            BinaryExpression eventVariableAssignmentExpression = eventVariableAssignmentStatement.Expression as BinaryExpression;
            if (eventVariableAssignmentExpression.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression ||
                eventVariableAssignmentExpression.Right.CodeNodeType != CodeNodeType.EventReferenceExpression)
            {
                return false;
            }

            VariableReferenceExpression eventVariableDeclaration = eventVariableAssignmentExpression.Left as VariableReferenceExpression;
            EventReferenceExpression eventReferenceExpression = eventVariableAssignmentExpression.Right as EventReferenceExpression;

            IfStatement theIf = statements[startIndex + 1] as IfStatement;
            if (theIf.Then == null ||
                theIf.Else != null ||
                theIf.Condition.CodeNodeType != CodeNodeType.BinaryExpression)
            {
                return false;
            }

            BinaryExpression condition = theIf.Condition as BinaryExpression;
            if (condition.Left.CodeNodeType != CodeNodeType.VariableReferenceExpression ||
                condition.Right.CodeNodeType != CodeNodeType.LiteralExpression ||
                condition.Operator != BinaryOperator.ValueInequality)
            {
                return false;
            }

            VariableReferenceExpression eventVariableReference = condition.Left as VariableReferenceExpression;
            if (eventVariableDeclaration.Variable != eventVariableReference.Variable)
            {
                return false;
            }

            LiteralExpression theLiteral = condition.Right as LiteralExpression;
            if (theLiteral.Value != null)
            {
                return false;
            }

            StatementCollection thenStatements = theIf.Then.Statements;
            if (thenStatements.Count != 1 ||
                thenStatements[0].CodeNodeType != CodeNodeType.ExpressionStatement)
            {
                return false;
            }

            ExpressionStatement delegateInvocationStatement = thenStatements[0] as ExpressionStatement;
            if (delegateInvocationStatement.Expression.CodeNodeType != CodeNodeType.DelegateInvokeExpression)
            {
                return false;
            }

            DelegateInvokeExpression delegateInvocationExpression = delegateInvocationStatement.Expression as DelegateInvokeExpression;
            if (delegateInvocationExpression.Target == null ||
                delegateInvocationExpression.Target.CodeNodeType != CodeNodeType.VariableReferenceExpression)
            {
                return false;
            }

            VariableReferenceExpression delegateInvocationVariableReferece = delegateInvocationExpression.Target as VariableReferenceExpression;
            if (delegateInvocationVariableReferece.Variable != eventVariableDeclaration.Variable)
            {
                return false;
            }

            List<Instruction> instructions = new List<Instruction>();
            instructions.AddRange(eventVariableAssignmentStatement.UnderlyingSameMethodInstructions);
            instructions.AddRange(condition.UnderlyingSameMethodInstructions);
            instructions.AddRange(delegateInvocationExpression.MappedInstructions);
            instructions.AddRange(delegateInvocationVariableReferece.UnderlyingSameMethodInstructions);

            result = new ExpressionStatement(new RaiseEventExpression(eventReferenceExpression.Event, delegateInvocationExpression.InvokeMethodReference, delegateInvocationExpression.Arguments, instructions));

            return true;
        }
    }
}
