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
			this.VariableToSingleAssignmentMap = new Dictionary<VariableDefinition, ExpressionStatement>();
			this.VariableToDefineUseCountContext = new Dictionary<VariableDefinition, DefineUseCount>();
		}

		public CodePatternsContext(BlockStatement body) : this()
		{
			(new CodePatternsContext.VariableDefineUseCounter(this)).Visit(body);
		}

		public CodePatternsContext(StatementCollection statements) : this()
		{
			(new CodePatternsContext.VariableDefineUseCounter(this)).Visit(statements);
		}

		private class VariableDefineUseCounter : BaseCodeVisitor
		{
			private readonly HashSet<VariableDefinition> bannedVariables;

			private readonly CodePatternsContext patternContext;

			public VariableDefineUseCounter(CodePatternsContext patternContext)
			{
				this.patternContext = patternContext;
			}

			private void AddDefinition(VariableDefinition variable, ExpressionStatement expressionStatement)
			{
				DefineUseCount defineUseCount;
				if (!this.bannedVariables.Contains(variable))
				{
					if (this.patternContext.VariableToDefineUseCountContext.TryGetValue(variable, out defineUseCount))
					{
						defineUseCount.DefineCount++;
						this.patternContext.VariableToSingleAssignmentMap.Remove(variable);
						return;
					}
					DefineUseCount defineUseCount1 = new DefineUseCount();
					defineUseCount1.DefineCount++;
					this.patternContext.VariableToDefineUseCountContext.Add(variable, defineUseCount1);
					this.patternContext.VariableToSingleAssignmentMap.Add(variable, expressionStatement);
				}
			}

			private void AddUsage(VariableDefinition variable)
			{
				DefineUseCount defineUseCount;
				if (!this.patternContext.VariableToDefineUseCountContext.TryGetValue(variable, out defineUseCount))
				{
					this.RemoveVariable(variable);
					return;
				}
				defineUseCount.UseCount++;
			}

			private void RemoveVariable(VariableDefinition variable)
			{
				if (this.bannedVariables.Add(variable))
				{
					this.patternContext.VariableToDefineUseCountContext.Remove(variable);
					this.patternContext.VariableToSingleAssignmentMap.Remove(variable);
				}
			}

			public override void VisitBinaryExpression(BinaryExpression node)
			{
				if (node.IsAssignmentExpression && node.Left.CodeNodeType == CodeNodeType.VariableReferenceExpression)
				{
					this.RemoveVariable((node.Left as VariableReferenceExpression).Variable.Resolve());
				}
				base.VisitBinaryExpression(node);
			}

			public override void VisitExpressionStatement(ExpressionStatement node)
			{
				if (node.IsAssignmentStatement() && node.Parent.CodeNodeType == CodeNodeType.BlockStatement)
				{
					BinaryExpression expression = node.Expression as BinaryExpression;
					if (expression.Left.CodeNodeType == CodeNodeType.VariableReferenceExpression)
					{
						this.Visit(expression.Right);
						this.AddDefinition((expression.Left as VariableReferenceExpression).Variable.Resolve(), node);
						return;
					}
				}
				base.VisitExpressionStatement(node);
			}

			public override void VisitUnaryExpression(UnaryExpression node)
			{
				if (node.Operator != UnaryOperator.AddressOf && node.Operator != UnaryOperator.AddressReference || node.Operand.CodeNodeType != CodeNodeType.VariableReferenceExpression)
				{
					base.VisitUnaryExpression(node);
					return;
				}
				this.RemoveVariable((node.Operand as VariableReferenceExpression).Variable.Resolve());
			}

			public override void VisitVariableDeclarationExpression(VariableDeclarationExpression node)
			{
				this.RemoveVariable(node.Variable.Resolve());
			}

			public override void VisitVariableReferenceExpression(VariableReferenceExpression node)
			{
				this.AddUsage(node.Variable.Resolve());
			}
		}
	}
}