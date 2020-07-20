using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class CreateCompilerOptimizedSwitchByStringStatementsStep : BaseCodeTransformer, IDecompilationStep
	{
		private CompilerOptimizedSwitchByStringData switchByStringData;

		public CreateCompilerOptimizedSwitchByStringStatementsStep()
		{
			base();
			return;
		}

		private CompilerOptimizedSwitchByStringStatement ComposeSwitch(CreateCompilerOptimizedSwitchByStringStatementsStep.SwitchData data)
		{
			V_0 = new CompilerOptimizedSwitchByStringStatement(data.get_SwitchExpression(), data.get_SwitchExpressionLoadInstructions());
			V_1 = data.get_CaseConditionToBlockMap().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (V_2.get_Value() != null && SwitchHelpers.BlockHasFallThroughSemantics(V_2.get_Value()))
					{
						V_2.get_Value().AddStatement(new BreakSwitchCaseStatement());
					}
					V_0.AddCase(new ConditionCase(V_2.get_Key(), V_2.get_Value()));
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			if (data.get_HaveDefaultCase())
			{
				if (SwitchHelpers.BlockHasFallThroughSemantics(data.get_DefaultCase()))
				{
					data.get_DefaultCase().AddStatement(new BreakSwitchCaseStatement());
				}
				V_0.AddCase(new DefaultCase(data.get_DefaultCase()));
			}
			return V_0;
		}

		private bool IsSwitchByString(IfElseIfStatement node)
		{
			V_0 = this.switchByStringData.get_SwitchBlocksToCasesMap().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					V_2 = V_1.get_Value().GetEnumerator();
					try
					{
						while (V_2.MoveNext())
						{
							V_3 = V_2.get_Current();
							if (!node.get_SearchableUnderlyingSameMethodInstructionOffsets().Contains(V_3))
							{
								continue;
							}
							V_4 = true;
							goto Label1;
						}
					}
					finally
					{
						((IDisposable)V_2).Dispose();
					}
				}
				goto Label0;
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
		Label1:
			return V_4;
		Label0:
			return false;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.switchByStringData = context.get_MethodContext().get_SwitchByStringData();
			if (this.switchByStringData.get_SwitchBlocksStartInstructions().get_Count() == 0)
			{
				return body;
			}
			return (BlockStatement)this.Visit(body);
		}

		private bool TryGetSwitchData(IfElseIfStatement node, out CreateCompilerOptimizedSwitchByStringStatementsStep.SwitchData data)
		{
			data = new CreateCompilerOptimizedSwitchByStringStatementsStep.SwitchData();
			V_0 = node.get_ConditionBlocks().GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					if (this.TryMatchCondition(V_1.get_Key(), V_1.get_Value(), data))
					{
						continue;
					}
					V_2 = false;
					goto Label1;
				}
				goto Label0;
			}
			finally
			{
				((IDisposable)V_0).Dispose();
			}
		Label1:
			return V_2;
		Label0:
			if (node.get_Else() != null)
			{
				data.set_DefaultCase(node.get_Else());
			}
			return true;
		}

		private bool TryMatchCondition(Expression condition, BlockStatement block, CreateCompilerOptimizedSwitchByStringStatementsStep.SwitchData data)
		{
			if (condition.get_CodeNodeType() != 23 && condition.get_CodeNodeType() != 24)
			{
				return false;
			}
			if (condition.get_CodeNodeType() != 24)
			{
				V_5 = condition as UnaryExpression;
				if (V_5.get_Operator() != 11 || V_5.get_Operand().get_CodeNodeType() != 24)
				{
					return false;
				}
				V_0 = V_5.get_Operand() as BinaryExpression;
			}
			else
			{
				V_0 = condition as BinaryExpression;
				if (V_0.get_Operator() == 11)
				{
					if (!this.TryMatchCondition(V_0.get_Left(), null, data) || !this.TryMatchCondition(V_0.get_Right(), null, data))
					{
						return false;
					}
					if (block != null)
					{
						V_2 = data.get_CaseConditionToBlockMap().get_Count() - 1;
						V_4 = data.get_CaseConditionToBlockMap().get_Item(V_2);
						V_3 = V_4.get_Key();
						data.get_CaseConditionToBlockMap().set_Item(V_2, new KeyValuePair<Expression, BlockStatement>(V_3, block));
					}
					return true;
				}
			}
			if (V_0.get_Right().get_CodeNodeType() != 22 || V_0.get_Operator() != 9)
			{
				return false;
			}
			V_1 = V_0.get_Right() as LiteralExpression;
			if (condition.get_CodeNodeType() == 23 && String.op_Inequality(V_1.get_ExpressionType().get_FullName(), "System.String"))
			{
				return false;
			}
			if (condition.get_CodeNodeType() == 24 && String.op_Inequality(V_1.get_ExpressionType().get_FullName(), "System.Object") || V_1.get_Value() != null)
			{
				return false;
			}
			if (data.get_SwitchExpression() != null)
			{
				if (!data.get_SwitchExpression().Equals(V_0.get_Left()))
				{
					return false;
				}
				data.get_SwitchExpressionLoadInstructions().Add(V_0.get_Left().get_UnderlyingSameMethodInstructions().First<Instruction>().get_Offset());
			}
			else
			{
				data.set_SwitchExpression(V_0.get_Left());
			}
			data.get_CaseConditionToBlockMap().Add(new KeyValuePair<Expression, BlockStatement>(V_1, block));
			return true;
		}

		public override ICodeNode VisitIfElseIfStatement(IfElseIfStatement node)
		{
			if (!this.IsSwitchByString(node) || !this.TryGetSwitchData(node, out V_0))
			{
				return this.VisitIfElseIfStatement(node);
			}
			return this.Visit(this.ComposeSwitch(V_0));
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
					return this.get_DefaultCase() != null;
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
				base();
				this.set_SwitchExpression(null);
				this.set_SwitchExpressionLoadInstructions(new List<int>());
				this.set_CaseConditionToBlockMap(new List<KeyValuePair<Expression, BlockStatement>>());
				this.set_DefaultCase(null);
				return;
			}
		}
	}
}