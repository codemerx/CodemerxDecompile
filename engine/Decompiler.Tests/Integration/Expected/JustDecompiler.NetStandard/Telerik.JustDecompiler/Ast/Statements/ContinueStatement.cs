using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class ContinueStatement : BasePdbStatement
	{
		private readonly List<Instruction> continueJumps;

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				return new ContinueStatement.u003cget_Childrenu003ed__3(-2);
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 10;
			}
		}

		public ContinueStatement(ICollection<Instruction> jumps)
		{
			base();
			this.continueJumps = new List<Instruction>();
			if (jumps != null)
			{
				this.continueJumps.AddRange(jumps);
				stackVariable8 = this.continueJumps;
				stackVariable9 = ContinueStatement.u003cu003ec.u003cu003e9__1_0;
				if (stackVariable9 == null)
				{
					dummyVar0 = stackVariable9;
					stackVariable9 = new Comparison<Instruction>(ContinueStatement.u003cu003ec.u003cu003e9.u003cu002ectoru003eb__1_0);
					ContinueStatement.u003cu003ec.u003cu003e9__1_0 = stackVariable9;
				}
				stackVariable8.Sort(stackVariable9);
			}
			return;
		}

		public override Statement Clone()
		{
			V_0 = new ContinueStatement(this.continueJumps);
			this.CopyParentAndLabel(V_0);
			return V_0;
		}

		public override Statement CloneStatementOnly()
		{
			V_0 = new ContinueStatement(null);
			this.CopyParentAndLabel(V_0);
			return V_0;
		}

		protected override IEnumerable<Instruction> GetOwnInstructions()
		{
			stackVariable1 = new ContinueStatement.u003cGetOwnInstructionsu003ed__4(-2);
			stackVariable1.u003cu003e4__this = this;
			return stackVariable1;
		}
	}
}