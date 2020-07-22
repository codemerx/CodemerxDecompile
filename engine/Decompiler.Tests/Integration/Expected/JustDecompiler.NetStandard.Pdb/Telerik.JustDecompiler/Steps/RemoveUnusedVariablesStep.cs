using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class RemoveUnusedVariablesStep : BaseCodeVisitor, IDecompilationStep
	{
		protected DecompilationContext context;

		private readonly Dictionary<VariableReference, ExpressionStatement> referenceToDeclarationStatementMap;

		private BlockStatement theBody;

		private readonly HashSet<VariableReference> bannedVariables;

		public RemoveUnusedVariablesStep()
		{
			this.referenceToDeclarationStatementMap = new Dictionary<VariableReference, ExpressionStatement>();
			this.bannedVariables = new HashSet<VariableReference>();
			base();
			return;
		}

		protected virtual bool CanExistInStatement(Expression expression)
		{
			if (expression.get_CodeNodeType() == 19 || expression.get_CodeNodeType() == 51 || expression.get_CodeNodeType() == 65)
			{
				return true;
			}
			V_0 = null;
			if (expression.get_CodeNodeType() != 87)
			{
				if (expression.get_CodeNodeType() == 24)
				{
					V_0 = expression as BinaryExpression;
				}
			}
			else
			{
				V_1 = expression as ParenthesesExpression;
				if (V_1.get_Expression().get_CodeNodeType() == 24)
				{
					V_0 = V_1.get_Expression() as BinaryExpression;
				}
			}
			if (V_0 != null && V_0.get_IsAssignmentExpression() || V_0.get_IsSelfAssign())
			{
				return true;
			}
			return false;
		}

		public void CleanUpUnusedDeclarations()
		{
			V_1 = this.referenceToDeclarationStatementMap.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_3 = V_2.get_Key();
					this.context.get_MethodContext().RemoveVariable(V_3);
					V_4 = V_2.get_Value();
					V_5 = V_4.get_Parent() as BlockStatement;
					if (!this.IsOptimisableAssignment(V_4))
					{
						V_6 = (V_4.get_Expression() as BinaryExpression).get_Right();
						if (!this.CanExistInStatement(V_6))
						{
							continue;
						}
						if (V_6.get_CodeNodeType() == 87)
						{
							V_6 = (V_6 as ParenthesesExpression).get_Expression();
						}
						V_7 = new ExpressionStatement(V_6);
						V_8 = V_5.get_Statements().IndexOf(V_4);
						V_5.AddStatementAt(V_8 + 1, V_7);
						this.TransferLabel(V_4);
						V_5.get_Statements().RemoveAt(V_8);
					}
					else
					{
						this.TransferLabel(V_4);
						dummyVar0 = V_5.get_Statements().Remove(V_4);
					}
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			V_0 = new HashSet<VariableDefinition>();
			V_9 = this.context.get_MethodContext().get_Variables().GetEnumerator();
			try
			{
				while (V_9.MoveNext())
				{
					V_10 = V_9.get_Current();
					if (this.bannedVariables.Contains(V_10))
					{
						continue;
					}
					dummyVar1 = V_0.Add(V_10);
				}
			}
			finally
			{
				V_9.Dispose();
			}
			V_11 = V_0.GetEnumerator();
			try
			{
				while (V_11.MoveNext())
				{
					V_12 = V_11.get_Current();
					this.context.get_MethodContext().RemoveVariable(V_12);
				}
			}
			finally
			{
				((IDisposable)V_11).Dispose();
			}
			return;
		}

		private bool IsLoopBody(BlockStatement blockStatement)
		{
			V_0 = blockStatement.get_Parent();
			if (V_0 == null)
			{
				return false;
			}
			if (V_0.get_CodeNodeType() == 8 || V_0.get_CodeNodeType() == 7 || V_0.get_CodeNodeType() == 12)
			{
				return true;
			}
			return V_0.get_CodeNodeType() == 11;
		}

		public bool IsOptimisableAssignment(ExpressionStatement statement)
		{
			V_0 = statement.get_Expression() as BinaryExpression;
			if (V_0 == null)
			{
				return false;
			}
			if (!V_0.get_IsAssignmentExpression())
			{
				return false;
			}
			if (V_0.get_Right().get_CodeNodeType() == 24 && (V_0.get_Right() as BinaryExpression).get_IsAssignmentExpression() || (V_0.get_Right() as BinaryExpression).get_IsSelfAssign())
			{
				return false;
			}
			if (V_0.get_Left() as VariableReferenceExpression == null && V_0.get_Left() as VariableDeclarationExpression == null)
			{
				return false;
			}
			return !(new SideEffectsFinder()).HasSideEffectsRecursive(V_0.get_Right());
		}

		private void MoveLabel(Statement destination, string theLabel)
		{
			if (String.op_Equality(destination.get_Label(), String.Empty))
			{
				destination.set_Label(theLabel);
				this.context.get_MethodContext().get_GotoLabels().set_Item(theLabel, destination);
				return;
			}
			V_0 = destination.get_Label();
			V_1 = this.context.get_MethodContext().get_GotoStatements().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (!String.op_Equality(V_2.get_TargetLabel(), theLabel))
					{
						continue;
					}
					V_2.set_TargetLabel(V_0);
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			dummyVar0 = this.context.get_MethodContext().get_GotoLabels().Remove(theLabel);
			return;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			this.theBody = body;
			this.Visit(body);
			this.CleanUpUnusedDeclarations();
			return body;
		}

		private void TransferLabel(ExpressionStatement expressionStatement)
		{
			if (String.op_Equality(expressionStatement.get_Label(), String.Empty))
			{
				return;
			}
			V_0 = expressionStatement.get_Label();
			V_1 = expressionStatement;
			while (V_1.get_Parent() != null)
			{
				V_3 = V_1.get_Parent() as BlockStatement;
				V_4 = V_3.get_Statements().IndexOf(V_1);
				if (V_4 != V_3.get_Statements().get_Count() - 1)
				{
					V_5 = V_3.get_Statements().get_Item(V_4 + 1);
					this.MoveLabel(V_5, V_0);
					return;
				}
				if (this.IsLoopBody(V_3))
				{
					this.MoveLabel(V_3.get_Statements().get_Item(0), V_0);
					return;
				}
				do
				{
					V_1 = V_1.get_Parent();
				}
				while (V_1.get_Parent() != null && V_1.get_Parent().get_CodeNodeType() != CodeNodeType.BlockStatement);
			}
			V_2 = new EmptyStatement();
			this.theBody.get_Statements().Add(V_2);
			this.MoveLabel(V_2, V_0);
			return;
		}

		public override void VisitDelegateCreationExpression(DelegateCreationExpression node)
		{
			if (node.get_MethodExpression().get_CodeNodeType() != 50)
			{
				this.VisitDelegateCreationExpression(node);
				return;
			}
			this.VisitLambdaExpression((LambdaExpression)node.get_MethodExpression());
			return;
		}

		public override void VisitExpressionStatement(ExpressionStatement node)
		{
			V_0 = null;
			if (node.get_Expression().get_CodeNodeType() == 24 && (node.get_Expression() as BinaryExpression).get_IsAssignmentExpression())
			{
				V_1 = (node.get_Expression() as BinaryExpression).get_Left();
				if (V_1.get_CodeNodeType() != 26)
				{
					if (V_1.get_CodeNodeType() == 27)
					{
						V_0 = (V_1 as VariableDeclarationExpression).get_Variable();
					}
				}
				else
				{
					V_0 = (V_1 as VariableReferenceExpression).get_Variable();
				}
			}
			if (V_0 == null || node.get_Parent().get_CodeNodeType() != CodeNodeType.BlockStatement || this.bannedVariables.Contains(V_0))
			{
				this.Visit(node.get_Expression());
				return;
			}
			if (this.referenceToDeclarationStatementMap.Remove(V_0))
			{
				dummyVar0 = this.bannedVariables.Add(V_0);
			}
			else
			{
				this.referenceToDeclarationStatementMap.set_Item(V_0, node);
			}
			this.Visit((node.get_Expression() as BinaryExpression).get_Right());
			return;
		}

		public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			dummyVar0 = this.referenceToDeclarationStatementMap.Remove(node.get_Variable());
			dummyVar1 = this.bannedVariables.Add(node.get_Variable());
			return;
		}
	}
}