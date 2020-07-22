using Mono.Cecil.Cil;
using Mono.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.AssignmentAnalysis;

namespace Telerik.JustDecompiler.Steps
{
	public class DeclareVariablesOnFirstAssignment : BaseCodeTransformer, IDecompilationStep
	{
		private MethodSpecificContext methodContext;

		private readonly HashSet<VariableDefinition> not_assigned;

		private readonly Dictionary<VariableDefinition, DeclareVariablesOnFirstAssignment.StatementDeclaration> variableDeclarations;

		private readonly Stack<Statement> statements;

		private readonly Stack<CodeNodeType> codeNodeTypes;

		private DeclareVariablesOnFirstAssignment.State state;

		public DeclareVariablesOnFirstAssignment()
		{
			this.not_assigned = new HashSet<VariableDefinition>();
			this.variableDeclarations = new Dictionary<VariableDefinition, DeclareVariablesOnFirstAssignment.StatementDeclaration>();
			this.statements = new Stack<Statement>();
			this.codeNodeTypes = new Stack<CodeNodeType>();
			base();
			return;
		}

		private bool GetResultOnFirstOccurrence(VariableDefinition variable)
		{
			if (this.state == DeclareVariablesOnFirstAssignment.State.LocateDeclarations)
			{
				if (!this.variableDeclarations.TryGetValue(variable, out V_0) || V_0.get_UsedInOtherStatements())
				{
					if (!this.methodContext.get_Variables().Contains(variable) && this.codeNodeTypes.Peek() == 24)
					{
						dummyVar0 = this.not_assigned.Add(variable);
						return true;
					}
				}
				else
				{
					V_0.set_UsedInOtherStatements(!this.IsChildOfCurrentStatement(V_0.get_Statement()));
				}
			}
			return false;
		}

		private bool IsChildOfCurrentStatement(Statement statement)
		{
			V_0 = this.statements.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					if (V_0.get_Current() != statement)
					{
						continue;
					}
					V_1 = true;
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
		Label1:
			return V_1;
		Label0:
			return false;
		}

