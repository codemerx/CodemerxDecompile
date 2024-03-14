using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Common
{
	internal static class SwitchHelpers
	{
		public static bool BlockHasFallThroughSemantics(BlockStatement caseBody)
		{
			bool flag;
			if (caseBody == null)
			{
				return false;
			}
			if (caseBody.Statements.Count == 0)
			{
				return true;
			}
			Statement item = caseBody.Statements[caseBody.Statements.Count - 1];
			if (item.CodeNodeType != CodeNodeType.ExpressionStatement)
			{
				if (item.CodeNodeType == CodeNodeType.BreakStatement || item.CodeNodeType == CodeNodeType.ContinueStatement || item.CodeNodeType == CodeNodeType.GotoStatement)
				{
					return false;
				}
				if (item.CodeNodeType == CodeNodeType.IfStatement)
				{
					IfStatement ifStatement = item as IfStatement;
					if (ifStatement.Else != null)
					{
						if (SwitchHelpers.BlockHasFallThroughSemantics(ifStatement.Else))
						{
							return true;
						}
						return SwitchHelpers.BlockHasFallThroughSemantics(ifStatement.Then);
					}
				}
				else if (item.CodeNodeType == CodeNodeType.IfElseIfStatement)
				{
					IfElseIfStatement ifElseIfStatement = item as IfElseIfStatement;
					if (ifElseIfStatement.Else == null)
					{
						return true;
					}
					bool flag1 = SwitchHelpers.BlockHasFallThroughSemantics(ifElseIfStatement.Else);
					if (!flag1)
					{
						return false;
					}
					List<KeyValuePair<Expression, BlockStatement>>.Enumerator enumerator = ifElseIfStatement.ConditionBlocks.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							flag1 |= SwitchHelpers.BlockHasFallThroughSemantics(enumerator.Current.Value);
							if (flag1)
							{
								continue;
							}
							flag = false;
							return flag;
						}
						return true;
					}
					finally
					{
						((IDisposable)enumerator).Dispose();
					}
					return flag;
				}
			}
			else
			{
				Expression expression = (item as ExpressionStatement).Expression;
				if (expression != null && expression.CodeNodeType == CodeNodeType.ReturnExpression || expression.CodeNodeType == CodeNodeType.ThrowExpression)
				{
					return false;
				}
			}
			return true;
		}
	}
}