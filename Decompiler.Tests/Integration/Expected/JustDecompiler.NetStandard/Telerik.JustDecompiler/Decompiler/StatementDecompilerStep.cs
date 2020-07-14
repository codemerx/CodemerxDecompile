using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Common;
using Telerik.JustDecompiler.Decompiler.DefineUseAnalysis;
using Telerik.JustDecompiler.Decompiler.LogicFlow;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Conditions;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Exceptions;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Loops;
using Telerik.JustDecompiler.Decompiler.LogicFlow.Switches;
using Telerik.JustDecompiler.Steps;

namespace Telerik.JustDecompiler.Decompiler
{
	public class StatementDecompilerStep : BaseInstructionVisitor, IDecompilationStep
	{
		private ExpressionDecompilerData expressions;

		private BlockLogicalConstruct logicalTree;

		private ControlFlowGraph theCFG;

		private DecompilationContext context;

		private TypeSystem typeSystem;

		private readonly Dictionary<CFGBlockLogicalConstruct, HashSet<CFGBlockLogicalConstruct>> gotoEndpointToOrigins;

		private readonly Dictionary<CFGBlockLogicalConstruct, HashSet<CFGBlockLogicalConstruct>> gotoOriginsToEndpoints;

		private readonly Dictionary<ILogicalConstruct, string> gotoEndpointConstructToLabel;

		private readonly Dictionary<CFGBlockLogicalConstruct, Dictionary<CFGBlockLogicalConstruct, GotoStatement>> gotoOriginBlockToGoToStatement;

		private readonly Dictionary<ILogicalConstruct, List<Statement>> logicalConstructToStatements;

		private readonly Dictionary<Statement, ILogicalConstruct> statementToLogicalConstruct;

		private readonly Dictionary<CFGBlockLogicalConstruct, CFGBlockLogicalConstruct> breaksOriginToEndPoint;

		private readonly Dictionary<CFGBlockLogicalConstruct, CFGBlockLogicalConstruct> continuesOriginToEndPoint;

		private readonly Dictionary<IBreaksContainer, CFGBlockLogicalConstruct> breaksContainerToBreakEndPoint;

		private List<GotoStatement> contextGotoStatements;

		private Dictionary<string, Statement> contextGotoLabels;

		private uint gotoLabelsCounter;

		private readonly Stack<ILogicalConstruct> parents;

		public StatementDecompilerStep()
		{
			this.gotoEndpointToOrigins = new Dictionary<CFGBlockLogicalConstruct, HashSet<CFGBlockLogicalConstruct>>();
			this.gotoEndpointConstructToLabel = new Dictionary<ILogicalConstruct, string>();
			this.gotoOriginBlockToGoToStatement = new Dictionary<CFGBlockLogicalConstruct, Dictionary<CFGBlockLogicalConstruct, GotoStatement>>();
			this.gotoOriginsToEndpoints = new Dictionary<CFGBlockLogicalConstruct, HashSet<CFGBlockLogicalConstruct>>();
			this.logicalConstructToStatements = new Dictionary<ILogicalConstruct, List<Statement>>();
			this.statementToLogicalConstruct = new Dictionary<Statement, ILogicalConstruct>();
			this.parents = new Stack<ILogicalConstruct>();
			this.breaksOriginToEndPoint = new Dictionary<CFGBlockLogicalConstruct, CFGBlockLogicalConstruct>();
			this.continuesOriginToEndPoint = new Dictionary<CFGBlockLogicalConstruct, CFGBlockLogicalConstruct>();
			this.breaksContainerToBreakEndPoint = new Dictionary<IBreaksContainer, CFGBlockLogicalConstruct>();
		}

		private void AddAndRegisterGoToLabel(ILogicalConstruct gotoConstruct, string theLabel)
		{
			Statement item = null;
			if (!(gotoConstruct is ConditionLogicalConstruct) || (gotoConstruct as ConditionLogicalConstruct).LogicalContainer == null)
			{
				item = (!(gotoConstruct is CFGBlockLogicalConstruct) || !(gotoConstruct.Parent is SwitchLogicalConstruct) ? this.logicalConstructToStatements[gotoConstruct][0] : this.logicalConstructToStatements[gotoConstruct.Parent as ILogicalConstruct][0]);
			}
			else
			{
				ConditionLogicalConstruct conditionLogicalConstruct = gotoConstruct as ConditionLogicalConstruct;
				if (!(conditionLogicalConstruct.LogicalContainer is LoopLogicalConstruct))
				{
					if (!(conditionLogicalConstruct.LogicalContainer is IfLogicalConstruct))
					{
						throw new Exception("Condition containing construct unaccounted for.");
					}
					item = this.logicalConstructToStatements[conditionLogicalConstruct.LogicalContainer][0];
				}
				else
				{
					LoopLogicalConstruct parent = (LoopLogicalConstruct)gotoConstruct.Parent;
					if (parent.LoopType != LoopType.PostTestedLoop)
					{
						if (parent.LoopType != LoopType.PreTestedLoop)
						{
							throw new Exception("Infinite loop with condition encountered.");
						}
						item = this.logicalConstructToStatements[parent][0];
					}
					else
					{
						item = new EmptyStatement();
						((DoWhileStatement)this.logicalConstructToStatements[parent][0]).Body.AddStatement(item);
					}
				}
			}
			item.Label = theLabel;
			this.contextGotoLabels.Add(item.Label, item);
		}

		private void AppendGoToFromConditionStartingAt(List<CFGBlockLogicalConstruct> goToStartPointHosts, CFGBlockLogicalConstruct theSuccessor, List<Statement> results)
		{
			this.AppendGoToStartingAt(goToStartPointHosts[0], results);
			GotoStatement item = this.gotoOriginBlockToGoToStatement[goToStartPointHosts[0]][theSuccessor];
			for (int i = 1; i < goToStartPointHosts.Count; i++)
			{
				if (!this.gotoOriginBlockToGoToStatement.ContainsKey(goToStartPointHosts[i]))
				{
					this.gotoOriginBlockToGoToStatement[goToStartPointHosts[i]] = new Dictionary<CFGBlockLogicalConstruct, GotoStatement>();
				}
				this.gotoOriginBlockToGoToStatement[goToStartPointHosts[i]].Add(theSuccessor, item);
			}
		}