		private void PopulateNotAssigned()
		{
			this.not_assigned.Clear();
			V_0 = this.methodContext.get_Variables().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (this.methodContext.get_VariablesToNotDeclare().Contains(V_1))
					{
						continue;
					}
					dummyVar0 = this.not_assigned.Add(V_1);
				}
			}
			finally
			{
				V_0.Dispose();
			}
			return;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement block)
		{
			this.methodContext = context.get_MethodContext();
			this.PopulateNotAssigned();
			this.codeNodeTypes.Push(0);
			dummyVar0 = this.VisitBlockStatement(block);
			dummyVar1 = this.codeNodeTypes.Pop();
			return this.ReplaceDeclarations(block);
		}

		private bool RemoveVariable(VariableDefinition variable)
		{
			stackVariable2 = this.ShouldRemoveVariable(variable);
			if (stackVariable2)
			{
				this.methodContext.RemoveVariable(variable);
			}
			dummyVar0 = this.not_assigned.Remove(variable);
			return stackVariable2;
		}

		private BlockStatement ReplaceDeclarations(BlockStatement block)
		{
			this.state = 1;
			this.PopulateNotAssigned();
			block = (BlockStatement)this.VisitBlockStatement(block);
			this.state = 0;
			return block;
		}

		private bool ShouldRemoveVariable(VariableDefinition variable)
		{
			if (this.state != 1)
			{
				return false;
			}
			if (this.variableDeclarations.TryGetValue(variable, out V_0))
			{
				return !V_0.get_UsedInOtherStatements();
			}
			return !this.methodContext.get_VariableAssignmentData().ContainsKey(variable);
		}

		private void TryAddNewVariableDeclaration(VariableDefinition variable)
		{
			if (this.state != DeclareVariablesOnFirstAssignment.State.LocateDeclarations)
			{
				return;
			}
			V_0 = this.statements.Peek();
			if (!this.variableDeclarations.TryGetValue(variable, out V_1))
			{
				if (!this.methodContext.get_VariableAssignmentData().TryGetValue(variable, out V_2) || V_2 == 2)
				{
					V_3 = new DeclareVariablesOnFirstAssignment.StatementDeclaration(V_0);
					V_3.set_UsedInOtherStatements(this.codeNodeTypes.Peek() != 24);
					this.variableDeclarations.Add(variable, V_3);
				}
			}
			else
			{
				if (!V_1.get_UsedInOtherStatements())
				{
					V_1.set_UsedInOtherStatements(!this.IsChildOfCurrentStatement(V_0));
					return;
				}
			}
			return;
		}

		private bool TryDiscardVariable(VariableDefinition variable)
		{
			if (!this.not_assigned.Contains(variable))
			{
				return this.GetResultOnFirstOccurrence(variable);
			}
			if (!this.methodContext.get_Variables().Contains(variable))
			{
				return false;
			}
			this.TryAddNewVariableDeclaration(variable);
			return this.RemoveVariable(variable);
		}

		private ICodeNode VisitAssignExpression(BinaryExpression node)
		{
			if (node.get_Left().get_CodeNodeType() == 26)
			{
				stackVariable4 = true;
			}
			else
			{
				stackVariable4 = node.get_Left().get_CodeNodeType() == 27;
			}
			if (stackVariable4)
			{
				this.codeNodeTypes.Push(24);
			}
			node.set_Left((Expression)this.Visit(node.get_Left()));
			if (stackVariable4)
			{
				dummyVar0 = this.codeNodeTypes.Pop();
			}
			node.set_Right((Expression)this.Visit(node.get_Right()));
			return node;
		}

		public override ICodeNode VisitBinaryExpression(BinaryExpression node)
		{
			if (node.get_IsAssignmentExpression())
			{
				return this.VisitAssignExpression(node);
			}
			return this.VisitBinaryExpression(node);
		}

		public override ICodeNode VisitBlockStatement(BlockStatement node)
		{
			this.statements.Push(node);
			stackVariable5 = this.VisitBlockStatement(node);
			dummyVar0 = this.statements.Pop();
			return stackVariable5;
		}

		public override ICodeNode VisitForEachStatement(ForEachStatement node)
		{
			this.codeNodeTypes.Push(24);
			node.set_Variable((VariableDeclarationExpression)this.Visit(node.get_Variable()));
			dummyVar0 = this.codeNodeTypes.Pop();
			node.set_Collection((Expression)this.Visit(node.get_Collection()));
			node.set_Body((BlockStatement)this.Visit(node.get_Body()));
			return node;
		}

		public override ICodeNode VisitForStatement(ForStatement node)
		{
			this.statements.Push(node);
			dummyVar0 = this.VisitForStatement(node);
			dummyVar1 = this.statements.Pop();
			return node;
		}

		public override ICodeNode VisitLambdaExpression(LambdaExpression node)
		{
			if (this.state == DeclareVariablesOnFirstAssignment.State.LocateDeclarations)
			{
				dummyVar0 = this.Visit((node.CloneExpressionOnly() as LambdaExpression).get_Body());
			}
			return node;
		}

		public override ICodeNode VisitVariableDeclarationExpression(VariableDeclarationExpression node)
		{
			dummyVar0 = this.TryDiscardVariable(node.get_Variable());
			return node;
		}

		public override ICodeNode VisitVariableReferenceExpression(VariableReferenceExpression node)
		{
			V_0 = (VariableDefinition)node.get_Variable();
			if (!this.TryDiscardVariable(V_0))
			{
				return node;
			}
			if (V_0.get_VariableType().get_IsByReference())
			{
				return new RefVariableDeclarationExpression(V_0, node.get_UnderlyingSameMethodInstructions());
			}
			return new VariableDeclarationExpression(V_0, node.get_UnderlyingSameMethodInstructions());
		}

		private enum State
		{
			LocateDeclarations,
			ReplaceDeclarations
		}

		private class StatementDeclaration
		{
			public Statement Statement
			{
				get;
				set;
			}

			public bool UsedInOtherStatements
			{
				get;
				set;
			}

			public StatementDeclaration(Statement statement)
			{
				base();
				this.set_Statement(statement);
				return;
			}
		}
	}
}