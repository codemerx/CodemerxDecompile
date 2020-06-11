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
using System.Collections.Generic;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.AssignmentAnalysis;

namespace Telerik.JustDecompiler.Steps
{
    public class DeclareVariablesOnFirstAssignment : BaseCodeTransformer, IDecompilationStep
    {
        MethodSpecificContext methodContext;
        readonly HashSet<VariableDefinition> not_assigned = new HashSet<VariableDefinition>();
        readonly Dictionary<VariableDefinition, StatementDeclaration> variableDeclarations = new Dictionary<VariableDefinition, StatementDeclaration>();
        readonly Stack<Statement> statements = new Stack<Statement>();
        readonly Stack<CodeNodeType> codeNodeTypes = new Stack<CodeNodeType>();
        State state = State.LocateDeclarations;

        public override ICodeNode VisitVariableReferenceExpression(VariableReferenceExpression node)
        {
            VariableDefinition variable = (VariableDefinition)node.Variable;

            if (!TryDiscardVariable(variable))
            {
                return node;
            }

            if (variable.VariableType.IsByReference)
            {
                return new RefVariableDeclarationExpression(variable, node.UnderlyingSameMethodInstructions);
            }
            else
            {
                return new VariableDeclarationExpression(variable, node.UnderlyingSameMethodInstructions);
            }
        }

        public override ICodeNode VisitVariableDeclarationExpression(VariableDeclarationExpression node)
        {
            TryDiscardVariable(node.Variable);

            return node;
        }

        private bool TryDiscardVariable(VariableDefinition variable)
        {
            if (!not_assigned.Contains(variable))
            {
                return GetResultOnFirstOccurrence(variable);
            }

            if (!methodContext.Variables.Contains(variable))
            {
                return false;
            }

            TryAddNewVariableDeclaration(variable);

            return RemoveVariable(variable);
        }

        private void TryAddNewVariableDeclaration(VariableDefinition variable)
        {
            if (state != State.LocateDeclarations)
                return;

            var statement = statements.Peek();
            StatementDeclaration statementDeclaration;
            AssignmentType assignmentType;
            if (variableDeclarations.TryGetValue(variable, out statementDeclaration))
            {
                if (!statementDeclaration.UsedInOtherStatements)
                {
                    statementDeclaration.UsedInOtherStatements = !IsChildOfCurrentStatement(statement);
                }
            }
            else if (!this.methodContext.VariableAssignmentData.TryGetValue(variable, out assignmentType) ||
                assignmentType == AssignmentType.SingleAssignment)
            {
                StatementDeclaration newStatementDeclaration = new StatementDeclaration(statement);
                newStatementDeclaration.UsedInOtherStatements = (codeNodeTypes.Peek() != CodeNodeType.BinaryExpression);
                variableDeclarations.Add(variable, newStatementDeclaration);
            }
        }

        private bool GetResultOnFirstOccurrence(VariableDefinition variable)
        {
            if (state == State.LocateDeclarations)
            {
                StatementDeclaration statementDeclaration;
                if (variableDeclarations.TryGetValue(variable, out statementDeclaration) && !statementDeclaration.UsedInOtherStatements)
                {
                    statementDeclaration.UsedInOtherStatements = !IsChildOfCurrentStatement(statementDeclaration.Statement);
                }
                else
                {
                    if (!methodContext.Variables.Contains(variable) && (codeNodeTypes.Peek() == CodeNodeType.BinaryExpression))
                    {
                        not_assigned.Add(variable);
                        return true;
                    }
                }
            }
            return false;
        }

        public override ICodeNode VisitBinaryExpression(BinaryExpression node)
        {
            if (node.IsAssignmentExpression)
            {
                return VisitAssignExpression(node);
            }
            return base.VisitBinaryExpression(node);
        }

