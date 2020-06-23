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
			Code code = instruction.OpCode.Code;
			if (code == Code.Br)
			{
				return true;
			}
			return code == Code.Br_S;
		}

		private void MapBranches(Expression expression)
		{
			List<Instruction> instructions = new List<Instruction>(expression.UnderlyingSameMethodInstructions);
			List<Instruction> instructions1 = new List<Instruction>();
			int count = instructions.Count;
			for (int i = 0; i < count; i++)
			{
				Instruction item = instructions[i];
				FlowControl flowControl = item.OpCode.FlowControl;
				if (flowControl != FlowControl.Branch && flowControl != FlowControl.Cond_Branch && (int)flowControl - (int)FlowControl.Return > (int)FlowControl.Break)
				{
					Instruction next = item.Next;
					if (next != null && !this.mappedInstructions.Contains(next) && (i == instructions.Count - 1 || next != instructions[i + 1]) && this.IsUnconditionalBranch(next) && !this.methodContext.ControlFlowGraph.InstructionToBlockMapping.ContainsKey(next.Offset))
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