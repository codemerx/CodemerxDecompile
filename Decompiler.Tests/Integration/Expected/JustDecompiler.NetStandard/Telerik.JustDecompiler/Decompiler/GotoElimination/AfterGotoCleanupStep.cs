using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Steps;

namespace Telerik.JustDecompiler.Decompiler.GotoElimination
{
	[Obsolete]
	internal class AfterGotoCleanupStep : BaseCodeVisitor, IDecompilationStep
	{
		private readonly List<ExpressionStatement> statementsToRemove;

		private List<IfStatement> emptyThenIfs;

		private TypeSystem typeSystem;

		public AfterGotoCleanupStep()
		{
			base();
			this.statementsToRemove = new List<ExpressionStatement>();
			this.emptyThenIfs = new List<IfStatement>();
			return;
		}

		private void CleanupEmptyIfs(BlockStatement body)
		{
			do
			{
				V_0 = this.emptyThenIfs.GetEnumerator();
				try
				{
					while (V_0.MoveNext())
					{
						V_1 = V_0.get_Current();
						if (V_1.get_Else() == null || V_1.get_Else().get_Statements().get_Count() == 0)
						{
							dummyVar0 = (V_1.get_Parent() as BlockStatement).get_Statements().Remove(V_1);
						}
						else
						{
							V_1.set_Then(V_1.get_Else());
							V_1.set_Else(null);
							dummyVar1 = Negator.Negate(V_1.get_Condition(), this.typeSystem);
						}
					}
				}
				finally
				{
					((IDisposable)V_0).Dispose();
				}
				this.emptyThenIfs = new List<IfStatement>();
				this.Visit(body);
			}
			while (this.emptyThenIfs.get_Count() != 0);
			return;
		}

		private void CleanupRedundantAssignments()
		{
			V_0 = this.statementsToRemove.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					dummyVar0 = (V_1.get_Parent() as BlockStatement).get_Statements().Remove(V_1);
				}
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
			return;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.typeSystem = context.get_MethodContext().get_Method().get_Module().get_TypeSystem();
			this.Visit(body);
			this.CleanupRedundantAssignments();
			this.CleanupEmptyIfs(body);
			return body;
		}

		public override void VisitExpressionStatement(ExpressionStatement node)
		{
			if (node.get_Expression() as BinaryExpression != null)
			{
				V_0 = node.get_Expression() as BinaryExpression;
				if (V_0.get_Operator() == 26 && V_0.get_Left() as VariableReferenceExpression != null && V_0.get_Right() as VariableReferenceExpression != null)
				{
					stackVariable18 = (V_0.get_Left() as VariableReferenceExpression).get_Variable();
					V_1 = (V_0.get_Right() as VariableReferenceExpression).get_Variable();
					if ((object)stackVariable18 == (object)V_1)
					{
						this.statementsToRemove.Add(node);
					}
				}
			}
			return;
		}

		public override void VisitIfStatement(IfStatement node)
		{
			if (node.get_Then().get_Statements().get_Count() == 0)
			{
				this.emptyThenIfs.Add(node);
			}
			this.VisitIfStatement(node);
			return;
		}
	}
}