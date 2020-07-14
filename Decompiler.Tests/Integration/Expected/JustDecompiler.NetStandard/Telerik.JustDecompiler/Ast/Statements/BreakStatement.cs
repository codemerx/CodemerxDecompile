using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class BreakStatement : BasePdbStatement
	{
		private readonly List<Instruction> breakJumps;

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.BreakStatement;
			}
		}

		public BreakStatement(IEnumerable<Instruction> jumps)
		{
			this.breakJumps = new List<Instruction>();
			if (jumps != null)
			{
				this.breakJumps.AddRange(jumps);
				this.breakJumps.Sort((Instruction x, Instruction y) => x.get_Offset().CompareTo(y.get_Offset()));
			}
		}

		public override Statement Clone()
		{
			BreakStatement breakStatement = new BreakStatement(this.breakJumps);
			base.CopyParentAndLabel(breakStatement);
			return breakStatement;
		}

		public override Statement CloneStatementOnly()
		{
			BreakStatement breakStatement = new BreakStatement(null);
			base.CopyParentAndLabel(breakStatement);
			return breakStatement;
		}

		protected override IEnumerable<Instruction> GetOwnInstructions()
		{
			BreakStatement breakStatement = null;
			if (breakStatement.breakJumps != null)
			{
				foreach (Instruction breakJump in breakStatement.breakJumps)
				{
					yield return breakJump;
				}
			}
		}
	}
}