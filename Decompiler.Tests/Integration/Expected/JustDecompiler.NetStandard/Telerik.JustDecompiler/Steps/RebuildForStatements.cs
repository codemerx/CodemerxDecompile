using Mono.Cecil.Cil;
using System;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class RebuildForStatements : BaseCodeVisitor, IDecompilationStep
	{
		public RebuildForStatements()
		{
			base();
			return;
		}

		private bool CheckTheInitializer(ExpressionStatement statement, out VariableReference forVariable)
		{
			forVariable = null;
			if (statement == null || !statement.IsAssignmentStatement())
			{
				return false;
			}
			return this.TryGetAssignedVariable(statement, out forVariable);
		}

		protected virtual bool CheckTheLoop(WhileStatement theWhile, VariableReference forVariable)
		{
			if (theWhile == null || theWhile.get_Body().get_Statements().get_Count() < 2)
			{
				return false;
			}
			if (!(new VariableFinder(forVariable)).FindVariable(theWhile.get_Condition()))
			{
				return false;
			}
			V_0 = theWhile.get_Body().get_Statements().get_Item(theWhile.get_Body().get_Statements().get_Count() - 1) as ExpressionStatement;
			if (V_0 == null || !this.TryGetAssignedVariable(V_0, out V_1) || (object)forVariable != (object)V_1)
			{
				return false;
			}
			return !(new RebuildForStatements.ContinueFinder()).FindContinue(theWhile.get_Body());
		}

		private ForStatement CreateForStatement(Statement initializer, WhileStatement theWhile)
		{
			V_0 = theWhile.get_Body().get_Statements().get_Count() - 1;
			V_1 = theWhile.get_Body().get_Statements().get_Item(V_0).get_Label();
			V_2 = new ForStatement((initializer as ExpressionStatement).get_Expression(), theWhile.get_Condition(), (theWhile.get_Body().get_Statements().get_Item(V_0) as ExpressionStatement).get_Expression(), new BlockStatement());
			V_3 = 0;
			while (V_3 < V_0)
			{
				V_2.get_Body().AddStatement(theWhile.get_Body().get_Statements().get_Item(V_3));
				V_3 = V_3 + 1;
			}
			if (!String.IsNullOrEmpty(V_1))
			{
				stackVariable42 = new EmptyStatement();
				stackVariable42.set_Label(V_1);
				V_2.get_Body().AddStatement(stackVariable42);
			}
			return V_2;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.Visit(body);
			return body;
		}

		protected bool TryGetAssignedVariable(ExpressionStatement node, out VariableReference variable)
		{
			variable = null;
			V_1 = node.get_Expression() as BinaryExpression;
			if (V_1 != null && !V_1.get_IsAssignmentExpression())
			{
				V_1 = null;
			}
			if (V_1 != null)
			{
				V_0 = V_1.get_Left();
			}
			else
			{
				V_2 = node.get_Expression() as UnaryExpression;
				if (V_2 == null || V_2.get_Operator() != 3 && V_2.get_Operator() != 4 && V_2.get_Operator() != 5 && V_2.get_Operator() != 6)
				{
					return false;
				}
				V_0 = V_2.get_Operand();
			}
			if (V_0.get_CodeNodeType() != 27)
			{
				if (V_0.get_CodeNodeType() == 26)
				{
					variable = ((VariableReferenceExpression)V_0).get_Variable();
				}
			}
			else
			{
				variable = ((VariableDeclarationExpression)V_0).get_Variable();
			}
			return (object)variable != (object)null;
		}

		public override void VisitBlockStatement(BlockStatement node)
		{
			V_0 = 0;
			while (V_0 < node.get_Statements().get_Count() - 1)
			{
				V_2 = node.get_Statements().get_Item(V_0) as ExpressionStatement;
				V_3 = node.get_Statements().get_Item(V_0 + 1) as WhileStatement;
				if (this.CheckTheInitializer(V_2, out V_1) && this.CheckTheLoop(V_3, V_1))
				{
					V_4 = this.CreateForStatement(V_2, V_3);
					V_4.set_Parent(node);
					node.get_Statements().set_Item(V_0, V_4);
					node.get_Statements().RemoveAt(V_0 + 1);
				}
				V_0 = V_0 + 1;
			}
			this.VisitBlockStatement(node);
			return;
		}

		private class ContinueFinder : BaseCodeVisitor
		{
			private bool found;

			public ContinueFinder()
			{
				base();
				return;
			}

			public bool FindContinue(ICodeNode node)
			{
				this.found = false;
				this.Visit(node);
				return this.found;
			}

			public override void Visit(ICodeNode node)
			{
				if (!this.found)
				{
					this.Visit(node);
				}
				return;
			}

			public override void VisitContinueStatement(ContinueStatement node)
			{
				this.found = true;
				return;
			}

			public override void VisitDoWhileStatement(DoWhileStatement node)
			{
				return;
			}

			public override void VisitForEachStatement(ForEachStatement node)
			{
				return;
			}

			public override void VisitForStatement(ForStatement node)
			{
				return;
			}

			public override void VisitWhileStatement(WhileStatement node)
			{
				return;
			}
		}
	}
}