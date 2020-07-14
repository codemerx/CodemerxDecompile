using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Common;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class CreateCompilerOptimizedSwitchByStringStatementsStep : BaseCodeTransformer, IDecompilationStep
	{
		private CompilerOptimizedSwitchByStringData switchByStringData;

		public CreateCompilerOptimizedSwitchByStringStatementsStep()
		{
		}

		private CompilerOptimizedSwitchByStringStatement ComposeSwitch(CreateCompilerOptimizedSwitchByStringStatementsStep.SwitchData data)
		{
			CompilerOptimizedSwitchByStringStatement compilerOptimizedSwitchByStringStatement = new CompilerOptimizedSwitchByStringStatement(data.SwitchExpression, data.SwitchExpressionLoadInstructions);
			foreach (KeyValuePair<Expression, BlockStatement> caseConditionToBlockMap in data.CaseConditionToBlockMap)
			{
				if (caseConditionToBlockMap.Value != null && SwitchHelpers.BlockHasFallThroughSemantics(caseConditionToBlockMap.Value))
				{
					caseConditionToBlockMap.Value.AddStatement(new BreakSwitchCaseStatement());
				}
				compilerOptimizedSwitchByStringStatement.AddCase(new ConditionCase(caseConditionToBlockMap.Key, caseConditionToBlockMap.Value));
			}
			if (data.HaveDefaultCase)
			{
				if (SwitchHelpers.BlockHasFallThroughSemantics(data.DefaultCase))
				{
					data.DefaultCase.AddStatement(new BreakSwitchCaseStatement());
				}
				compilerOptimizedSwitchByStringStatement.AddCase(new DefaultCase(data.DefaultCase));
			}
			return compilerOptimizedSwitchByStringStatement;
		}

		private bool IsSwitchByString(IfElseIfStatement node)
		{
			bool flag;
			Dictionary<int, List<int>>.Enumerator enumerator = this.switchByStringData.SwitchBlocksToCasesMap.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					List<int>.Enumerator enumerator1 = enumerator.Current.Value.GetEnumerator();
					try
					{
						while (enumerator1.MoveNext())
						{
							int current = enumerator1.Current;
							if (!node.SearchableUnderlyingSameMethodInstructionOffsets.Contains(current))
							{
								continue;
							}
							flag = true;
							return flag;
						}
					}
					finally
					{
						((IDisposable)enumerator1).Dispose();
					}
				}
				return false;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.switchByStringData = context.MethodContext.SwitchByStringData;
			if (this.switchByStringData.SwitchBlocksStartInstructions.Count == 0)
			{
				return body;
			}
			return (BlockStatement)this.Visit(body);
		}

		private bool TryGetSwitchData(IfElseIfStatement node, out CreateCompilerOptimizedSwitchByStringStatementsStep.SwitchData data)
		{
			bool flag;
			data = new CreateCompilerOptimizedSwitchByStringStatementsStep.SwitchData();
			List<KeyValuePair<Expression, BlockStatement>>.Enumerator enumerator = node.ConditionBlocks.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<Expression, BlockStatement> current = enumerator.Current;
					if (this.TryMatchCondition(current.Key, current.Value, data))
					{
						continue;
					}
					flag = false;
					return flag;
				}
				if (node.Else != null)
				{
					data.DefaultCase = node.Else;
				}
				return true;
			}
			finally
			{
				((IDisposable)enumerator).Dispose();
			}
			return flag;
		}

		private bool TryMatchCondition(Expression condition, BlockStatement block, CreateCompilerOptimizedSwitchByStringStatementsStep.SwitchData data)
		{
			BinaryExpression operand;
			if (condition.CodeNodeType != CodeNodeType.UnaryExpression && condition.CodeNodeType != CodeNodeType.BinaryExpression)
			{
				return false;
			}
			if (condition.CodeNodeType != CodeNodeType.BinaryExpression)
			{
				UnaryExpression unaryExpression = condition as UnaryExpression;
				if (unaryExpression.Operator != UnaryOperator.None || unaryExpression.Operand.CodeNodeType != CodeNodeType.BinaryExpression)
				{
					return false;
				}
				operand = unaryExpression.Operand as BinaryExpression;
			}
			else
			{
				operand = condition as BinaryExpression;
				if (operand.Operator == BinaryOperator.LogicalOr)
				{
					if (!this.TryMatchCondition(operand.Left, null, data) || !this.TryMatchCondition(operand.Right, null, data))
					{
						return false;
					}
					if (block != null)
					{
						int count = data.CaseConditionToBlockMap.Count - 1;
						Expression key = data.CaseConditionToBlockMap[count].Key;
						data.CaseConditionToBlockMap[count] = new KeyValuePair<Expression, BlockStatement>(key, block);
					}
					return true;
				}
			}
			if (operand.Right.CodeNodeType != CodeNodeType.LiteralExpression || operand.Operator != BinaryOperator.ValueEquality)
			{
				return false;
			}
			LiteralExpression right = operand.Right as LiteralExpression;
			if (condition.CodeNodeType == CodeNodeType.UnaryExpression && right.ExpressionType.get_FullName() != "System.String")
			{
				return false;
			}
			if (condition.CodeNodeType == CodeNodeType.BinaryExpression && (right.ExpressionType.get_FullName() != "System.Object" || right.Value != null))
			{
				return false;
			}
			if (data.SwitchExpression != null)
			{
				if (!data.SwitchExpression.Equals(operand.Left))
				{
					return false;
				}
				data.SwitchExpressionLoadInstructions.Add(operand.Left.UnderlyingSameMethodInstructions.First<Instruction>().get_Offset());
			}
			else
			{
				data.SwitchExpression = operand.Left;
			}
			data.CaseConditionToBlockMap.Add(new KeyValuePair<Expression, BlockStatement>(right, block));
			return true;
		}

		public override ICodeNode VisitIfElseIfStatement(IfElseIfStatement node)
		{
			CreateCompilerOptimizedSwitchByStringStatementsStep.SwitchData switchDatum;
			if (!this.IsSwitchByString(node) || !this.TryGetSwitchData(node, out switchDatum))
			{
				return base.VisitIfElseIfStatement(node);
			}
			return this.Visit(this.ComposeSwitch(switchDatum));
		}

		private class SwitchData
		{
			public List<KeyValuePair<Expression, BlockStatement>> CaseConditionToBlockMap
			{
				get;
				set;
			}

			public BlockStatement DefaultCase
			{
				get;
				set;
			}

			public bool HaveDefaultCase
			{
				get
				{
					return this.DefaultCase != null;
				}
			}

			public Expression SwitchExpression
			{
				get;
				set;
			}

			public List<int> SwitchExpressionLoadInstructions
			{
				get;
				set;
			}

			public SwitchData()
			{
				this.SwitchExpression = null;
				this.SwitchExpressionLoadInstructions = new List<int>();
				this.CaseConditionToBlockMap = new List<KeyValuePair<Expression, BlockStatement>>();
				this.DefaultCase = null;
			}
		}
	}
}