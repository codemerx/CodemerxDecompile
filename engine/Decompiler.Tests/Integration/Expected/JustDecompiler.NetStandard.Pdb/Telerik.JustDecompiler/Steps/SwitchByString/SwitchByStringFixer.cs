using Mono.Cecil;
using System;
using System.Collections.Generic;
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
			base();
			this.theTypeSystem = theTypeSystem;
			return;
		}

		private void FillValueDictionary(BlockStatement then)
		{
			this.valueDictionary = new Dictionary<int, string>();
			V_0 = 1;
			while (V_0 < then.get_Statements().get_Count() - 1)
			{
				V_3 = then.get_Statements().get_Item(V_0) as ExpressionStatement;
				if (V_3 != null && V_3.get_Expression() as MethodInvocationExpression != null)
				{
					V_4 = (then.get_Statements().get_Item(V_0) as ExpressionStatement).get_Expression() as MethodInvocationExpression;
					this.GetPair(V_4, out V_1, out V_2);
					this.valueDictionary.Add(V_1, V_2);
				}
				V_0 = V_0 + 1;
			}
			return;
		}

		private void FixConditionExpression(BinaryExpression binaryExpression)
		{
			if (binaryExpression.get_Operator() != 9)
			{
				if (binaryExpression.get_Operator() == 11)
				{
					this.FixConditionExpression(binaryExpression.get_Left() as BinaryExpression);
					this.FixConditionExpression(binaryExpression.get_Right() as BinaryExpression);
				}
			}
			else
			{
				if (binaryExpression.get_Left().Equals(this.theIntVariable))
				{
					binaryExpression.set_Left(this.theStringVariable.CloneExpressionOnly());
					V_0 = (Int32)(binaryExpression.get_Right() as LiteralExpression).get_Value();
					binaryExpression.set_Right(new LiteralExpression(this.valueDictionary.get_Item(V_0), this.theTypeSystem, null));
					return;
				}
			}
			return;
		}

		private BlockStatement FixSwitchingIf(BlockStatement switchBlock)
		{
			if (switchBlock.get_Statements().get_Count() < 1)
			{
				return switchBlock;
			}
			V_0 = this.FixSwitchingStatement(switchBlock.get_Statements().get_Item(0));
			switchBlock.get_Statements().RemoveAt(0);
			switchBlock.AddStatementAt(0, V_0);
			return switchBlock;
		}

		private Statement FixSwitchingStatement(Statement statement)
		{
			if (statement as SwitchStatement == null)
			{
				if (statement as IfElseIfStatement != null)
				{
					V_4 = (statement as IfElseIfStatement).get_ConditionBlocks().GetEnumerator();
					try
					{
						while (V_4.MoveNext())
						{
							V_5 = V_4.get_Current();
							this.FixConditionExpression(V_5.get_Key() as BinaryExpression);
						}
					}
					finally
					{
						((IDisposable)V_4).Dispose();
					}
				}
			}
			else
			{
				V_0 = statement as SwitchStatement;
				if (V_0.get_Condition().Equals(this.theIntVariable))
				{
					V_0.set_Condition(this.theStringVariable.CloneExpressionOnly());
				}
				V_1 = V_0.get_Cases().GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = V_1.get_Current();
						if (V_2 as ConditionCase == null)
						{
							continue;
						}
						stackVariable34 = V_2 as ConditionCase;
						V_3 = (Int32)(stackVariable34.get_Condition() as LiteralExpression).get_Value();
						stackVariable34.set_Condition(new LiteralExpression(this.valueDictionary.get_Item(V_3), this.theTypeSystem, null));
					}
				}
				finally
				{
					if (V_1 != null)
					{
						V_1.Dispose();
					}
				}
			}
			return statement;
		}

		public Statement FixToSwitch(IfStatement node, VariableReferenceExpression stringVariable, VariableReferenceExpression intVariable)
		{
			this.theIntVariable = intVariable;
			this.theStringVariable = stringVariable;
			if (node.get_Then().get_Statements().get_Count() != 2)
			{
				return node;
			}
			if (node.get_Then().get_Statements().get_Item(0) as IfStatement == null || node.get_Then().get_Statements().get_Item(1) as IfStatement == null)
			{
				return node;
			}
			this.FillValueDictionary((node.get_Then().get_Statements().get_Item(0) as IfStatement).get_Then());
			V_0 = this.FixSwitchingIf((node.get_Then().get_Statements().get_Item(1) as IfStatement).get_Then());
			node.get_Then().get_Statements().Clear();
			node.set_Then(V_0);
			return node;
		}

		private void GetPair(MethodInvocationExpression addInvocation, out int value, out string str)
		{
			value = -1;
			str = String.Empty;
			if (addInvocation.get_Arguments().get_Count() != 2)
			{
				return;
			}
			if (addInvocation.get_Arguments().get_Item(0) as LiteralExpression == null || addInvocation.get_Arguments().get_Item(1) as LiteralExpression == null)
			{
				return;
			}
			value = (Int32)(addInvocation.get_Arguments().get_Item(1) as LiteralExpression).get_Value();
			str = (String)(addInvocation.get_Arguments().get_Item(0) as LiteralExpression).get_Value();
			return;
		}
	}
}