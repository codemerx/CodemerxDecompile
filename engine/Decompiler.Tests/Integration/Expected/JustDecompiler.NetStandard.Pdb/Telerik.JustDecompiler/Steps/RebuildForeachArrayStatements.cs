using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class RebuildForeachArrayStatements : BaseCodeVisitor, IDecompilationStep
	{
		private readonly Stack<VariableDefinition> currentForIndeces;

		private readonly Stack<bool> currentForIndecesUsed;

		private DecompilationContext context;

		public RebuildForeachArrayStatements()
		{
			this.currentForIndeces = new Stack<VariableDefinition>();
			this.currentForIndecesUsed = new Stack<bool>();
			base();
			return;
		}

		private bool CheckForIndexUsages(RebuildForeachArrayStatements.ForeachArrayMatcher matcher)
		{
			this.currentForIndeces.Push(matcher.get_Incrementor());
			this.currentForIndecesUsed.Push(false);
			V_0 = matcher.get_Foreach().get_Body().get_Statements().GetEnumerator();
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
			dummyVar0 = this.currentForIndeces.Pop();
			return this.currentForIndecesUsed.Pop();
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			this.Visit(body);
			this.context = null;
			return body;
		}

		private void ProcessBlock(BlockStatement node)
		{
			V_0 = 0;
			while (V_0 < node.get_Statements().get_Count() - 1)
			{
				V_1 = new RebuildForeachArrayStatements.ForeachArrayMatcher(node.get_Statements().get_Item(V_0), node.get_Statements().get_Item(V_0 + 1), this.context.get_MethodContext());
				if (V_1.Match() && !this.CheckForIndexUsages(V_1))
				{
					this.context.get_MethodContext().RemoveVariable(V_1.get_Incrementor());
					if (V_1.get_CurrentVariable() != null)
					{
						this.context.get_MethodContext().RemoveVariable(V_1.get_CurrentVariable());
					}
					node.get_Statements().RemoveAt(V_0);
					node.get_Statements().RemoveAt(V_0);
					node.AddStatementAt(V_0, V_1.get_Foreach());
					this.ProcessBlock(V_1.get_Foreach().get_Body());
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

		public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			if (this.currentForIndeces.get_Count() > 0 && this.currentForIndeces.Peek() == node.get_Variable() && !this.currentForIndecesUsed.Peek())
			{
				dummyVar0 = this.currentForIndecesUsed.Pop();
				this.currentForIndecesUsed.Push(true);
			}
			this.VisitVariableReferenceExpression(node);
			return;
		}

		private class ForeachArrayMatcher
		{
			private readonly Statement statement;

			private readonly Statement nextStatement;

			private readonly MethodSpecificContext methodContext;

			private ForEachStatement foreach;

			private Expression source;

			private BlockStatement statementBody;

			private IEnumerable<Instruction> foreachConditionInstructions;

			public VariableReference CurrentVariable
			{
				get;
				private set;
			}

			public ForEachStatement Foreach
			{
				get
				{
					stackVariable2 = this.foreach;
					if (stackVariable2 == null)
					{
						dummyVar0 = stackVariable2;
						stackVariable2 = new ForEachStatement(new VariableDeclarationExpression(this.get_CurrentVariable().Resolve(), null), this.source, this.statementBody, this.foreachConditionInstructions, null);
					}
					this.foreach = stackVariable2;
					return this.foreach;
				}
			}

			public VariableDefinition Incrementor
			{
				get;
				private set;
			}

			public ForeachArrayMatcher(Statement statement, Statement nextStatement, MethodSpecificContext methodContext)
			{
				base();
				this.statement = statement;
				this.nextStatement = nextStatement;
				this.methodContext = methodContext;
				return;
			}

			private bool CheckArrayIndexer(Expression expression, VariableReference variableReference, Expression arrayExpression)
			{
				if (expression as ArrayIndexerExpression == null)
				{
					return false;
				}
				V_0 = (ArrayIndexerExpression)expression;
				if (V_0.get_Indices().get_Count() != 1)
				{
					return false;
				}
				if (V_0.get_Indices().get_Item(0) as VariableReferenceExpression == null)
				{
					return false;
				}
				if ((object)((VariableReferenceExpression)V_0.get_Indices().get_Item(0)).get_Variable() != (object)variableReference)
				{
					return false;
				}
				if (!V_0.get_Target().CheckInnerReferenceExpressions(arrayExpression))
				{
					return false;
				}
				return true;
			}

			private VariableReference CheckAssingExpression(Statement statement, VariableReference variableReference, Expression arrayExpression)
			{
				if (statement as ExpressionStatement == null)
				{
					return null;
				}
				V_0 = (ExpressionStatement)statement;
				if (this.CheckArrayIndexer(V_0.get_Expression(), variableReference, arrayExpression))
				{
					V_2 = this.GetArrayElementType(arrayExpression);
					if (V_2 != null)
					{
						V_3 = new VariableDefinition(V_2, this.methodContext.get_Method());
						dummyVar0 = this.methodContext.get_VariablesToRename().Add(V_3);
						return V_3;
					}
				}
				if (V_0.get_Expression().get_CodeNodeType() != 24 || !(V_0.get_Expression() as BinaryExpression).get_IsAssignmentExpression())
				{
					return null;
				}
				V_1 = (BinaryExpression)V_0.get_Expression();
				if (V_1.get_Left() as VariableReferenceExpression == null)
				{
					return null;
				}
				if (!this.CheckArrayIndexer(V_1.get_Right(), variableReference, arrayExpression))
				{
					return null;
				}
				return (V_1.get_Left() as VariableReferenceExpression).get_Variable();
			}

			private bool CheckLiteralExpressionValue(Expression expression, int value)
			{
				if (expression as LiteralExpression == null)
				{
					return false;
				}
				V_0 = (LiteralExpression)expression;
				if (V_0.get_Value() as Int32 != 0 && (Int32)V_0.get_Value() == value)
				{
					return true;
				}
				return false;
			}

			private void CopyWhileBodyStatements(WhileStatement whileStatement)
			{
				this.statementBody = new BlockStatement();
				V_0 = 1;
				while (V_0 < whileStatement.get_Body().get_Statements().get_Count() - 1)
				{
					this.statementBody.AddStatement(whileStatement.get_Body().get_Statements().get_Item(V_0));
					V_0 = V_0 + 1;
				}
				return;
			}

			private TypeReference GetArrayElementType(Expression arrayExpression)
			{
				V_0 = arrayExpression.GetTargetTypeReference();
				if (!V_0.get_IsArray())
				{
					return null;
				}
				return V_0.GetElementType();
			}

			private PropertyReferenceExpression GetPropertyReferenceFromCast(Expression expression)
			{
				if (expression as ExplicitCastExpression == null)
				{
					return null;
				}
				V_0 = (ExplicitCastExpression)expression;
				if (String.op_Inequality(V_0.get_TargetType().get_FullName(), "System.Int32"))
				{
					return null;
				}
				if (V_0.get_Expression() as PropertyReferenceExpression == null)
				{
					return null;
				}
				V_1 = (PropertyReferenceExpression)V_0.get_Expression();
				if (String.op_Inequality(V_1.get_Property().get_FullName(), "Int32.System Length()"))
				{
					return null;
				}
				return V_1;
			}

			private VariableReference GetVariableReference()
			{
				V_0 = (ExpressionStatement)this.statement;
				if (V_0.get_Expression().get_CodeNodeType() != 24 || !(V_0.get_Expression() as BinaryExpression).get_IsAssignmentExpression())
				{
					return null;
				}
				V_1 = (BinaryExpression)V_0.get_Expression();
				if (V_1.get_Left() as VariableReferenceExpression == null)
				{
					return null;
				}
				if (!this.CheckLiteralExpressionValue(V_1.get_Right(), 0))
				{
					return null;
				}
				return ((VariableReferenceExpression)V_1.get_Left()).get_Variable();
			}

			private static bool IsArrayExpression(Expression expression)
			{
				V_0 = expression.GetTargetTypeReference();
				if (V_0 == null)
				{
					return false;
				}
				return V_0.get_IsArray();
			}

			private bool IsIncrementExpression(Statement statement, VariableReference variableReference)
			{
				if (statement as ExpressionStatement == null)
				{
					return false;
				}
				V_0 = (ExpressionStatement)statement;
				if (V_0.get_Expression().get_CodeNodeType() != 24 || !(V_0.get_Expression() as BinaryExpression).get_IsAssignmentExpression())
				{
					return false;
				}
				V_1 = (BinaryExpression)V_0.get_Expression();
				if (V_1.get_Left() as VariableReferenceExpression == null)
				{
					return false;
				}
				if ((object)(V_1.get_Left() as VariableReferenceExpression).get_Variable() != (object)variableReference)
				{
					return false;
				}
				if (V_1.get_Right() as BinaryExpression == null)
				{
					return false;
				}
				V_2 = (BinaryExpression)V_1.get_Right();
				if (V_2.get_Operator() != 1)
				{
					return false;
				}
				if (!this.CheckLiteralExpressionValue(V_2.get_Right(), 1))
				{
					return false;
				}
				if (V_2.get_Left() as VariableReferenceExpression == null)
				{
					return false;
				}
				if ((object)(V_2.get_Left() as VariableReferenceExpression).get_Variable() != (object)variableReference)
				{
					return false;
				}
				return true;
			}

			internal bool Match()
			{
				if (this.nextStatement as WhileStatement == null)
				{
					return false;
				}
				V_0 = (WhileStatement)this.nextStatement;
				if (V_0.get_Body().get_Statements().get_Count() < 2)
				{
					return false;
				}
				if (this.statement as ExpressionStatement == null)
				{
					return false;
				}
				V_1 = this.GetVariableReference();
				if (V_0.get_Condition() as BinaryExpression == null)
				{
					return false;
				}
				V_2 = (BinaryExpression)V_0.get_Condition();
				if (V_2.get_Operator() != 13)
				{
					return false;
				}
				if (V_2.get_Left() as VariableReferenceExpression == null)
				{
					return false;
				}
				if ((object)((VariableReferenceExpression)V_2.get_Left()).get_Variable() != (object)V_1)
				{
					return false;
				}
				V_3 = this.GetPropertyReferenceFromCast(V_2.get_Right());
				if (V_3 == null)
				{
					return false;
				}
				if (!RebuildForeachArrayStatements.ForeachArrayMatcher.IsArrayExpression(V_3.get_Target()))
				{
					return false;
				}
				this.set_CurrentVariable(this.CheckAssingExpression(V_0.get_Body().get_Statements().get_Item(0), V_1, V_3.get_Target()));
				if (this.get_CurrentVariable() == null)
				{
					return false;
				}
				if (!this.IsIncrementExpression(V_0.get_Body().get_Statements().get_Item(V_0.get_Body().get_Statements().get_Count() - 1), V_1))
				{
					return false;
				}
				this.set_Incrementor(V_1.Resolve());
				this.source = V_3.get_Target();
				if (this.get_Incrementor() == null)
				{
					return false;
				}
				this.CopyWhileBodyStatements(V_0);
				this.foreachConditionInstructions = V_0.get_Condition().get_UnderlyingSameMethodInstructions();
				return true;
			}
		}
	}
}