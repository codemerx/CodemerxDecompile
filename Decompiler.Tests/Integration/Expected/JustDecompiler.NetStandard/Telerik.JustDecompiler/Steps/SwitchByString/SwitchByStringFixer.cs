using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps.SwitchByString
{
	internal class SwitchByStringFixer
	{
		private Dictionary<int, string> valueDictionary;

		private VariableReferenceExpression theIntVariable;

		private VariableReferenceExpression theStringVariable;

		private readonly TypeSystem theTypeSystem;

		public SwitchByStringFixer(TypeSystem theTypeSystem)
		{
			this.theTypeSystem = theTypeSystem;
		}

		private void FillValueDictionary(BlockStatement then)
		{
			int num;
			string str;
			this.valueDictionary = new Dictionary<int, string>();
			for (int i = 1; i < then.Statements.Count - 1; i++)
			{
				ExpressionStatement item = then.Statements[i] as ExpressionStatement;
				if (item != null && item.Expression is MethodInvocationExpression)
				{
					MethodInvocationExpression expression = (then.Statements[i] as ExpressionStatement).Expression as MethodInvocationExpression;
					this.GetPair(expression, out num, out str);
					this.valueDictionary.Add(num, str);
				}
			}
		}

		private void FixConditionExpression(BinaryExpression binaryExpression)
		{
			if (binaryExpression.Operator == BinaryOperator.ValueEquality)
			{
				if (binaryExpression.Left.Equals(this.theIntVariable))
				{
					binaryExpression.Left = this.theStringVariable.CloneExpressionOnly();
					int value = (Int32)(binaryExpression.Right as LiteralExpression).Value;
					binaryExpression.Right = new LiteralExpression(this.valueDictionary[value], this.theTypeSystem, null);
					return;
				}
			}
			else if (binaryExpression.Operator == BinaryOperator.LogicalOr)
			{
				this.FixConditionExpression(binaryExpression.Left as BinaryExpression);
				this.FixConditionExpression(binaryExpression.Right as BinaryExpression);
			}
		}

		private BlockStatement FixSwitchingIf(BlockStatement switchBlock)
		{
			if (switchBlock.Statements.Count < 1)
			{
				return switchBlock;
			}
			Statement statement = this.FixSwitchingStatement(switchBlock.Statements[0]);
			switchBlock.Statements.RemoveAt(0);
			switchBlock.AddStatementAt(0, statement);
			return switchBlock;
		}

		private Statement FixSwitchingStatement(Statement statement)
		{
			if (statement is SwitchStatement)
			{
				SwitchStatement switchStatement = statement as SwitchStatement;
				if (switchStatement.Condition.Equals(this.theIntVariable))
				{
					switchStatement.Condition = this.theStringVariable.CloneExpressionOnly();
				}
				foreach (SwitchCase @case in switchStatement.Cases)
				{
					if (!(@case is ConditionCase))
					{
						continue;
					}
					ConditionCase literalExpression = @case as ConditionCase;
					int value = (Int32)(literalExpression.Condition as LiteralExpression).Value;
					literalExpression.Condition = new LiteralExpression(this.valueDictionary[value], this.theTypeSystem, null);
				}
			}
			else if (statement is IfElseIfStatement)
			{
				foreach (KeyValuePair<Expression, BlockStatement> conditionBlock in (statement as IfElseIfStatement).ConditionBlocks)
				{
					this.FixConditionExpression(conditionBlock.Key as BinaryExpression);
				}
			}
			return statement;
		}

		public Statement FixToSwitch(IfStatement node, VariableReferenceExpression stringVariable, VariableReferenceExpression intVariable)
		{
			this.theIntVariable = intVariable;
			this.theStringVariable = stringVariable;
			if (node.Then.Statements.Count != 2)
			{
				return node;
			}
			if (!(node.Then.Statements[0] is IfStatement) || !(node.Then.Statements[1] is IfStatement))
			{
				return node;
			}
			this.FillValueDictionary((node.Then.Statements[0] as IfStatement).Then);
			BlockStatement blockStatement = this.FixSwitchingIf((node.Then.Statements[1] as IfStatement).Then);
			node.Then.Statements.Clear();
			node.Then = blockStatement;
			return node;
		}

		private void GetPair(MethodInvocationExpression addInvocation, out int value, out string str)
		{
			value = -1;
			str = String.Empty;
			if (addInvocation.Arguments.Count != 2)
			{
				return;
			}
			if (!(addInvocation.Arguments[0] is LiteralExpression) || !(addInvocation.Arguments[1] is LiteralExpression))
			{
				return;
			}
			value = (Int32)(addInvocation.Arguments[1] as LiteralExpression).Value;
			str = (String)(addInvocation.Arguments[0] as LiteralExpression).Value;
		}
	}
}