		private void AppendGoToStartingAt(CFGBlockLogicalConstruct goToStartPointHost, List<Statement> results)
		{
			CFGBlockLogicalConstruct cFGBlockLogicalConstruct = this.gotoOriginsToEndpoints[goToStartPointHost].FirstOrDefault<CFGBlockLogicalConstruct>();
			GotoStatement statementToContext = this.ConstructAndRecordGoToStatementToContext("", this.GetJumpingInstructions(goToStartPointHost, cFGBlockLogicalConstruct));
			Dictionary<CFGBlockLogicalConstruct, GotoStatement> cFGBlockLogicalConstructs = new Dictionary<CFGBlockLogicalConstruct, GotoStatement>()
			{
				{ cFGBlockLogicalConstruct, statementToContext }
			};
			this.gotoOriginBlockToGoToStatement.Add(goToStartPointHost, cFGBlockLogicalConstructs);
			results.Add(statementToContext);
		}

		private bool CheckForBreak(ILogicalConstruct theConstruct)
		{
			bool flag;
			if (!(theConstruct is CFGBlockLogicalConstruct))
			{
				if (!(theConstruct is ConditionLogicalConstruct))
				{
					throw new Exception("The given construct cannot be break.");
				}
				HashSet<ISingleEntrySubGraph>.Enumerator enumerator = theConstruct.Children.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						CFGBlockLogicalConstruct current = (CFGBlockLogicalConstruct)enumerator.Current;
						if (!this.breaksOriginToEndPoint.ContainsKey(current))
						{
							continue;
						}
						flag = true;
						return flag;
					}
					return false;
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				return flag;
			}
			else if (this.breaksOriginToEndPoint.ContainsKey(theConstruct as CFGBlockLogicalConstruct))
			{
				return true;
			}
			return false;
		}

