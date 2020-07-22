using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps.CodePatterns
{
	internal class CodePatternsContext
	{
		public Dictionary<VariableDefinition, DefineUseCount> VariableToDefineUseCountContext
		{
			get;
			private set;
		}

		public Dictionary<VariableDefinition, ExpressionStatement> VariableToSingleAssignmentMap
		{
			get;
			private set;
		}

		private CodePatternsContext()
		{
			base();
			this.set_VariableToSingleAssignmentMap(new Dictionary<VariableDefinition, ExpressionStatement>());
			this.set_VariableToDefineUseCountContext(new Dictionary<VariableDefinition, DefineUseCount>());
			return;
		}

		public CodePatternsContext(BlockStatement body)
		{
			this();
			(new CodePatternsContext.VariableDefineUseCounter(this)).Visit(body);
			return;
		}

		public CodePatternsContext(StatementCollection statements)
		{
			this();
			(new CodePatternsContext.VariableDefineUseCounter(this)).Visit(statements);
			return;
		}

		private class VariableDefineUseCounter : BaseCodeVisitor
		{
			private readonly HashSet<VariableDefinition> bannedVariables;

			private readonly CodePatternsContext patternContext;

			public VariableDefineUseCounter(CodePatternsContext patternContext)
			{
				this.bannedVariables = new HashSet<VariableDefinition>();
				base();
				this.patternContext = patternContext;
				return;
			}

			private void AddDefinition(VariableDefinition variable, ExpressionStatement expressionStatement)
			{
				if (!this.bannedVariables.Contains(variable))
				{
					if (this.patternContext.get_VariableToDefineUseCountContext().TryGetValue(variable, out V_0))
					{
						stackVariable25 = V_0;
						stackVariable25.DefineCount = stackVariable25.DefineCount + 1;
						dummyVar0 = this.patternContext.get_VariableToSingleAssignmentMap().Remove(variable);
						return;
					}
					V_1 = new DefineUseCount();
					stackVariable11 = V_1;
					stackVariable11.DefineCount = stackVariable11.DefineCount + 1;
					this.patternContext.get_VariableToDefineUseCountContext().Add(variable, V_1);
					this.patternContext.get_VariableToSingleAssignmentMap().Add(variable, expressionStatement);
				}
				return;
			}

			private void AddUsage(VariableDefinition variable)
			{
				if (!this.patternContext.get_VariableToDefineUseCountContext().TryGetValue(variable, out V_0))
				{
					this.RemoveVariable(variable);
					return;
				}
				stackVariable8 = V_0;
				stackVariable8.UseCount = stackVariable8.UseCount + 1;
				return;
			}

			private void RemoveVariable(VariableDefinition variable)
			{
				if (this.bannedVariables.Add(variable))
				{
					dummyVar0 = this.patternContext.get_VariableToDefineUseCountContext().Remove(variable);
					dummyVar1 = this.patternContext.get_VariableToSingleAssignmentMap().Remove(variable);
				}
				return;
			}

			public override void VisitBinaryExpression(BinaryExpression node)
			{
				if (node.get_IsAssignmentExpression() && node.get_Left().get_CodeNodeType() == 26)
				{
					this.RemoveVariable((node.get_Left() as VariableReferenceExpression).get_Variable().Resolve());
				}
				this.VisitBinaryExpression(node);
				return;
			}

			public override void VisitExpressionStatement(ExpressionStatement node)
			{
				if (node.IsAssignmentStatement() && node.get_Parent().get_CodeNodeType() == CodeNodeType.BlockStatement)
				{
					V_0 = node.get_Expression() as BinaryExpression;
					if (V_0.get_Left().get_CodeNodeType() == 26)
					{
						this.Visit(V_0.get_Right());
						this.AddDefinition((V_0.get_Left() as VariableReferenceExpression).get_Variable().Resolve(), node);
						return;
					}
				}
				this.VisitExpressionStatement(node);
				return;
			}

			public override void VisitUnaryExpression(UnaryExpression node)
			{
				if (node.get_Operator() != 9 && node.get_Operator() != 7 || node.get_Operand().get_CodeNodeType() != 26)
				{
					this.VisitUnaryExpression(node);
					return;
				}
				this.RemoveVariable((node.get_Operand() as VariableReferenceExpression).get_Variable().Resolve());
				return;
			}

			public override void VisitVariableDeclarationExpression(VariableDeclarationExpression node)
			{
				this.RemoveVariable(node.get_Variable().Resolve());
				return;
			}

			public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
			{
				this.AddUsage(node.get_Variable().Resolve());
				return;
			}
		}
	}
}