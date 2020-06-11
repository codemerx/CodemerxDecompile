using System;
using System.Collections.Generic;
using Mono.Cecil;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Steps.SwitchByString
{
	class SwitchByStringFixer
	{
		private Dictionary<int, string> valueDictionary;
		private VariableReferenceExpression theIntVariable;
		private VariableReferenceExpression theStringVariable;
		private readonly TypeSystem theTypeSystem;

		public SwitchByStringFixer(TypeSystem theTypeSystem)
		{
			this.theTypeSystem = theTypeSystem;
		}

		public Statement FixToSwitch(IfStatement node, VariableReferenceExpression stringVariable, VariableReferenceExpression intVariable)
		{
			/// The checks in the matcher ensured that the first statement in <paramref name="node"/> is an if statement, and
			/// that the dictionary is filled inside it's Then body.
			/// 
			this.theIntVariable = intVariable;
			this.theStringVariable = stringVariable;
			if (node.Then.Statements.Count != 2)
			{
				// sanity check;
				return node;
			}
			if (!(node.Then.Statements[0] is IfStatement) || !(node.Then.Statements[1] is IfStatement))
			{
				/// sanity checks
				return node;
			}
			FillValueDictionary((node.Then.Statements[0] as IfStatement).Then);
			BlockStatement result = FixSwitchingIf((node.Then.Statements[1] as IfStatement).Then);

			node.Then.Statements.Clear();
			node.Then = result;

			return node;
		}
  
		private BlockStatement FixSwitchingIf(BlockStatement switchBlock)
		{
			if (switchBlock.Statements.Count < 1)
			{
				// sanity check.
				return switchBlock;
			}
			Statement fixedSwitch = FixSwitchingStatement(switchBlock.Statements[0]);
			switchBlock.Statements.RemoveAt(0);
			switchBlock.AddStatementAt(0, fixedSwitch);
			return switchBlock;
		}
  
		private Statement FixSwitchingStatement(Statement statement)
		{
			if (statement is SwitchStatement)
			{ 
				SwitchStatement theSwitch = statement as SwitchStatement;
				if (theSwitch.Condition.Equals(theIntVariable)) 
				{
					theSwitch.Condition = theStringVariable.CloneExpressionOnly();
				}
				foreach (SwitchCase @case in theSwitch.Cases)
				{
					if (@case is ConditionCase)
					{ 
						ConditionCase condCase = @case as ConditionCase;
						int caseValue = (int)(condCase.Condition as LiteralExpression).Value;
						condCase.Condition = new LiteralExpression(valueDictionary[caseValue], theTypeSystem, null);
					}
				}
			}
			else if (statement is IfElseIfStatement)
			{
				IfElseIfStatement irregularSwitch = statement as IfElseIfStatement;
				foreach (KeyValuePair<Expression, BlockStatement> condPair in irregularSwitch.ConditionBlocks)
				{
					FixConditionExpression(condPair.Key as BinaryExpression);
				}
			}
			return statement;
		}
  
		private void FixConditionExpression(BinaryExpression binaryExpression)
		{
			if (binaryExpression.Operator == BinaryOperator.ValueEquality)
			{
				if (binaryExpression.Left.Equals(theIntVariable))
				{
					binaryExpression.Left = theStringVariable.CloneExpressionOnly();
					int value = (int)(binaryExpression.Right as LiteralExpression).Value;
					binaryExpression.Right = new LiteralExpression(valueDictionary[value],theTypeSystem,null);
				}
			}
			else if (binaryExpression.Operator == BinaryOperator.LogicalOr)
			{
				FixConditionExpression(binaryExpression.Left as BinaryExpression);
				FixConditionExpression(binaryExpression.Right as BinaryExpression);
			}
		}
  
		private void FillValueDictionary(BlockStatement then)
		{
			valueDictionary = new Dictionary<int, string>();
			for (int i = 1; i < then.Statements.Count-1; i++)
			{
				int value;
				string str;
				// the checks in the matcher ensured that this expression exists
				ExpressionStatement addInvocationExpressionStatement = then.Statements[i] as ExpressionStatement;
				if (addInvocationExpressionStatement == null || !(addInvocationExpressionStatement.Expression is MethodInvocationExpression))
				{
					// sanity check, should not happen;
					continue;
				}
				MethodInvocationExpression addInvocation = (then.Statements[i] as ExpressionStatement).Expression as MethodInvocationExpression;
				GetPair(addInvocation, out value, out str);
				valueDictionary.Add(value, str);
			}
		}
  
		private void GetPair(MethodInvocationExpression addInvocation, out int value, out string str)
		{
			value = -1;
			str = string.Empty;
			if (addInvocation.Arguments.Count != 2)
			{
				// sanity check
				return;
			}
			if (!(addInvocation.Arguments[0] is LiteralExpression) || !(addInvocation.Arguments[1] is LiteralExpression))
			{
				//sanity check
				return;
			}
			value = (int)(addInvocation.Arguments[1] as LiteralExpression).Value;
			str = (string)(addInvocation.Arguments[0] as LiteralExpression).Value;
		}
	}
}
