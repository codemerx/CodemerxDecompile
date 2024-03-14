using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler.LogicFlow;

namespace Telerik.JustDecompiler.Decompiler.LogicFlow.Conditions
{
	internal class ConditionBuilder
	{
		private readonly LogicalFlowBuilderContext logicalBuilderContext;

		private readonly TypeReference booleanTypeReference;

		private readonly TypeSystem typeSystem;

		public ConditionBuilder(LogicalFlowBuilderContext logicalBuilderContext, TypeSystem typeSystem)
		{
			this.typeSystem = typeSystem;
			this.logicalBuilderContext = logicalBuilderContext;
			this.booleanTypeReference = logicalBuilderContext.CFG.MethodBody.get_Method().get_Module().get_TypeSystem().get_Boolean();
		}

		private bool ArePredecessorsLegal(ILogicalConstruct node, HashSet<ILogicalConstruct> allowedPredecessors)
		{
			bool flag;
			HashSet<ISingleEntrySubGraph>.Enumerator enumerator = node.SameParentPredecessors.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					if (allowedPredecessors.Contains((ILogicalConstruct)enumerator.Current))
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

		public void BuildConstructs(ILogicalConstruct theConstruct)
		{
			this.CreateSimpleConditions();
			this.CreateComplexConditions(theConstruct);
		}

		private bool CanBePartOfComplexCondition(ILogicalConstruct node, HashSet<ILogicalConstruct> nodesInCondition, CFGBlockLogicalConstruct commonSuccessor)
		{
			if (node == null || !(node is ConditionLogicalConstruct) || !this.ArePredecessorsLegal(node, nodesInCondition) || !node.CFGSuccessors.Contains(commonSuccessor))
			{
				return false;
			}
			return !nodesInCondition.Contains(node);
		}

		private ConditionLogicalConstruct CreateComplexCondition(ConditionLogicalConstruct conditionNode)
		{
			ConditionLogicalConstruct conditionLogicalConstruct;
			BinaryOperator binaryOperator;
			Expression conditionExpression = conditionNode.ConditionExpression;
			HashSet<ILogicalConstruct> logicalConstructs = new HashSet<ILogicalConstruct>();
			ConditionLogicalConstruct conditionLogicalConstruct1 = conditionNode;
			logicalConstructs.Add(conditionLogicalConstruct1);
			ILogicalConstruct trueSuccessor = conditionLogicalConstruct1.TrueSuccessor;
			ILogicalConstruct falseSuccessor = conditionLogicalConstruct1.FalseSuccessor;
			while (true)
			{
				if (!this.CanBePartOfComplexCondition(trueSuccessor, logicalConstructs, conditionLogicalConstruct1.FalseCFGSuccessor))
				{
					if (!this.CanBePartOfComplexCondition(falseSuccessor, logicalConstructs, conditionLogicalConstruct1.TrueCFGSuccessor))
					{
						break;
					}
					conditionLogicalConstruct = falseSuccessor as ConditionLogicalConstruct;
					if (conditionLogicalConstruct.TrueSuccessor != trueSuccessor)
					{
						conditionLogicalConstruct.Negate(this.typeSystem);
					}
					binaryOperator = BinaryOperator.LogicalOr;
				}
				else
				{
					conditionLogicalConstruct = trueSuccessor as ConditionLogicalConstruct;
					if (conditionLogicalConstruct.FalseSuccessor != falseSuccessor)
					{
						conditionLogicalConstruct.Negate(this.typeSystem);
					}
					binaryOperator = BinaryOperator.LogicalAnd;
				}
				conditionExpression = new BinaryExpression(binaryOperator, conditionExpression, conditionLogicalConstruct.ConditionExpression, this.typeSystem, null, false)
				{
					ExpressionType = this.booleanTypeReference
				};
				conditionLogicalConstruct1 = conditionLogicalConstruct;
				trueSuccessor = conditionLogicalConstruct1.TrueSuccessor;
				falseSuccessor = conditionLogicalConstruct1.FalseSuccessor;
				logicalConstructs.Add(conditionLogicalConstruct1);
			}
			if (logicalConstructs.Count == 1)
			{
				return conditionNode;
			}
			HashSet<ConditionLogicalConstruct> conditionLogicalConstructs = new HashSet<ConditionLogicalConstruct>();
			foreach (ConditionLogicalConstruct logicalConstruct in logicalConstructs)
			{
				conditionLogicalConstructs.Add(logicalConstruct);
			}
			return new ConditionLogicalConstruct(conditionNode, conditionLogicalConstruct1, conditionLogicalConstructs, conditionExpression);
		}

		private void CreateComplexConditions(ILogicalConstruct theConstruct)
		{
			if (theConstruct is ConditionLogicalConstruct || theConstruct is CFGBlockLogicalConstruct)
			{
				return;
			}
			foreach (ILogicalConstruct child in theConstruct.Children)
			{
				this.CreateComplexConditions(child);
			}
			while (this.TryTraverseAndMerge(theConstruct))
			{
			}
		}

		private void CreateSimpleConditions()
		{
			foreach (CFGBlockLogicalConstruct[] value in this.logicalBuilderContext.CFGBlockToLogicalConstructMap.Values)
			{
				CFGBlockLogicalConstruct cFGBlockLogicalConstruct = value[(int)value.Length - 1];
				InstructionBlock theBlock = cFGBlockLogicalConstruct.TheBlock;
				if ((int)theBlock.Successors.Length != 2 || !(theBlock.Successors[0] != theBlock.Successors[1]) || theBlock.Last.get_OpCode().get_Code() == 68)
				{
					continue;
				}
				ConditionLogicalConstruct.GroupInSimpleConditionConstruct(cFGBlockLogicalConstruct);
			}
		}

		private bool TryTraverseAndMerge(ILogicalConstruct theConstruct)
		{
			HashSet<ILogicalConstruct> logicalConstructs = new HashSet<ILogicalConstruct>();
			Queue<ILogicalConstruct> logicalConstructs1 = new Queue<ILogicalConstruct>();
			logicalConstructs1.Enqueue(theConstruct.Entry as ILogicalConstruct);
			bool flag = false;
			while (logicalConstructs1.Count > 0)
			{
				ILogicalConstruct logicalConstruct = logicalConstructs1.Dequeue();
				ConditionLogicalConstruct conditionLogicalConstruct = logicalConstruct as ConditionLogicalConstruct;
				if (conditionLogicalConstruct != null)
				{
					ConditionLogicalConstruct conditionLogicalConstruct1 = this.CreateComplexCondition(conditionLogicalConstruct);
					flag = flag | conditionLogicalConstruct1 != logicalConstruct;
					logicalConstruct = conditionLogicalConstruct1;
				}
				logicalConstructs.Add(logicalConstruct);
				foreach (ILogicalConstruct sameParentSuccessor in logicalConstruct.SameParentSuccessors)
				{
					if (logicalConstructs.Contains(sameParentSuccessor))
					{
						continue;
					}
					logicalConstructs1.Enqueue(sameParentSuccessor);
				}
				while (logicalConstructs1.Count > 0 && logicalConstructs.Contains(logicalConstructs1.Peek()))
				{
					logicalConstructs1.Dequeue();
				}
			}
			return flag;
		}
	}
}