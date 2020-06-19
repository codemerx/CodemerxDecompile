#region license
//
//	(C) 2007 - 2008 Novell, Inc. http://www.novell.com
//	(C) 2007 - 2008 Jb Evain http://evain.net
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Common;

namespace Telerik.JustDecompiler.Steps
{
	class RebuildForStatements : BaseCodeVisitor, IDecompilationStep
	{
        public BlockStatement Process(DecompilationContext context, BlockStatement body)
        {
            Visit(body);
            return body;
        }

		public override void VisitBlockStatement(BlockStatement node)
		{
            for (int i = 0; i < node.Statements.Count - 1; i++)
            {
                VariableReference forVariable;
                ExpressionStatement initializer = node.Statements[i] as ExpressionStatement;
                WhileStatement theWhile = node.Statements[i + 1] as WhileStatement;

                if (CheckTheInitializer(initializer, out forVariable) &&
                    CheckTheLoop(theWhile, forVariable))
                {
                    ForStatement theForStatement = CreateForStatement(initializer, theWhile);
                    theForStatement.Parent = node;

                    node.Statements[i] = theForStatement;
                    node.Statements.RemoveAt(i + 1);
                }
            }

            base.VisitBlockStatement(node);
		}

        private bool CheckTheInitializer(ExpressionStatement statement, out VariableReference forVariable)
        {
            forVariable = null;
            return statement != null && statement.IsAssignmentStatement() && TryGetAssignedVariable(statement, out forVariable);
        }

        protected virtual bool CheckTheLoop(WhileStatement theWhile, VariableReference forVariable)
        {
            if (theWhile == null || theWhile.Body.Statements.Count < 2)
            {
                return false;
            }

            VariableFinder variableFinder = new VariableFinder(forVariable);
            if (!variableFinder.FindVariable(theWhile.Condition))
            {
                return false;
            }

            ExpressionStatement incrementCandidate = theWhile.Body.Statements[theWhile.Body.Statements.Count - 1] as ExpressionStatement;
            VariableReference incrementVariable;
            if (incrementCandidate == null || !TryGetAssignedVariable(incrementCandidate, out incrementVariable) || forVariable != incrementVariable)
            {
                return false;
            }

            ContinueFinder continueFinder = new ContinueFinder();
            return !continueFinder.FindContinue(theWhile.Body);
        }

        private ForStatement CreateForStatement(Statement initializer, WhileStatement theWhile)
        {
            int forStatementsCount = theWhile.Body.Statements.Count - 1;
            string incrementLabel = theWhile.Body.Statements[forStatementsCount].Label;
            ForStatement result = new ForStatement(
                (initializer as ExpressionStatement).Expression,
                theWhile.Condition,
                (theWhile.Body.Statements[forStatementsCount] as ExpressionStatement).Expression,
                new BlockStatement());

            for (int i = 0; i < forStatementsCount; i++)
            {
                result.Body.AddStatement(theWhile.Body.Statements[i]);
            }

            if (!string.IsNullOrEmpty(incrementLabel))
            {
                EmptyStatement emptyStatement = new EmptyStatement() { Label = incrementLabel };
                result.Body.AddStatement(emptyStatement);
            }

            return result;
        }

        protected bool TryGetAssignedVariable(ExpressionStatement node, out VariableReference variable)
        {
            variable = null;

            Expression variableExpression;
            BinaryExpression assign = node.Expression as BinaryExpression;
            if (assign != null && !assign.IsAssignmentExpression)
            {
                assign = null;
            }

            if (assign == null)
            {
                UnaryExpression unaryExpression = node.Expression as UnaryExpression;
                if (unaryExpression == null ||
                        unaryExpression.Operator != UnaryOperator.PostDecrement &&
                        unaryExpression.Operator != UnaryOperator.PostIncrement &&
                        unaryExpression.Operator != UnaryOperator.PreDecrement &&
                        unaryExpression.Operator != UnaryOperator.PreIncrement)
                {
                    return false;
                }

                variableExpression = unaryExpression.Operand;
            }
            else
            {
                variableExpression = assign.Left;
            }

            if (variableExpression.CodeNodeType == CodeNodeType.VariableDeclarationExpression)
            {
                variable = ((VariableDeclarationExpression)variableExpression).Variable;
            }
            else if (variableExpression.CodeNodeType == CodeNodeType.VariableReferenceExpression)
            {
                variable = ((VariableReferenceExpression)variableExpression).Variable;
            }

            return variable != null;
        }

        private class ContinueFinder : BaseCodeVisitor
        {
            private bool found;

            public bool FindContinue(ICodeNode node)
            {
                found = false;
                Visit(node);
                return found;
            }

            public override void Visit(ICodeNode node)
            {
                if (!found)
                {
                    base.Visit(node);
                }
            }

            public override void VisitContinueStatement(ContinueStatement node)
            {
                found = true;
            }

            public override void VisitDoWhileStatement(DoWhileStatement node)
            {
                return;
            }

            public override void VisitForEachStatement(ForEachStatement node)
            {
                return;
            }

            public override void VisitWhileStatement(WhileStatement node)
            {
                return;
            }

            public override void VisitForStatement(ForStatement node)
            {
                return;
            }
        }
	}
}