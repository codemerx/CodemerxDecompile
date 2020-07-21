using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class CaseGotoStatement : GotoStatement
	{
		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 69;
			}
		}

		public SwitchCase TargetedSwitchCase
		{
			get;
			private set;
		}

		public CaseGotoStatement(GotoStatement transformedGoto, SwitchCase targetedCase)
		{
			base(transformedGoto.get_Label(), transformedGoto.get_UnderlyingSameMethodInstructions());
			this.set_TargetedSwitchCase(targetedCase);
			return;
		}

		private CaseGotoStatement(CaseGotoStatement toClone, IEnumerable<Instruction> instructions)
		{
			base(toClone.get_Label(), instructions);
			this.set_TargetedSwitchCase(toClone.get_TargetedSwitchCase());
			return;
		}

		public override Statement Clone()
		{
			V_0 = new CaseGotoStatement(this, this.get_UnderlyingSameMethodInstructions());
			this.CopyParentAndLabel(V_0);
			return V_0;
		}

		public override Statement CloneStatementOnly()
		{
			V_0 = new CaseGotoStatement(this, null);
			this.CopyParentAndLabel(V_0);
			return V_0;
		}
	}
}