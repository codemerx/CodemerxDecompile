using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Decompiler;
using Telerik.JustDecompiler.Decompiler.DefineUseAnalysis;

namespace Telerik.JustDecompiler.Decompiler.TypeInference
{
	internal class GreedyTypeInferer : TypeInferer
	{
		private HashSet<VariableReference> resolvedVariables;

		public GreedyTypeInferer(DecompilationContext context, Dictionary<int, Expression> offsetToExpression)
		{
			base(context, offsetToExpression);
			this.resolvedVariables = new HashSet<VariableReference>();
			return;
		}

		private T First<T>(IEnumerable<T> collection)
		{
			V_0 = collection.GetEnumerator();
			try
			{
				dummyVar0 = V_0.MoveNext();
				V_1 = V_0.get_Current();
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
			return V_1;
		}

		private bool FixVariableType(VariableReference variable, TypeReference type)
		{
			variable.set_VariableType(type);
			if (type == null)
			{
				return false;
			}
			dummyVar0 = this.resolvedVariables.Add(variable);
			return true;
		}

		private IList<VariableReference> GetVariablesToInfer()
		{
			V_0 = new List<VariableReference>();
			V_1 = this.context.get_MethodContext().get_StackData().get_VariableToDefineUseInfo().get_Keys().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (V_2.get_VariableType() != null)
					{
						dummyVar0 = this.resolvedVariables.Add(V_2);
					}
					else
					{
						V_0.Add(V_2);
					}
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			return V_0;
		}

		public new HashSet<VariableReference> InferTypes()
		{
			this.resolvedVariables = new HashSet<VariableReference>();
			V_0 = false;
			do
			{
				V_0 = false;
				V_1 = this.GetVariablesToInfer().GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = V_1.get_Current();
						if (!this.IsOnlyAssignedOnce(V_2, out V_3))
						{
							if (!this.IsOnlyUsedOnce(V_2, out V_3))
							{
								continue;
							}
							V_0 = this.FixVariableType(V_2, V_3) | V_0;
						}
						else
						{
							V_0 = this.FixVariableType(V_2, V_3) | V_0;
						}
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
			while (V_0);
			return this.resolvedVariables;
		}

		private bool IsOnlyAssignedOnce(VariableReference variable, out TypeReference type)
		{
			type = null;
			if (!this.context.get_MethodContext().get_StackData().get_VariableToDefineUseInfo().TryGetValue(variable.Resolve(), out V_0))
			{
				throw new Exception("Define/use info not found.");
			}
			if (V_0.get_DefinedAt().get_Count() != 1)
			{
				return false;
			}
			type = this.offsetToExpression.get_Item(this.First<int>(V_0.get_DefinedAt())).get_ExpressionType();
			return true;
		}

		private bool IsOnlyUsedOnce(VariableReference variable, out TypeReference type)
		{
			type = null;
			if (!this.context.get_MethodContext().get_StackData().get_VariableToDefineUseInfo().TryGetValue(variable.Resolve(), out V_0))
			{
				throw new Exception("Define/use info not found.");
			}
			if (V_0.get_UsedAt().get_Count() != 1)
			{
				return false;
			}
			V_1 = this.First<int>(V_0.get_UsedAt());
			V_2 = new UsedAsTypeHelper(this.context.get_MethodContext());
			V_3 = this.context.get_MethodContext().get_ControlFlowGraph().get_OffsetToInstruction().get_Item(V_1);
			type = V_2.GetUseExpressionTypeNode(V_3, this.offsetToExpression.get_Item(V_1), variable);
			return true;
		}
	}
}