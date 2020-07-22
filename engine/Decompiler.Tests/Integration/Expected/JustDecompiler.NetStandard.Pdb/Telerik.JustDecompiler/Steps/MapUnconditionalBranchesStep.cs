using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class MapUnconditionalBranchesStep : BaseCodeVisitor, IDecompilationStep
	{
		private readonly HashSet<Instruction> mappedInstructions;

		private MethodSpecificContext methodContext;

		public MapUnconditionalBranchesStep()
		{
			this.mappedInstructions = new HashSet<Instruction>();
			base();
			return;
		}

		private bool IsUnconditionalBranch(Instruction instruction)
		{
			V_0 = instruction.get_OpCode().get_Code();
			if (V_0 == 55)
			{
				return true;
			}
			return V_0 == 42;
		}

		private void MapBranches(Expression expression)
		{
			V_0 = new List<Instruction>(expression.get_UnderlyingSameMethodInstructions());
			V_1 = new List<Instruction>();
			V_2 = V_0.get_Count();
			V_3 = 0;
			while (V_3 < V_2)
			{
				V_4 = V_0.get_Item(V_3);
				V_6 = V_4.get_OpCode().get_FlowControl();
				if (V_6 != null && V_6 != 3 && V_6 - 7 > 1)
				{
					V_5 = V_4.get_Next();
					if (V_5 != null && !this.mappedInstructions.Contains(V_5) && V_3 == V_0.get_Count() - 1 || (object)V_5 != (object)V_0.get_Item(V_3 + 1) && this.IsUnconditionalBranch(V_5) && !this.methodContext.get_ControlFlowGraph().get_InstructionToBlockMapping().ContainsKey(V_5.get_Offset()))
					{
						V_1.Add(V_5);
					}
				}
				V_3 = V_3 + 1;
			}
			this.mappedInstructions.UnionWith(V_1);
			expression.MapBranchInstructions(V_1);
			return;
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.methodContext = context.get_MethodContext();
			this.mappedInstructions.UnionWith(body.get_UnderlyingSameMethodInstructions());
			this.Visit(body);
			return body;
		}

		public override void Visit(ICodeNode node)
		{
			V_0 = node as Expression;
			if (V_0 != null)
			{
				this.MapBranches(V_0);
				return;
			}
			this.Visit(node);
			return;
		}
	}
}