using System.Collections.Generic;
using Mono.Cecil.Cil;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps
{
    internal class RebuildFixedStatements : BaseCodeVisitor, IDecompilationStep
    {
		MethodSpecificContext methodContext;
        readonly IList<VariableReference> variables = new List<VariableReference>();

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
        {
            this.methodContext = context.MethodContext;

            Visit(body);

            return body;
        }

		public override void VisitBlockStatement (BlockStatement node)
		{
            ProcessBlock (node);

			foreach (Statement statement in node.Statements)
			{
				Visit(statement);
			}
		}

        void ProcessBlock (BlockStatement node)
        {
            for (int i = 0; i < node.Statements.Count - 1; i++)
            {
                FixedStatement @fixed = null;
                List<VariableReference> variableReferences = new List<VariableReference>();

                Statement statement = node.Statements[i];

                @fixed = GetFixedStatement(statement, variableReferences);

                if (@fixed == null)
                {
                    continue;
                }

				foreach (VariableReference variable in variableReferences)
				{
					methodContext.RemoveVariable(variable);
				}
                
                // remove the first statement.
                node.Statements.RemoveAt(i);
                // append the fixed block
                node.AddStatementAt(i, @fixed);

                ExpressionStatement expressionStmt = node.Statements[i + 1] as ExpressionStatement;

                int len = node.Statements.Count;

                VariableReference fixedVariable = null;

                VariableReferenceVisitor varibleVisitor = new VariableReferenceVisitor();
                varibleVisitor.Visit(@fixed.Expression);

                fixedVariable = varibleVisitor.Variable;
               
                for (int stmtIndex = i + 1; stmtIndex < len; stmtIndex++)
                {
                    varibleVisitor = new VariableReferenceVisitor();

                    varibleVisitor.Visit(node.Statements[stmtIndex]);

                    VariableReference variable = varibleVisitor.Variable;

                    if (variable != null && !variables.Contains(variable))
                    {
                        variables.Add(variable);
                    }

                    if (node.Statements[stmtIndex].CodeNodeType == CodeNodeType.ExpressionStatement)
                    {
                        expressionStmt = node.Statements[stmtIndex] as ExpressionStatement;

                        if (variable != null 
                            && variable == fixedVariable
                            && expressionStmt.Expression.CodeNodeType == CodeNodeType.BinaryExpression 
                            && (expressionStmt.Expression as BinaryExpression).IsAssignmentExpression
							&& (expressionStmt.Expression as BinaryExpression).Right.CodeNodeType == CodeNodeType.LiteralExpression
							&& ((expressionStmt.Expression as BinaryExpression).Right as LiteralExpression).Value == null)
                        {
							node.Statements.RemoveAt(stmtIndex);
							stmtIndex--;
							len--;
							break;
						}
                    }
                    @fixed.Body.AddStatement(node.Statements[stmtIndex]);
					node.Statements.RemoveAt(stmtIndex);
					stmtIndex--;
					len--;
                }

				ProcessBlock(@fixed.Body);

                break;
            }
        }
  
        private FixedStatement GetFixedStatement(Statement statement, List<VariableReference> variableReferences)
        {
            if (statement.CodeNodeType == CodeNodeType.ExpressionStatement)
            {
                ExpressionStatement expression = statement as ExpressionStatement;
                if (expression.Expression is BinaryExpression)
                {
                    BinaryExpression binEx = expression.Expression as BinaryExpression;
                    if (binEx.IsAssignmentExpression && binEx.Left.ExpressionType.IsPinned && binEx.Right.CodeNodeType != CodeNodeType.LiteralExpression)
                    {
                        //expression = expression.Expression;
                        variableReferences.Add((binEx.Left as VariableReferenceExpression).Variable);
                        return new FixedStatement(expression.Expression, new BlockStatement());
                    }
                }
            }
            return null;
        }

        internal class VariableReferenceVisitor : BaseCodeVisitor
        {
			public VariableReference Variable { get; private set; }

            public override void VisitExpressionStatement(ExpressionStatement node)
            {
                this.Visit(node.Expression);
            }

            public override void VisitBinaryExpression(BinaryExpression node)
            {
                if (node.IsAssignmentExpression)
                {
					this.Visit(node.Left);
                    return;
                }
                base.VisitBinaryExpression(node);
            }

			public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
			{
				Variable = node.Variable;
			}
        }
    }
}
