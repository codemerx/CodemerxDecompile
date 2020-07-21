using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Common;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class RebuildUsingStatements : BaseCodeVisitor, IDecompilationStep
	{
		private DecompilationContext context;

		public RebuildUsingStatements()
		{
			base();
			return;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			this.Visit(body);
			return body;
		}

		private void ProcessBlock(BlockStatement node)
		{
			V_0 = 0;
			while (V_0 < node.get_Statements().get_Count() - 1)
			{
				V_1 = new RebuildUsingStatements.UsingMatcher(node.get_Statements().get_Item(V_0), node.get_Statements().get_Item(V_0 + 1));
				if (V_1.Match())
				{
					if (V_1.get_VariableReference() != null)
					{
						this.context.get_MethodContext().RemoveVariable(V_1.get_VariableReference());
					}
					if (!V_1.get_RemoveExpression())
					{
						stackVariable27 = V_0;
						if (V_1.get_HasExpression())
						{
							stackVariable30 = 1;
						}
						else
						{
							stackVariable30 = 0;
						}
						V_2 = stackVariable27 + stackVariable30;
						node.get_Statements().RemoveAt(V_2);
						node.AddStatementAt(V_2, V_1.get_Using());
					}
					else
					{
						node.get_Statements().RemoveAt(V_0);
						node.get_Statements().RemoveAt(V_0);
						node.AddStatementAt(V_0, V_1.get_Using());
					}
					this.ProcessBlock(V_1.get_Using().get_Body());
				}
				V_0 = V_0 + 1;
			}
			return;
		}

		public override void VisitBlockStatement(BlockStatement node)
		{
			this.ProcessBlock(node);
			V_0 = node.get_Statements().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					this.Visit(V_1);
				}
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
			return;
		}

		private class UsingMatcher
		{
			private readonly Statement statement;

			private readonly Statement nextStatement;

			private UsingStatement using;

			private readonly BlockStatement blockStatement;

			private Expression expression;

			private VariableReference variableReference;

			private bool hasExpression;

			private bool removeExpression;

			private TryStatement theTry;

			public bool HasExpression
			{
				get
				{
					return this.hasExpression;
				}
			}

			public bool RemoveExpression
			{
				get
				{
					return this.removeExpression;
				}
			}

			public UsingStatement Using
			{
				get
				{
					stackVariable2 = this.using;
					if (stackVariable2 == null)
					{
						dummyVar0 = stackVariable2;
						stackVariable2 = new UsingStatement(this.expression.CloneExpressionOnly(), this.blockStatement, this.theTry.get_Finally().get_UnderlyingSameMethodInstructions());
					}
					this.using = stackVariable2;
					return this.using;
				}
			}

			public VariableReference VariableReference
			{
				get
				{
					return this.variableReference;
				}
			}

			public UsingMatcher(Statement statement, Statement nextStatement)
			{
				this.blockStatement = new BlockStatement();
				base();
				this.statement = statement;
				this.nextStatement = nextStatement;
				return;
			}

			private bool CheckStandardUsingBlock(IfStatement ifStatement, TryStatement try, ExpressionStatement expressionStatement)
			{
				if (ifStatement.get_Condition().get_CodeNodeType() != 24)
				{
					return false;
				}
				V_0 = (BinaryExpression)ifStatement.get_Condition();
				V_1 = this.GetDisposeMethodReference(ifStatement, V_0);
				if (V_1 == null)
				{
					return false;
				}
				V_2 = V_1.get_Target();
				if (V_2 != null && V_2.get_CodeNodeType() == 31 && String.op_Equality((V_2 as ExplicitCastExpression).get_TargetType().get_FullName(), "System.IDisposable"))
				{
					V_2 = (V_2 as ExplicitCastExpression).get_Expression();
				}
				if (!V_0.get_Left().CheckInnerReferenceExpressions(V_2))
				{
					return false;
				}
				this.PrepareUsingBlock(V_0, try, expressionStatement);
				return true;
			}

			private void FixExpression()
			{
				if (this.variableReference == null)
				{
					return;
				}
				V_0 = new VariableFinder(this.variableReference);
				V_1 = this.expression as BinaryExpression;
				if (V_1.get_Right().IsReferenceExpression() && !V_0.FindVariable(this.theTry.get_Try()))
				{
					V_2 = new List<Instruction>(V_1.get_Left().get_UnderlyingSameMethodInstructions());
					V_2.AddRange(V_1.get_MappedInstructions());
					this.expression = V_1.get_Right().CloneAndAttachInstructions(V_2);
				}
				return;
			}

			private MethodReferenceExpression GetDisposeMethodReference(IfStatement ifStatement, BinaryExpression binaryExpression)
			{
				if (binaryExpression.get_Operator() != 10)
				{
					return null;
				}
				if (!binaryExpression.get_Left().IsReferenceExpression() || !RebuildUsingStatements.UsingMatcher.IsNullLiteralExpression(binaryExpression.get_Right()))
				{
					return null;
				}
				return this.GetDisposeMethodReference(ifStatement);
			}

			private MethodReferenceExpression GetDisposeMethodReference(IfStatement ifStatement)
			{
				if (ifStatement.get_Else() != null)
				{
					return null;
				}
				if (ifStatement.get_Then().get_Statements().get_Count() != 1)
				{
					return null;
				}
				V_0 = ifStatement.get_Then().get_Statements().get_Item(0);
				if (V_0 as ExpressionStatement == null)
				{
					return null;
				}
				return this.GetDisposeMethodReference(V_0);
			}

			private MethodReferenceExpression GetDisposeMethodReference(Statement stmt)
			{
				V_0 = (ExpressionStatement)stmt;
				if (V_0.get_Expression() as MethodInvocationExpression == null)
				{
					return null;
				}
				V_1 = (MethodInvocationExpression)V_0.get_Expression();
				if (V_1.get_Arguments().get_Count() != 0)
				{
					return null;
				}
				if (V_1.get_MethodExpression() == null)
				{
					return null;
				}
				V_2 = V_1.get_MethodExpression();
				V_3 = V_1.get_MethodExpression().get_Method().get_DeclaringType();
				if (V_3 == null)
				{
					return null;
				}
				V_4 = Utilities.GetCorlibTypeReference(Type.GetTypeFromHandle(// 
				// Current member / type: Telerik.JustDecompiler.Ast.Expressions.MethodReferenceExpression Telerik.JustDecompiler.Steps.RebuildUsingStatements/UsingMatcher::GetDisposeMethodReference(Telerik.JustDecompiler.Ast.Statements.Statement)
				// Exception in: Telerik.JustDecompiler.Ast.Expressions.MethodReferenceExpression GetDisposeMethodReference(Telerik.JustDecompiler.Ast.Statements.Statement)
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com


			private static bool IsNullLiteralExpression(Expression expression)
			{
				if (expression as LiteralExpression == null)
				{
					return false;
				}
				return (object)((LiteralExpression)expression).get_Value() == (object)null;
			}

			private bool IsTryFinallyStatement(TryStatement try)
			{
				if (try.get_CatchClauses().get_Count() != 0)
				{
					return false;
				}
				return try.get_Finally() != null;
			}

			internal bool Match()
			{
				V_0 = null;
				this.theTry = null;
				if (this.statement.get_CodeNodeType() != 17)
				{
					if (this.nextStatement.get_CodeNodeType() == 17)
					{
						this.theTry = this.nextStatement as TryStatement;
						V_0 = this.statement as ExpressionStatement;
						this.hasExpression = true;
					}
				}
				else
				{
					this.theTry = this.statement as TryStatement;
				}
				if (this.theTry == null)
				{
					return false;
				}
				if (!this.IsTryFinallyStatement(this.theTry))
				{
					return false;
				}
				if (this.theTry.get_Finally().get_Body().get_Statements().get_Count() != 1)
				{
					return false;
				}
				V_1 = this.theTry.get_Finally().get_Body().get_Statements().get_Item(0);
				if (V_1.get_CodeNodeType() != 3)
				{
					return false;
				}
				if (!this.CheckStandardUsingBlock((IfStatement)V_1, this.theTry, V_0))
				{
					return false;
				}
				this.FixExpression();
				return true;
			}

			private void PrepareUsingBlock(BinaryExpression binaryExpression, TryStatement try, ExpressionStatement expressionStatement)
			{
				this.expression = binaryExpression.get_Left();
				V_0 = 0;
				while (V_0 < try.get_Try().get_Statements().get_Count())
				{
					this.blockStatement.AddStatement(try.get_Try().get_Statements().get_Item(V_0));
					V_0 = V_0 + 1;
				}
				if (expressionStatement != null)
				{
					this.VisitAssignExpression(expressionStatement);
				}
				return;
			}

			private void VisitAssignExpression(ExpressionStatement expressionStatement)
			{
				if (expressionStatement.get_Expression().get_CodeNodeType() != 24 || !(expressionStatement.get_Expression() as BinaryExpression).get_IsAssignmentExpression())
				{
					return;
				}
				V_0 = (BinaryExpression)expressionStatement.get_Expression();
				this.expression = V_0;
				this.VisitVariableReference(V_0);
				return;
			}

			private void VisitVariableReference(BinaryExpression assingExpression)
			{
				if (assingExpression.get_Left() as VariableReferenceExpression != null)
				{
					this.variableReference = ((VariableReferenceExpression)assingExpression.get_Left()).get_Variable();
					this.removeExpression = true;
				}
				return;
			}
		}
	}
}