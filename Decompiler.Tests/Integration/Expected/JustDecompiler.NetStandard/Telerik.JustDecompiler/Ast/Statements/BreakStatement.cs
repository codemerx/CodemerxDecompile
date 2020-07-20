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
				return new BreakStatement.u003cget_Childrenu003ed__3(-2);
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 9;
			}
		}

		public BreakStatement(IEnumerable<Instruction> jumps)
		{
			base();
			this.breakJumps = new List<Instruction>();
			if (jumps != null)
			{
				this.breakJumps.AddRange(jumps);
				stackVariable8 = this.breakJumps;
				stackVariable9 = BreakStatement.u003cu003ec.u003cu003e9__1_0;
				if (stackVariable9 == null)
				{
					dummyVar0 = stackVariable9;
					stackVariable9 = new Comparison<Instruction>(BreakStatement.u003cu003ec.u003cu003e9.u003cu002ectoru003eb__1_0);
					BreakStatement.u003cu003ec.u003cu003e9__1_0 = stackVariable9;
				}
				stackVariable8.Sort(stackVariable9);
			}
			return;
		}

		public override Statement Clone()
		{
			V_0 = new BreakStatement(this.breakJumps);
			this.CopyParentAndLabel(V_0);
			return V_0;
		}

		public override Statement CloneStatementOnly()
		{
			V_0 = new BreakStatement(null);
			this.CopyParentAndLabel(V_0);
			return V_0;
		}

		protected override IEnumerable<Instruction> GetOwnInstructions()
		{
			stackVariable1 = new BreakStatement.u003cGetOwnInstructionsu003ed__4(-2);
			stackVariable1.u003cu003e4__this = this;
			return stackVariable1;
		}
	}
}