		private bool CheckForContinue(ILogicalConstruct theConstruct)
		{
			bool flag;
			if (!(theConstruct is CFGBlockLogicalConstruct))
			{
				if (!(theConstruct is ConditionLogicalConstruct))
				{
					throw new Exception("The given construct cannot be continue.");
				}
				HashSet<ISingleEntrySubGraph>.Enumerator enumerator = theConstruct.Children.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						CFGBlockLogicalConstruct current = (CFGBlockLogicalConstruct)enumerator.Current;
						if (!this.continuesOriginToEndPoint.ContainsKey(current))
						{
							continue;
						}
						flag = true;
						return flag;
					}
					return false;
				}
				finally
				{
					((IDisposable)enumerator).Dispose();
				}
				return flag;
			}
			else if (this.continuesOriginToEndPoint.ContainsKey(theConstruct as CFGBlockLogicalConstruct))
			{
				return true;
			}
			return false;
		}

		private bool CheckForNormalFlowReachability(ILogicalConstruct startPoint, ILogicalConstruct parentConstruct)
		{
			for (ILogicalConstruct i = startPoint; i != parentConstruct; i = i.Parent as ILogicalConstruct)
			{
				if (i.CFGFollowNode != null)
				{
					return false;
				}
			}
			return true;
		}

		private GotoStatement ConstructAndRecordGoToStatementToContext(string label, IEnumerable<Instruction> jumps)
		{
			GotoStatement gotoStatement = new GotoStatement(label, jumps);
			this.contextGotoStatements.Add(gotoStatement);
			return gotoStatement;
		}

		private void DetermineBreaksEndPoint(IBreaksContainer breaksContainer)
		{
			CFGBlockLogicalConstruct breakContainerCFGFollowNode = this.GetBreakContainerCFGFollowNode(breaksContainer);
			if (breakContainerCFGFollowNode != null)
			{
				this.breaksContainerToBreakEndPoint[breaksContainer] = breakContainerCFGFollowNode;
			}
		}

		private bool ExistsStatementForConstruct(ILogicalConstruct theConstruct)
		{
			if (!(theConstruct is ConditionLogicalConstruct) || (theConstruct as ConditionLogicalConstruct).LogicalContainer == null)
			{
				return this.logicalConstructToStatements.ContainsKey(theConstruct);
			}
			return this.logicalConstructToStatements.ContainsKey((theConstruct as ConditionLogicalConstruct).LogicalContainer);
		}

		private void FindAndMarkGoToExits(ILogicalConstruct current, bool gotoReachableConstruct)
		{
			if (current is IBreaksContainer)
			{
				this.DetermineBreaksEndPoint((IBreaksContainer)current);
			}
			if (current is CFGBlockLogicalConstruct)
			{
				this.FindBreak(current as CFGBlockLogicalConstruct);
				this.FindContinue(current as CFGBlockLogicalConstruct);
			}
			foreach (ILogicalConstruct allSuccessor in current.AllSuccessors)
			{
				CFGBlockLogicalConstruct nearestFollowNode = this.GetNearestFollowNode(current);
				if (allSuccessor.FirstBlock != nearestFollowNode)
				{
					this.MarkGoTosIfNotLoopEdge(current, allSuccessor);
				}
				else if (!(current is LoopLogicalConstruct) || (current as LoopLogicalConstruct).LoopType == LoopType.InfiniteLoop || (current as LoopLogicalConstruct).LoopCondition.FalseCFGSuccessor == nearestFollowNode.FirstBlock)
				{
					if (!gotoReachableConstruct)
					{
						continue;
					}
					ILogicalConstruct parent = nearestFollowNode;
					while (parent.Parent != null && ((ILogicalConstruct)parent.Parent).FirstBlock == nearestFollowNode)
					{
						parent = (ILogicalConstruct)parent.Parent;
					}
					if (!this.ExistsStatementForConstruct(parent))
					{
						continue;
					}
					this.MarkGoTosIfNotLoopEdge(current, allSuccessor);
				}
				else
				{
					this.MarkGoTosIfNotLoopEdge(current, allSuccessor);
				}
			}
		}

		private CFGBlockLogicalConstruct FindBreak(CFGBlockLogicalConstruct startPoint)
		{
			CFGBlockLogicalConstruct cFGBlockLogicalConstruct;
			if (this.breaksOriginToEndPoint.ContainsKey(startPoint))
			{
				return this.breaksOriginToEndPoint[startPoint];
			}
			IBreaksContainer innerMostParentOfType = this.GetInnerMostParentOfType<IBreaksContainer>(startPoint);
			if (innerMostParentOfType == null || this.ExistsStatementForConstruct(innerMostParentOfType))
			{
				return null;
			}
			if (!this.breaksContainerToBreakEndPoint.TryGetValue(innerMostParentOfType, out cFGBlockLogicalConstruct) || !startPoint.CFGSuccessors.Contains(cFGBlockLogicalConstruct))
			{
				return null;
			}
			this.breaksOriginToEndPoint[startPoint] = cFGBlockLogicalConstruct;
			return cFGBlockLogicalConstruct;
		}

		private CFGBlockLogicalConstruct FindContinue(CFGBlockLogicalConstruct startPoint)
		{
			if (this.continuesOriginToEndPoint.ContainsKey(startPoint))
			{
				return this.continuesOriginToEndPoint[startPoint];
			}
			LoopLogicalConstruct innerMostParentOfType = this.GetInnerMostParentOfType<LoopLogicalConstruct>(startPoint);
			if (innerMostParentOfType == null || this.ExistsStatementForConstruct(innerMostParentOfType))
			{
				return null;
			}
			if (startPoint.CFGSuccessors.Contains(innerMostParentOfType.LoopContinueEndPoint))
			{
				if (!this.CheckForNormalFlowReachability(startPoint, innerMostParentOfType))
				{
					this.continuesOriginToEndPoint[startPoint] = innerMostParentOfType.LoopContinueEndPoint;
					return innerMostParentOfType.LoopContinueEndPoint;
				}
				IBreaksContainer breaksContainer = this.GetInnerMostParentOfType<IBreaksContainer>(startPoint);
				if (breaksContainer != null)
				{
					LoopLogicalConstruct loopLogicalConstruct = this.GetInnerMostParentOfType<LoopLogicalConstruct>(breaksContainer);
					if (loopLogicalConstruct != null && loopLogicalConstruct == innerMostParentOfType)
					{
						this.continuesOriginToEndPoint[startPoint] = innerMostParentOfType.LoopContinueEndPoint;
						return innerMostParentOfType.LoopContinueEndPoint;
					}
				}
			}
			return null;
		}

		private ILogicalConstruct FindGoToEndpointConstruct(CFGBlockLogicalConstruct gotoEndPoint, CFGBlockLogicalConstruct gotoOrigin)
		{
			if (gotoEndPoint == gotoOrigin)
			{
				if (!(gotoEndPoint.Parent is ConditionLogicalConstruct) || !(gotoEndPoint.Parent.Parent is LoopLogicalConstruct) && !(gotoEndPoint.Parent.Parent is IfLogicalConstruct))
				{
					return (ILogicalConstruct)gotoEndPoint.Parent;
				}
				return (ILogicalConstruct)gotoEndPoint.Parent.Parent;
			}
			ILogicalConstruct logicalConstruct = (ILogicalConstruct)LogicalFlowUtilities.FindFirstCommonParent((IEnumerable<ISingleEntrySubGraph>)(new ISingleEntrySubGraph[] { gotoEndPoint, gotoOrigin }));
			ILogicalConstruct parent = gotoEndPoint;
			while (parent.Parent != logicalConstruct)
			{
				parent = (ILogicalConstruct)parent.Parent;
			}
			if (parent.FirstBlock != gotoEndPoint)
			{
				throw new Exception("GoTo misplaced.");
			}
			if (parent is CaseLogicalConstruct)
			{
				parent = parent.Entry as ILogicalConstruct;
			}
			else if (parent is CFGBlockLogicalConstruct && parent.Parent is SwitchLogicalConstruct)
			{
				parent = parent.Parent as ILogicalConstruct;
			}
			return parent;
		}

		private ILogicalConstruct FindTopMostParentOfBlock(CFGBlockLogicalConstruct gotoEndPoint)
		{
			ILogicalConstruct parent = gotoEndPoint;
			while (parent.Parent != null && ((ILogicalConstruct)parent.Parent).FirstBlock == gotoEndPoint)
			{
				parent = (ILogicalConstruct)parent.Parent;
			}
			if (parent is BlockLogicalConstruct)
			{
				parent = (ILogicalConstruct)parent.Entry;
			}
			return parent;
		}

		private string GenerateGoToLabel()
		{
			uint num = this.gotoLabelsCounter;
			this.gotoLabelsCounter = num + 1;
			return String.Concat("Label", num.ToString());
		}

		private CFGBlockLogicalConstruct GetBreakContainerCFGFollowNode(IBreaksContainer breaksContainer)
		{
			ILogicalConstruct loopCondition;
			ILogicalConstruct parent = breaksContainer;
			while (parent != null && parent.CFGFollowNode == null)
			{
				parent = parent.Parent as ILogicalConstruct;
				if (!(parent is IBreaksContainer))
				{
					continue;
				}
				return null;
			}
			if (parent == null || this.ExistsStatementForConstruct(parent.FollowNode))
			{
				return null;
			}
			if (breaksContainer is SwitchLogicalConstruct)
			{
				return parent.CFGFollowNode;
			}
			LoopLogicalConstruct loopLogicalConstruct = (LoopLogicalConstruct)breaksContainer;
			if (loopLogicalConstruct.LoopType != LoopType.InfiniteLoop)
			{
				loopCondition = loopLogicalConstruct.LoopCondition;
			}
			else
			{
				loopCondition = loopLogicalConstruct.LoopBodyBlock;
			}
			if (!loopCondition.CFGSuccessors.Contains(parent.CFGFollowNode))
			{
				return null;
			}
			return parent.CFGFollowNode;
		}

		private CFGBlockLogicalConstruct GetEffectiveGotoEndPoint(CFGBlockLogicalConstruct gotoEndPointBlock)
		{
			List<Statement> statements;
			CFGBlockLogicalConstruct current = gotoEndPointBlock;
			while (this.logicalConstructToStatements.TryGetValue(current, out statements) && statements.Count == 0)
			{
				IEnumerator<CFGBlockLogicalConstruct> enumerator = current.CFGSuccessors.GetEnumerator();
				using (enumerator)
				{
					if (!enumerator.MoveNext())
					{
						throw new Exception("End block with no statements reached.");
					}
					current = enumerator.Current;
					if (enumerator.MoveNext())
					{
						throw new Exception("No statements generated for multi exit block");
					}
				}
			}
			return current;
		}

		private T GetInnerMostParentOfType<T>()
		where T : class
		{
			ILogicalConstruct[] array = this.parents.ToArray();
			for (int i = 0; i < (int)array.Length; i++)
			{
				ILogicalConstruct logicalConstruct = array[i];
				if (logicalConstruct is T)
				{
					return (T)(logicalConstruct as T);
				}
			}
			return default(T);
		}

		private T GetInnerMostParentOfType<T>(ILogicalConstruct startNode)
		where T : class
		{
			for (ILogicalConstruct i = startNode.Parent as ILogicalConstruct; i != null; i = i.Parent as ILogicalConstruct)
			{
				if (i is T)
				{
					return (T)(i as T);
				}
			}
			return default(T);
		}

		private List<Instruction> GetJumpingInstructions(ILogicalConstruct from, CFGBlockLogicalConstruct to)
		{
			if (!(from is CFGBlockLogicalConstruct) || from.Parent is ConditionLogicalConstruct || !this.IsUnconditionalJump((from as CFGBlockLogicalConstruct).TheBlock.Last))
			{
				return null;
			}
			List<Instruction> instructions = new List<Instruction>();
			foreach (CFGBlockLogicalConstruct cFGBlock in from.CFGBlocks)
			{
				if (!cFGBlock.CFGSuccessors.Contains(to))
				{
					continue;
				}
				instructions.Add(cFGBlock.TheBlock.Last);
			}
			return instructions;
		}

		private CFGBlockLogicalConstruct GetNearestFollowNode(ILogicalConstruct current)
		{
			if (current.CFGFollowNode != null)
			{
				return current.CFGFollowNode;
			}
			ILogicalConstruct[] array = this.parents.ToArray();
			for (int i = 0; i < (int)array.Length; i++)
			{
				ILogicalConstruct logicalConstruct = array[i];
				if (logicalConstruct.CFGFollowNode != null)
				{
					return logicalConstruct.CFGFollowNode;
				}
			}
			return null;
		}

		private List<CFGBlockLogicalConstruct> HostsGoToStartPoint(ConditionLogicalConstruct condition)
		{
			List<CFGBlockLogicalConstruct> cFGBlockLogicalConstructs = new List<CFGBlockLogicalConstruct>();
			foreach (CFGBlockLogicalConstruct cFGBlock in condition.CFGBlocks)
			{
				if (!this.gotoOriginsToEndpoints.ContainsKey(cFGBlock))
				{
					continue;
				}
				cFGBlockLogicalConstructs.Add(cFGBlock);
			}
			return cFGBlockLogicalConstructs;
		}

		private BlockStatement InsertGotoEndpoints(BlockStatement body)
		{
			bool count = false;
			do
			{
				HashSet<ILogicalConstruct> logicalConstructs = new HashSet<ILogicalConstruct>();
				foreach (CFGBlockLogicalConstruct key in this.gotoEndpointToOrigins.Keys)
				{
					ILogicalConstruct logicalConstruct = this.FindTopMostParentOfBlock(key);
					if (this.ExistsStatementForConstruct(logicalConstruct))
					{
						continue;
					}
					logicalConstructs.Add(logicalConstruct);
				}
				count = logicalConstructs.Count > 0;
				foreach (ILogicalConstruct logicalConstruct1 in logicalConstructs)
				{
					if (this.ExistsStatementForConstruct(logicalConstruct1))
					{
						continue;
					}
					foreach (Statement onlyReachableStatement in this.TraverseGoToOnlyReachableStatements(logicalConstruct1))
					{
						body.AddStatement(onlyReachableStatement);
					}
				}
			}
			while (count);
			foreach (CFGBlockLogicalConstruct item in this.gotoEndpointToOrigins.Keys)
			{
				CFGBlockLogicalConstruct effectiveGotoEndPoint = this.GetEffectiveGotoEndPoint(item);
				foreach (CFGBlockLogicalConstruct cFGBlockLogicalConstruct in this.gotoEndpointToOrigins[item])
				{
					ILogicalConstruct endpointConstruct = this.FindGoToEndpointConstruct(effectiveGotoEndPoint, cFGBlockLogicalConstruct);
					if (!this.gotoEndpointConstructToLabel.ContainsKey(endpointConstruct))
					{
						string label = this.GenerateGoToLabel();
						this.AddAndRegisterGoToLabel(endpointConstruct, label);
						this.gotoEndpointConstructToLabel[endpointConstruct] = label;
					}
					this.gotoOriginBlockToGoToStatement[cFGBlockLogicalConstruct][item].TargetLabel = this.gotoEndpointConstructToLabel[endpointConstruct];
				}
			}
			return body;
		}

		private bool IsLoopCondition(CFGBlockLogicalConstruct cfgBlock)
		{
			if (!(cfgBlock.Parent is ConditionLogicalConstruct))
			{
				return false;
			}
			return (cfgBlock.Parent as ConditionLogicalConstruct).LogicalContainer is LoopLogicalConstruct;
		}

		private bool IsUnconditionalJump(Instruction instruction)
		{
			Code code = instruction.get_OpCode().get_Code();
			if (code == 55)
			{
				return true;
			}
			return code == 42;
		}

		private void MarkGoTosIfNotLoopEdge(ILogicalConstruct start, ILogicalConstruct end)
		{
			ILogicalConstruct logicalConstruct;
			HashSet<CFGBlockLogicalConstruct> cFGBlockLogicalConstructs;
			CFGBlockLogicalConstruct firstBlock = end.FirstBlock;
			LoopLogicalConstruct innerMostParentOfType = this.GetInnerMostParentOfType<LoopLogicalConstruct>();
			HashSet<CFGBlockLogicalConstruct> cFGBlocks = start.CFGBlocks;
			foreach (CFGBlockLogicalConstruct cFGPredecessor in firstBlock.CFGPredecessors)
			{
				if (!cFGBlocks.Contains(cFGPredecessor))
				{
					continue;
				}
				logicalConstruct = (!this.IsLoopCondition(cFGPredecessor) ? this.GetInnerMostParentOfType<LoopLogicalConstruct>(cFGPredecessor) : this.GetInnerMostParentOfType<LoopLogicalConstruct>(cFGPredecessor.Parent.Parent as ILogicalConstruct));
				ILogicalConstruct innerMostParentOfType1 = this.GetInnerMostParentOfType<IBreaksContainer>(cFGPredecessor);
				CFGBlockLogicalConstruct cFGBlockLogicalConstruct = this.FindBreak(cFGPredecessor);
				CFGBlockLogicalConstruct cFGBlockLogicalConstruct1 = this.FindContinue(cFGPredecessor);
				if (cFGBlockLogicalConstruct == firstBlock && !this.ExistsStatementForConstruct(innerMostParentOfType1) || cFGBlockLogicalConstruct1 == firstBlock && !this.ExistsStatementForConstruct(logicalConstruct) || innerMostParentOfType != null && logicalConstruct == innerMostParentOfType && !this.ExistsStatementForConstruct(innerMostParentOfType) && innerMostParentOfType.LoopContinueEndPoint == firstBlock && this.CheckForNormalFlowReachability(cFGPredecessor, innerMostParentOfType))
				{
					continue;
				}
				if ((!this.gotoOriginsToEndpoints.TryGetValue(cFGPredecessor, out cFGBlockLogicalConstructs) ? false : cFGBlockLogicalConstructs.Contains(firstBlock)))
				{
					continue;
				}
				if (!this.gotoEndpointToOrigins.ContainsKey(firstBlock))
				{
					this.gotoEndpointToOrigins.Add(firstBlock, new HashSet<CFGBlockLogicalConstruct>());
				}
				if (!this.gotoEndpointToOrigins[firstBlock].Contains(cFGPredecessor))
				{
					this.gotoEndpointToOrigins[firstBlock].Add(cFGPredecessor);
				}
				if (this.gotoOriginsToEndpoints.ContainsKey(cFGPredecessor))
				{
					this.gotoOriginsToEndpoints[cFGPredecessor].Add(firstBlock);
				}
				else
				{
					this.gotoOriginsToEndpoints.Add(cFGPredecessor, new HashSet<CFGBlockLogicalConstruct>(new CFGBlockLogicalConstruct[] { firstBlock }));
				}
			}
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.context = context;
			this.contextGotoLabels = context.MethodContext.GotoLabels;
			this.contextGotoStatements = context.MethodContext.GotoStatements;
			this.theCFG = context.MethodContext.ControlFlowGraph;
			this.typeSystem = context.MethodContext.Method.get_Module().get_TypeSystem();
			this.logicalTree = context.MethodContext.LogicalConstructsTree;
			this.expressions = context.MethodContext.Expressions;
			body = (BlockStatement)this.ProcessLogicalConstruct(this.logicalTree, false)[0];
			body = this.InsertGotoEndpoints(body);
			context.MethodContext.StatementToLogicalConstruct = this.statementToLogicalConstruct;
			context.MethodContext.LogicalConstructToStatements = this.logicalConstructToStatements;
			return body;
		}

		private BlockStatement ProcessBlockLogicalConstruct(ILogicalConstruct theConstruct, bool gotoReachableConstruct)
		{
			BlockStatement blockStatement = new BlockStatement();
			for (ILogicalConstruct i = (ILogicalConstruct)theConstruct.Entry; i != null; i = i.FollowNode)
			{
				foreach (Statement statement in this.ProcessLogicalConstruct(i, gotoReachableConstruct))
				{
					blockStatement.AddStatement(statement);
				}
			}
			return blockStatement;
		}

		private void ProcessCfgBlockLogicalConstruct(CFGBlockLogicalConstruct theBlock, List<Statement> results, bool closestBreakContainerExists, bool closestLoopParentExists)
		{
			// 
			// Current member / type: System.Void Telerik.JustDecompiler.Decompiler.StatementDecompilerStep::ProcessCfgBlockLogicalConstruct(Telerik.JustDecompiler.Decompiler.LogicFlow.CFGBlockLogicalConstruct,System.Collections.Generic.List`1<Telerik.JustDecompiler.Ast.Statements.Statement>,System.Boolean,System.Boolean)
			// File path: C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\Decompiler.Tests\bin\Release\netcoreapp2.1\Integration\Actual\JustDecompiler.NetStandard.dll
			// 
			// Product version: 0.0.0.0
			// Exception in: System.Void ProcessCfgBlockLogicalConstruct(Telerik.JustDecompiler.Decompiler.LogicFlow.CFGBlockLogicalConstruct,System.Collections.Generic.List<Telerik.JustDecompiler.Ast.Statements.Statement>,System.Boolean,System.Boolean)
			// 
			// Specified argument was out of the range of valid values.
			// Parameter name: Target of array indexer expression is not an array.
			//    at Telerik.JustDecompiler.Ast.Expressions.ArrayIndexerExpression.get_ExpressionType() in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Ast\Expressions\ArrayIndexerExpression.cs:line 129
			//    at Telerik.JustDecompiler.Ast.Expressions.BinaryExpression.UpdateType() in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Ast\Expressions\BinaryExpression.cs:line 228
			//    at Telerik.JustDecompiler.Steps.FixBinaryExpressionsStep.VisitBinaryExpression(BinaryExpression expression) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Steps\FixBinaryExpressionsStep.cs:line 74
			//    at Telerik.JustDecompiler.Ast.BaseCodeTransformer.Visit(ICodeNode node) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Ast\BaseCodeTransformer.cs:line 276
			//    at Telerik.JustDecompiler.Steps.FixBinaryExpressionsStep.Process(DecompilationContext context, BlockStatement body) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Steps\FixBinaryExpressionsStep.cs:line 44
			//    at Telerik.JustDecompiler.Decompiler.ExpressionDecompilerStep.Process(DecompilationContext theContext, BlockStatement body) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\ExpressionDecompilerStep.cs:line 93
			//    at Telerik.JustDecompiler.Decompiler.DecompilationPipeline.RunInternal(MethodBody body, BlockStatement block, ILanguage language) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\DecompilationPipeline.cs:line 81
			//    at Telerik.JustDecompiler.Decompiler.DecompilationPipeline.Run(MethodBody body, ILanguage language) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\DecompilationPipeline.cs:line 70
			//    at Telerik.JustDecompiler.Decompiler.Extensions.Decompile(MethodBody body, ILanguage language, DecompilationContext& context, TypeSpecificContext typeContext) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\Extensions.cs:line 61
			//    at Telerik.JustDecompiler.Decompiler.WriterContextServices.BaseWriterContextService.DecompileMethod(ILanguage language, MethodDefinition method, TypeSpecificContext typeContext) in C:\Users\CodeMerx\Work\CodemerxDecompileEngine\CodemerxDecompileEngine\JustDecompiler.Shared\Decompiler\WriterContextServices\BaseWriterContextService.cs:line 117
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		private IfStatement ProcessConditionLogicalConstruct(ConditionLogicalConstruct clc, bool closestBreakContainerExists, IBreaksContainer closestBreakContainerParent, bool closestLoopParentExists)
		{
			bool flag;
			BlockStatement blockStatement = new BlockStatement();
			BlockStatement blockStatement1 = new BlockStatement();
			if (!closestBreakContainerExists && this.CheckForBreak(clc))
			{
				CFGBlockLogicalConstruct item = this.breaksContainerToBreakEndPoint[closestBreakContainerParent];
				List<Instruction> jumpingInstructions = this.GetJumpingInstructions(clc, item);
				if (clc.TrueCFGSuccessor != item)
				{
					if (clc.FalseCFGSuccessor != item)
					{
						throw new Exception("Incorrect mark as break child!");
					}
					blockStatement1.AddStatement(new BreakStatement(jumpingInstructions));
				}
				else
				{
					blockStatement.AddStatement(new BreakStatement(jumpingInstructions));
				}
			}
			if (!closestLoopParentExists && this.CheckForContinue(clc))
			{
				CFGBlockLogicalConstruct loopContinueEndPoint = this.GetInnerMostParentOfType<LoopLogicalConstruct>().LoopContinueEndPoint;
				if (clc.TrueCFGSuccessor != loopContinueEndPoint || blockStatement.Statements.Count != 0)
				{
					if (clc.FalseCFGSuccessor != loopContinueEndPoint || blockStatement1.Statements.Count != 0)
					{
						throw new Exception("Incorrect mark as continue child!");
					}
					blockStatement1.AddStatement(new ContinueStatement(this.GetJumpingInstructions(clc, loopContinueEndPoint)));
				}
				else
				{
					blockStatement.AddStatement(new ContinueStatement(this.GetJumpingInstructions(clc, loopContinueEndPoint)));
				}
			}
			flag = (!this.gotoEndpointToOrigins.ContainsKey(clc.TrueCFGSuccessor) ? false : this.TryCreateGoTosForConditinalConstructSuccessor(blockStatement, clc, clc.TrueCFGSuccessor));
			bool flag1 = (!this.gotoEndpointToOrigins.ContainsKey(clc.FalseCFGSuccessor) ? false : this.TryCreateGoTosForConditinalConstructSuccessor(blockStatement1, clc, clc.FalseCFGSuccessor));
			if (!flag && !flag1 && blockStatement.Statements.Count == 0 && blockStatement1.Statements.Count == 0)
			{
				throw new Exception("Orphaned condition not properly marked as goto!");
			}
			if (blockStatement.Statements.Count == 0)
			{
				BlockStatement blockStatement2 = blockStatement;
				blockStatement = blockStatement1;
				blockStatement1 = blockStatement2;
				clc.Negate(this.typeSystem);
			}
			if (blockStatement1.Statements.Count == 0)
			{
				blockStatement1 = null;
			}
			return new IfStatement(clc.ConditionExpression, blockStatement, blockStatement1);
		}

		private TryStatement ProcessExceptionHandlingLogicalConstruct(ExceptionHandlingLogicalConstruct exceptionHandlingConstruct, bool gotoReachableConstruct)
		{
			VariableDefinition variableDefinition;
			VariableDeclarationExpression variableDeclarationExpression;
			BlockStatement item = (BlockStatement)this.ProcessLogicalConstruct(exceptionHandlingConstruct.Try, gotoReachableConstruct)[0];
			if (exceptionHandlingConstruct is TryFinallyLogicalConstruct)
			{
				TryFinallyLogicalConstruct tryFinallyLogicalConstruct = exceptionHandlingConstruct as TryFinallyLogicalConstruct;
				FinallyClause finallyClause = new FinallyClause((BlockStatement)this.ProcessLogicalConstruct(tryFinallyLogicalConstruct.Finally, gotoReachableConstruct)[0], null);
				return new TryStatement(item, null, finallyClause);
			}
			if (exceptionHandlingConstruct is TryFaultLogicalConstruct)
			{
				TryFaultLogicalConstruct tryFaultLogicalConstruct = exceptionHandlingConstruct as TryFaultLogicalConstruct;
				BlockStatement blockStatement = (BlockStatement)this.ProcessLogicalConstruct(tryFaultLogicalConstruct.Fault, gotoReachableConstruct)[0];
				return new TryStatement(item, blockStatement, null);
			}
			TryStatement tryStatement = new TryStatement()
			{
				Try = item
			};
			IFilteringExceptionHandler[] handlers = (exceptionHandlingConstruct as TryCatchFilterLogicalConstruct).Handlers;
			for (int i = 0; i < (int)handlers.Length; i++)
			{
				IFilteringExceptionHandler filteringExceptionHandler = handlers[i];
				if (!(filteringExceptionHandler is ExceptionHandlingBlockCatch))
				{
					ExceptionHandlingBlockFilter exceptionHandlingBlockFilter = filteringExceptionHandler as ExceptionHandlingBlockFilter;
					BlockStatement item1 = (BlockStatement)this.ProcessLogicalConstruct(exceptionHandlingBlockFilter.Filter, gotoReachableConstruct)[0];
					BlockStatement blockStatement1 = (BlockStatement)this.ProcessLogicalConstruct(exceptionHandlingBlockFilter.Handler, gotoReachableConstruct)[0];
					tryStatement.AddToCatchClauses(new CatchClause(blockStatement1, null, null, item1));
				}
				else
				{
					ExceptionHandlingBlockCatch exceptionHandlingBlockCatch = filteringExceptionHandler as ExceptionHandlingBlockCatch;
					BlockStatement item2 = (BlockStatement)this.ProcessLogicalConstruct(exceptionHandlingBlockCatch, gotoReachableConstruct)[0];
					if (this.context.MethodContext.StackData.ExceptionHandlerStartToExceptionVariableMap.TryGetValue(exceptionHandlingBlockCatch.FirstBlock.TheBlock.First.get_Offset(), out variableDefinition))
					{
						variableDeclarationExpression = new VariableDeclarationExpression(variableDefinition, null);
					}
					else
					{
						variableDeclarationExpression = null;
					}
					VariableDeclarationExpression variableDeclarationExpression1 = variableDeclarationExpression;
					tryStatement.AddToCatchClauses(new CatchClause(item2, exceptionHandlingBlockCatch.CatchType, variableDeclarationExpression1, null));
				}
			}
			return tryStatement;
		}

		private IfStatement ProcessIfLogicalConstruct(IfLogicalConstruct theIf, bool gotoReachableConstruct)
		{
			BlockStatement item = (BlockStatement)this.ProcessLogicalConstruct(theIf.Then, gotoReachableConstruct)[0];
			BlockStatement blockStatement = null;
			if (theIf.Else != null)
			{
				blockStatement = (BlockStatement)this.ProcessLogicalConstruct(theIf.Else, gotoReachableConstruct)[0];
			}
			if (blockStatement != null && blockStatement.Statements.Count == 0)
			{
				blockStatement = null;
			}
			if (blockStatement != null && item.Statements.Count == 0)
			{
				theIf.Negate(this.typeSystem);
				item = blockStatement;
				blockStatement = null;
			}
			IfStatement ifStatement = new IfStatement(theIf.Condition.ConditionExpression, item, blockStatement);
			List<CFGBlockLogicalConstruct> startPoint = this.HostsGoToStartPoint(theIf.Condition);
			if (startPoint.Count != 0)
			{
				if (theIf.Else != null)
				{
					throw new Exception("Malformed IF statement.");
				}
				List<Statement> statements = new List<Statement>();
				this.AppendGoToFromConditionStartingAt(startPoint, theIf.Condition.FalseCFGSuccessor, statements);
				ifStatement.Else = new BlockStatement();
				ifStatement.Else.AddStatement(statements[0]);
			}
			return ifStatement;
		}

		private List<Statement> ProcessLogicalConstruct(ILogicalConstruct theConstruct, bool gotoReachableConstruct)
		{
			if (this.logicalConstructToStatements.ContainsKey(theConstruct))
			{
				return this.logicalConstructToStatements[theConstruct];
			}
			List<Statement> statements = new List<Statement>();
			this.FindAndMarkGoToExits(theConstruct, gotoReachableConstruct);
			this.parents.Push(theConstruct);
			if (!(theConstruct is BlockLogicalConstruct))
			{
				LoopLogicalConstruct innerMostParentOfType = this.GetInnerMostParentOfType<LoopLogicalConstruct>(theConstruct);
				bool flag = (innerMostParentOfType == null ? false : this.ExistsStatementForConstruct(innerMostParentOfType));
				IBreaksContainer breaksContainer = this.GetInnerMostParentOfType<IBreaksContainer>(theConstruct);
				bool flag1 = (breaksContainer == null ? false : this.ExistsStatementForConstruct(breaksContainer));
				if (theConstruct is ExceptionHandlingLogicalConstruct)
				{
					statements.Add(this.ProcessExceptionHandlingLogicalConstruct(theConstruct as ExceptionHandlingLogicalConstruct, gotoReachableConstruct));
				}
				else if (theConstruct is IfLogicalConstruct)
				{
					statements.Add(this.ProcessIfLogicalConstruct(theConstruct as IfLogicalConstruct, gotoReachableConstruct));
				}
				else if (theConstruct is LoopLogicalConstruct)
				{
					this.ProcessLoopLogicalConstruct(theConstruct as LoopLogicalConstruct, gotoReachableConstruct, statements);
				}
				else if (theConstruct is SwitchLogicalConstruct)
				{
					statements.Add(this.ProcessSwitchLogicalConstruct(theConstruct as SwitchLogicalConstruct, gotoReachableConstruct));
				}
				else if (theConstruct is CFGBlockLogicalConstruct)
				{
					this.ProcessCfgBlockLogicalConstruct(theConstruct as CFGBlockLogicalConstruct, statements, flag1, flag);
				}
				else if (theConstruct is ConditionLogicalConstruct)
				{
					statements.Add(this.ProcessConditionLogicalConstruct(theConstruct as ConditionLogicalConstruct, flag1, breaksContainer, flag));
				}
			}
			else
			{
				statements.Add(this.ProcessBlockLogicalConstruct(theConstruct, gotoReachableConstruct));
			}
			this.parents.Pop();
			this.logicalConstructToStatements.Add(theConstruct, statements);
			foreach (Statement statement in statements)
			{
				this.statementToLogicalConstruct[statement] = theConstruct;
			}
			return statements;
		}

		private void ProcessLoopLogicalConstruct(LoopLogicalConstruct theLogicalLoop, bool gotoReachableConstruct, List<Statement> results)
		{
			BlockStatement blockStatement;
			Expression literalExpression;
			blockStatement = (theLogicalLoop.LoopBodyBlock == null ? new BlockStatement() : (BlockStatement)this.ProcessLogicalConstruct(theLogicalLoop.LoopBodyBlock, gotoReachableConstruct)[0]);
			if (theLogicalLoop.LoopCondition == null)
			{
				literalExpression = new LiteralExpression(true, this.typeSystem, null);
			}
			else
			{
				literalExpression = theLogicalLoop.LoopCondition.ConditionExpression;
			}
			if (theLogicalLoop.LoopType == LoopType.PreTestedLoop || theLogicalLoop.LoopType == LoopType.InfiniteLoop)
			{
				results.Add(new WhileStatement(literalExpression, blockStatement));
			}
			else if (theLogicalLoop.LoopType == LoopType.PostTestedLoop)
			{
				results.Add(new DoWhileStatement(literalExpression, blockStatement));
			}
			if (theLogicalLoop.LoopCondition != null)
			{
				List<CFGBlockLogicalConstruct> startPoint = this.HostsGoToStartPoint(theLogicalLoop.LoopCondition);
				if (startPoint.Count != 0)
				{
					this.AppendGoToFromConditionStartingAt(startPoint, theLogicalLoop.LoopCondition.FalseCFGSuccessor, results);
				}
			}
		}

		private SwitchStatement ProcessSwitchLogicalConstruct(SwitchLogicalConstruct theLogicalSwitch, bool gotoReachableConstruct)
		{
			List<int> key;
			BlockStatement blockStatement;
			KeyValuePair<List<int>, CFGBlockLogicalConstruct> item;
			HashSet<CFGBlockLogicalConstruct> cFGBlockLogicalConstructs;
			CFGBlockLogicalConstruct cFGBlockLogicalConstruct;
			HashSet<CFGBlockLogicalConstruct> cFGBlockLogicalConstructs1;
			CFGBlockLogicalConstruct cFGBlockLogicalConstruct1;
			SwitchStatement switchStatement = new SwitchStatement(theLogicalSwitch.SwitchConditionExpression, (theLogicalSwitch.Entry as ILogicalConstruct).FirstBlock.TheBlock.Last);
			Dictionary<CFGBlockLogicalConstruct, GotoStatement> cFGBlockLogicalConstructs2 = new Dictionary<CFGBlockLogicalConstruct, GotoStatement>();
			this.gotoOriginBlockToGoToStatement.Add(theLogicalSwitch.FirstBlock, cFGBlockLogicalConstructs2);
			int num = 0;
			int num1 = 0;
			while (num != (int)theLogicalSwitch.ConditionCases.Length || num1 != theLogicalSwitch.NonDominatedCFGSuccessors.Count)
			{
				CaseLogicalConstruct conditionCases = null;
				CFGBlockLogicalConstruct value = null;
				if (num < (int)theLogicalSwitch.ConditionCases.Length && num1 < theLogicalSwitch.NonDominatedCFGSuccessors.Count)
				{
					if (theLogicalSwitch.ConditionCases[num].CaseNumbers[0] >= theLogicalSwitch.NonDominatedCFGSuccessors[num1].Key[0])
					{
						item = theLogicalSwitch.NonDominatedCFGSuccessors[num1];
						key = item.Key;
						item = theLogicalSwitch.NonDominatedCFGSuccessors[num1];
						value = item.Value;
						num1++;
					}
					else
					{
						conditionCases = theLogicalSwitch.ConditionCases[num];
						key = conditionCases.CaseNumbers;
						num++;
					}
				}
				else if (num != (int)theLogicalSwitch.ConditionCases.Length)
				{
					conditionCases = theLogicalSwitch.ConditionCases[num];
					key = conditionCases.CaseNumbers;
					num++;
				}
				else
				{
					item = theLogicalSwitch.NonDominatedCFGSuccessors[num1];
					key = item.Key;
					item = theLogicalSwitch.NonDominatedCFGSuccessors[num1];
					value = item.Value;
					num1++;
				}
				for (int i = 0; i < key.Count - 1; i++)
				{
					int item1 = key[i];
					switchStatement.AddCase(new ConditionCase()
					{
						Condition = new LiteralExpression((object)item1, this.typeSystem, null)
					});
				}
				LiteralExpression literalExpression = new LiteralExpression((object)key[key.Count - 1], this.typeSystem, null)
				{
					ExpressionType = this.context.MethodContext.Method.get_Module().get_TypeSystem().get_Int32()
				};
				if (conditionCases == null)
				{
					blockStatement = new BlockStatement();
					if (this.continuesOriginToEndPoint.TryGetValue(theLogicalSwitch.FirstBlock, out cFGBlockLogicalConstruct) && cFGBlockLogicalConstruct == value)
					{
						blockStatement.AddStatement(new ContinueStatement(null));
					}
					else if (!this.gotoEndpointToOrigins.TryGetValue(value, out cFGBlockLogicalConstructs) || !cFGBlockLogicalConstructs.Contains(theLogicalSwitch.FirstBlock))
					{
						blockStatement.AddStatement(new BreakStatement(null));
					}
					else
					{
						GotoStatement statementToContext = this.ConstructAndRecordGoToStatementToContext("", this.GetJumpingInstructions(theLogicalSwitch.FirstBlock, value));
						cFGBlockLogicalConstructs2.Add(value, statementToContext);
						blockStatement.AddStatement(statementToContext);
					}
				}
				else
				{
					blockStatement = (BlockStatement)this.ProcessLogicalConstruct(conditionCases, gotoReachableConstruct)[0];
				}
				if (SwitchHelpers.BlockHasFallThroughSemantics(blockStatement))
				{
					blockStatement.AddStatement(new BreakSwitchCaseStatement());
				}
				switchStatement.AddCase(new ConditionCase(literalExpression, blockStatement));
			}
			if (theLogicalSwitch.DefaultCase == null)
			{
				BlockStatement blockStatement1 = new BlockStatement();
				if (this.continuesOriginToEndPoint.TryGetValue(theLogicalSwitch.FirstBlock, out cFGBlockLogicalConstruct1) && cFGBlockLogicalConstruct1 == theLogicalSwitch.DefaultCFGSuccessor)
				{
					blockStatement1.AddStatement(new ContinueStatement(null));
				}
				else if (this.gotoEndpointToOrigins.TryGetValue(theLogicalSwitch.DefaultCFGSuccessor, out cFGBlockLogicalConstructs1) && cFGBlockLogicalConstructs1.Contains(theLogicalSwitch.FirstBlock))
				{
					GotoStatement gotoStatement = this.ConstructAndRecordGoToStatementToContext("", this.GetJumpingInstructions(theLogicalSwitch.FirstBlock, theLogicalSwitch.DefaultCFGSuccessor));
					cFGBlockLogicalConstructs2.Add(theLogicalSwitch.DefaultCFGSuccessor, gotoStatement);
					blockStatement1.AddStatement(gotoStatement);
				}
				if (blockStatement1.Statements.Count > 0)
				{
					switchStatement.AddCase(new DefaultCase(blockStatement1));
				}
			}
			else
			{
				BlockStatement item2 = (BlockStatement)this.ProcessLogicalConstruct(theLogicalSwitch.DefaultCase, gotoReachableConstruct)[0];
				if (this.gotoEndpointToOrigins.ContainsKey(theLogicalSwitch.DefaultCase.FirstBlock) || item2.Statements.Count > 1 || item2.Statements.Count == 1 && item2.Statements[0].CodeNodeType != CodeNodeType.BreakStatement)
				{
					if (SwitchHelpers.BlockHasFallThroughSemantics(item2))
					{
						item2.AddStatement(new BreakSwitchCaseStatement());
					}
					switchStatement.AddCase(new DefaultCase(item2));
				}
			}
			return switchStatement;
		}

		private List<Statement> TraverseGoToOnlyReachableStatements(ILogicalConstruct start)
		{
			List<Statement> statements = new List<Statement>();
			ILogicalConstruct followNode = start;
			do
			{
				statements.AddRange(this.ProcessLogicalConstruct(followNode, true));
				if (followNode.FollowNode != null && this.ExistsStatementForConstruct(followNode.FollowNode))
				{
					break;
				}
				followNode = followNode.FollowNode;
			}
			while (followNode != null);
			return statements;
		}

		private bool TryCreateGoTosForConditinalConstructSuccessor(BlockStatement theBlock, ConditionLogicalConstruct clc, CFGBlockLogicalConstruct theSuccessor)
		{
			GotoStatement statementToContext = null;
			bool flag = false;
			foreach (CFGBlockLogicalConstruct cFGPredecessor in theSuccessor.CFGPredecessors)
			{
				if (!clc.CFGBlocks.Contains(cFGPredecessor) || !this.gotoEndpointToOrigins[theSuccessor].Contains(cFGPredecessor))
				{
					continue;
				}
				if (statementToContext == null)
				{
					statementToContext = this.ConstructAndRecordGoToStatementToContext("", this.GetJumpingInstructions(cFGPredecessor, theSuccessor));
					theBlock.AddStatement(statementToContext);
				}
				if (!this.gotoOriginBlockToGoToStatement.ContainsKey(cFGPredecessor))
				{
					this.gotoOriginBlockToGoToStatement.Add(cFGPredecessor, new Dictionary<CFGBlockLogicalConstruct, GotoStatement>());
				}
				this.gotoOriginBlockToGoToStatement[cFGPredecessor].Add(theSuccessor, statementToContext);
				flag = true;
			}
			return flag;
		}
	}
}