        private ICodeNode VisitAssignExpression(BinaryExpression node)
        {
            bool pushBinary = node.Left.CodeNodeType == CodeNodeType.VariableReferenceExpression ||
                node.Left.CodeNodeType == CodeNodeType.VariableDeclarationExpression;
            if (pushBinary)
            {
                codeNodeTypes.Push(CodeNodeType.BinaryExpression);
            }

            node.Left = (Expression)Visit(node.Left);

            if (pushBinary)
            {
                codeNodeTypes.Pop();
            }

            node.Right = (Expression)Visit(node.Right);
            return node;
        }

        public override ICodeNode VisitForEachStatement(ForEachStatement node)
        {
            codeNodeTypes.Push(CodeNodeType.BinaryExpression);
            node.Variable = (VariableDeclarationExpression)Visit(node.Variable);
            codeNodeTypes.Pop();
            node.Collection = (Telerik.JustDecompiler.Ast.Expressions.Expression)Visit(node.Collection);
            node.Body = (BlockStatement)Visit(node.Body);
            return node;
        }

        public override ICodeNode VisitForStatement(ForStatement node)
        {
            statements.Push(node);
            base.VisitForStatement(node);
            statements.Pop();
            return node;
        }

        private bool IsChildOfCurrentStatement(Statement statement)
        {
            foreach (var childStatement in statements)
            {
                if (childStatement == statement)
                {
                    return true;
                }
            }
            return false;
        }

        private bool RemoveVariable(VariableDefinition variable)
        {
            bool result = ShouldRemoveVariable(variable);
            if (result)
            {
                methodContext.RemoveVariable(variable);
            }
            not_assigned.Remove(variable);
            return result;
        }

        private bool ShouldRemoveVariable(VariableDefinition variable)
        {
            if (state == State.ReplaceDeclarations)
            {
                StatementDeclaration statementDeclaration;
                if (variableDeclarations.TryGetValue(variable, out statementDeclaration))
                {
                    return !statementDeclaration.UsedInOtherStatements;
                }
                return !this.methodContext.VariableAssignmentData.ContainsKey(variable);
            }
            else
            {
                return false;
            }
        }

        public BlockStatement Process(DecompilationContext context, BlockStatement block)
        {
            this.methodContext = context.MethodContext;
            PopulateNotAssigned();
            codeNodeTypes.Push(CodeNodeType.BlockStatement);
            VisitBlockStatement(block);
            codeNodeTypes.Pop();

            BlockStatement result = ReplaceDeclarations(block);
            return result;
        }

        private BlockStatement ReplaceDeclarations(BlockStatement block)
        {
            state = State.ReplaceDeclarations;
            PopulateNotAssigned();
            block = (BlockStatement)VisitBlockStatement(block);
            state = State.LocateDeclarations;
            return block;
        }

        public override ICodeNode VisitBlockStatement(BlockStatement node)
        {
            statements.Push(node);
            ICodeNode newNode = base.VisitBlockStatement(node);
            statements.Pop();
            return newNode;
        }

        void PopulateNotAssigned()
        {
            not_assigned.Clear();
            foreach (VariableDefinition variable in methodContext.Variables)
            {
                if (!methodContext.VariablesToNotDeclare.Contains(variable))
                {
                    not_assigned.Add(variable);
                }
            }
        }

        public override ICodeNode VisitLambdaExpression(LambdaExpression node)
        {
            // Visit of lambdas is needed only when locating declarations.
            if (this.state == State.LocateDeclarations)
            {
                // The following visit of the body of the lambda expression is needed, because variables that are declared in the
                // containing method, may be used in it. The clone is needed, because the lambda expression is already visited
                // and processed, and the only thing we need is to visit the variable references in it.
                Visit((node.CloneExpressionOnly() as LambdaExpression).Body);
            }
            
            return node;
        }

        enum State
        {
            LocateDeclarations,
            ReplaceDeclarations
        }

        class StatementDeclaration
        {
            public StatementDeclaration(Statement statement)
            {
                this.Statement = statement;
            }

            public Statement Statement { get; set; }

            public bool UsedInOtherStatements { get; set; }
        }
    }
}