using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;
using Telerik.JustDecompiler.Ast.Statements;
using Telerik.JustDecompiler.Cil;
using Telerik.JustDecompiler.Decompiler;

namespace Telerik.JustDecompiler.Steps
{
	internal class MapUnconditionalBranchesStep : BaseCodeVisitor, IDecompilationStep
	{
		private readonly HashSet<Instruction> mappedInstructions = new HashSet<Instruction>();

		private MethodSpecificContext methodContext;

		public MapUnconditionalBranchesStep()
		{
		}

		private bool IsUnconditionalBranch(Instruction instruction)
		{
			Code code = instruction.get_OpCode().get_Code();
			if (code == 55)
			{
				return true;
			}
			return code == 42;
		}

		private void MapBranches(Expression expression)
		{
			List<Instruction> instructions = new List<Instruction>(expression.UnderlyingSameMethodInstructions);
			List<Instruction> instructions1 = new List<Instruction>();
			int count = instructions.Count;
			for (int i = 0; i < count; i++)
			{
				Instruction item = instructions[i];
				FlowControl flowControl = item.get_OpCode().get_FlowControl();
				if (flowControl != null && flowControl != 3 && flowControl - 7 > 1)
				{
					Instruction next = item.get_Next();
					if (next != null && !this.mappedInstructions.Contains(next) && (i == instructions.Count - 1 || (object)next != (object)instructions[i + 1]) && this.IsUnconditionalBranch(next) && !this.methodContext.ControlFlowGraph.InstructionToBlockMapping.ContainsKey(next.get_Offset()))
					{
						instructions1.Add(next);
					}
				}
			}
			this.mappedInstructions.UnionWith(instructions1);
			expression.MapBranchInstructions(instructions1);
		}

		public BlockStatement Process(DecompilationContext context, BlockStatement body)
		{
			this.methodContext = context.MethodContext;
			this.mappedInstructions.UnionWith(body.UnderlyingSameMethodInstructions);
			this.Visit(body);
			return body;
		}

		public override void Visit(ICodeNode node)
		{
			Expression expression = node as Expression;
			if (expression != null)
			{
				this.MapBranches(expression);
				return;
			}
			base.Visit(node);
		}
	}
}