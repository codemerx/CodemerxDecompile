using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class GotoStatement : BasePdbStatement
	{
		private List<Instruction> jumps;

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				return new GotoStatement.u003cget_Childrenu003ed__4(-2);
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 2;
			}
		}

		public string TargetLabel
		{
			get;
			set;
		}

		public GotoStatement(string label, IEnumerable<Instruction> gotoJumps)
		{
			base();
			this.set_TargetLabel(label);
			this.jumps = new List<Instruction>();
			if (gotoJumps != null)
			{
				this.jumps.AddRange(gotoJumps);
				stackVariable10 = this.jumps;
				stackVariable11 = GotoStatement.u003cu003ec.u003cu003e9__1_0;
				if (stackVariable11 == null)
				{
					dummyVar0 = stackVariable11;
					stackVariable11 = new Comparison<Instruction>(GotoStatement.u003cu003ec.u003cu003e9.u003cu002ectoru003eb__1_0);
					GotoStatement.u003cu003ec.u003cu003e9__1_0 = stackVariable11;
				}
				stackVariable10.Sort(stackVariable11);
			}
			return;
		}

		public override Statement Clone()
		{
			V_0 = new GotoStatement(this.get_TargetLabel(), this.jumps);
			this.CopyParentAndLabel(V_0);
			return V_0;
		}

		public override Statement CloneStatementOnly()
		{
			V_0 = new GotoStatement(this.get_TargetLabel(), null);
			this.CopyParentAndLabel(V_0);
			return V_0;
		}

		protected override IEnumerable<Instruction> GetOwnInstructions()
		{
			stackVariable1 = new GotoStatement.u003cGetOwnInstructionsu003ed__2(-2);
			stackVariable1.u003cu003e4__this = this;
			return stackVariable1;
		}
	}
}