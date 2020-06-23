using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.DefineUseAnalysis;

namespace Telerik.JustDecompiler.Decompiler.TypeInference
{
	internal class GreedyTypeInferer : TypeInferer
	{
		private HashSet<VariableReference> resolvedVariables;

		public GreedyTypeInferer(DecompilationContext context, Dictionary<int, Expression> offsetToExpression) : base(context, offsetToExpression)
		{
			this.resolvedVariables = new HashSet<VariableReference>();
		}

		private T First<T>(IEnumerable<T> collection)
		{
			T current;
			using (IEnumerator<T> enumerator = collection.GetEnumerator())
			{
				enumerator.MoveNext();
				current = enumerator.Current;
			}
			return current;
		}

		private bool FixVariableType(VariableReference variable, TypeReference type)
		{
			variable.VariableType = type;
			if (type == null)
			{
				return false;
			}
			this.resolvedVariables.Add(variable);
			return true;
		}

		private IList<VariableReference> GetVariablesToInfer()
		{
			List<VariableReference> variableReferences = new List<VariableReference>();
			foreach (VariableDefinition key in this.context.MethodContext.StackData.VariableToDefineUseInfo.Keys)
			{
				if (key.VariableType != null)
				{
					this.resolvedVariables.Add(key);
				}
				else
				{
					variableReferences.Add(key);
				}
			}
			return variableReferences;
		}

		public new HashSet<VariableReference> InferTypes()
		{
			TypeReference typeReference;
			this.resolvedVariables = new HashSet<VariableReference>();
			bool flag = false;
			do
			{
				flag = false;
				foreach (VariableReference variablesToInfer in this.GetVariablesToInfer())
				{
					if (!this.IsOnlyAssignedOnce(variablesToInfer, out typeReference))
					{
						if (!this.IsOnlyUsedOnce(variablesToInfer, out typeReference))
						{
							continue;
						}
						flag = this.FixVariableType(variablesToInfer, typeReference) | flag;
					}
					else
					{
						flag = this.FixVariableType(variablesToInfer, typeReference) | flag;
					}
				}
			}
			while (flag);
			return this.resolvedVariables;
		}

		private bool IsOnlyAssignedOnce(VariableReference variable, out TypeReference type)
		{
			StackVariableDefineUseInfo stackVariableDefineUseInfo;
			type = null;
			if (!this.context.MethodContext.StackData.VariableToDefineUseInfo.TryGetValue(variable.Resolve(), out stackVariableDefineUseInfo))
			{
				throw new Exception("Define/use info not found.");
			}
			if (stackVariableDefineUseInfo.DefinedAt.Count != 1)
			{
				return false;
			}
			type = this.offsetToExpression[this.First<int>(stackVariableDefineUseInfo.DefinedAt)].ExpressionType;
			return true;
		}

		private bool IsOnlyUsedOnce(VariableReference variable, out TypeReference type)
		{
			StackVariableDefineUseInfo stackVariableDefineUseInfo;
			type = null;
			if (!this.context.MethodContext.StackData.VariableToDefineUseInfo.TryGetValue(variable.Resolve(), out stackVariableDefineUseInfo))
			{
				throw new Exception("Define/use info not found.");
			}
			if (stackVariableDefineUseInfo.UsedAt.Count != 1)
			{
				return false;
			}
			int num = this.First<int>(stackVariableDefineUseInfo.UsedAt);
			UsedAsTypeHelper usedAsTypeHelper = new UsedAsTypeHelper(this.context.MethodContext);
			Instruction item = this.context.MethodContext.ControlFlowGraph.OffsetToInstruction[num];
			type = usedAsTypeHelper.GetUseExpressionTypeNode(item, this.offsetToExpression[num], variable);
			return true;
		}
	}
}