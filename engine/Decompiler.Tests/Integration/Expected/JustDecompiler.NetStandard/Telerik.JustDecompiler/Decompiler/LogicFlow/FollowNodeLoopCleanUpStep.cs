using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Conditions;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Exceptions;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Loops;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Switches;
using Telerik.JustDecompiler.Steps;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow
{
	internal class FollowNodeLoopCleanUpStep : IDecompilationStep
	{
		private readonly HashSet<ILogicalConstruct> visitedConstructs;

		public FollowNodeLoopCleanUpStep()
		{
			base();
			this.visitedConstructs = new HashSet<ILogicalConstruct>();
			return;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.ProcessLogicalConstruct(context.get_MethodContext().get_LogicalConstructsTree());
			return body;
		}

		private void ProcessGotoFlowConstructs(BlockLogicalConstruct theConstruct)
		{
			V_0 = new ILogicalConstruct[theConstruct.get_Children().get_Count()];
			V_1 = 0;
			V_3 = theConstruct.get_Children().GetEnumerator();
			try
			{
				while (V_3.MoveNext())
				{
					V_4 = (ILogicalConstruct)V_3.get_Current();
					stackVariable14 = V_1;
					V_1 = stackVariable14 + 1;
					V_0[stackVariable14] = V_4;
				}
			}
			finally
			{
				((IDisposable)V_3).Dispose();
			}
			Array.Sort<ISingleEntrySubGraph>(V_0);
			V_2 = new HashSet<ILogicalConstruct>();
			V_6 = V_0;
			V_7 = 0;
			while (V_7 < (int)V_6.Length)
			{
				V_8 = V_6[V_7];
				if (this.visitedConstructs.Add(V_8))
				{
					if (this.visitedConstructs.Contains(V_8.get_FollowNode()) || !V_2.Add(V_8.get_FollowNode()))
					{
						V_8.set_CFGFollowNode(null);
					}
					this.ProcessLogicalConstruct(V_8);
				}
				V_7 = V_7 + 1;
			}
			return;
		}

		private void ProcessLogicalConstruct(ILogicalConstruct theConstruct)
		{
			if (theConstruct as BlockLogicalConstruct == null)
			{
				if (theConstruct as ExceptionHandlingLogicalConstruct == null)
				{
					if (theConstruct as IfLogicalConstruct == null)
					{
						if (theConstruct as LoopLogicalConstruct == null)
						{
							if (theConstruct as SwitchLogicalConstruct == null)
							{
								if (theConstruct as ConditionLogicalConstruct != null)
								{
									this.ProcessLogicalConstruct((theConstruct as ConditionLogicalConstruct).get_FirstBlock());
								}
							}
							else
							{
								V_6 = theConstruct as SwitchLogicalConstruct;
								V_7 = V_6.get_ConditionCases();
								V_2 = 0;
								while (V_2 < (int)V_7.Length)
								{
									this.ProcessLogicalConstruct(V_7[V_2]);
									V_2 = V_2 + 1;
								}
								this.ProcessLogicalConstruct(V_6.get_DefaultCase());
							}
						}
						else
						{
							V_5 = theConstruct as LoopLogicalConstruct;
							this.ProcessLogicalConstruct(V_5.get_LoopBodyBlock());
							this.ProcessLogicalConstruct(V_5.get_LoopCondition());
						}
					}
					else
					{
						V_4 = theConstruct as IfLogicalConstruct;
						this.ProcessLogicalConstruct(V_4.get_Then());
						if (V_4.get_Else() != null)
						{
							this.ProcessLogicalConstruct(V_4.get_Else());
						}
					}
				}
				else
				{
					this.ProcessLogicalConstruct((theConstruct as ExceptionHandlingLogicalConstruct).get_Try());
					if (theConstruct as TryCatchFilterLogicalConstruct == null)
					{
						if (theConstruct as TryFaultLogicalConstruct == null)
						{
							if (theConstruct as TryFinallyLogicalConstruct != null)
							{
								this.ProcessLogicalConstruct((theConstruct as TryFinallyLogicalConstruct).get_Finally());
							}
						}
						else
						{
							this.ProcessLogicalConstruct((theConstruct as TryFaultLogicalConstruct).get_Fault());
						}
					}
					else
					{
						V_1 = (theConstruct as TryCatchFilterLogicalConstruct).get_Handlers();
						V_2 = 0;
						while (V_2 < (int)V_1.Length)
						{
							V_3 = V_1[V_2];
							if (V_3.get_HandlerType() != 1)
							{
								if (V_3.get_HandlerType() == FilteringExceptionHandlerType.Catch)
								{
									this.ProcessLogicalConstruct(V_3 as ExceptionHandlingBlockCatch);
								}
							}
							else
							{
								this.ProcessLogicalConstruct((V_3 as ExceptionHandlingBlockFilter).get_Filter());
								this.ProcessLogicalConstruct((V_3 as ExceptionHandlingBlockFilter).get_Handler());
							}
							V_2 = V_2 + 1;
						}
					}
				}
			}
			else
			{
				V_0 = (ILogicalConstruct)theConstruct.get_Entry();
				while (V_0 != null)
				{
					this.ProcessLogicalConstruct(V_0);
					if (this.visitedConstructs.Contains(V_0.get_FollowNode()))
					{
						V_0.set_CFGFollowNode(null);
					}
					V_0 = V_0.get_FollowNode();
				}
				this.ProcessGotoFlowConstructs(theConstruct as BlockLogicalConstruct);
			}
			dummyVar0 = this.visitedConstructs.Add(theConstruct);
			return;
		}
	}
}