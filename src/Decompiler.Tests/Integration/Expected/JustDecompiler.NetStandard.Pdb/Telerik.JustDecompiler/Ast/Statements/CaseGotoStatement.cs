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
				return Telerik.JustDecompiler.Ast.CodeNodeType.CaseGotoStatement;
			}
		}

		public SwitchCase TargetedSwitchCase
		{
			get;
			private set;
		}

		public CaseGotoStatement(GotoStatement transformedGoto, SwitchCase targetedCase) : base(transformedGoto.Label, transformedGoto.UnderlyingSameMethodInstructions)
		{
			this.TargetedSwitchCase = targetedCase;
		}

		private CaseGotoStatement(CaseGotoStatement toClone, IEnumerable<Instruction> instructions) : base(toClone.Label, instructions)
		{
			this.TargetedSwitchCase = toClone.TargetedSwitchCase;
		}

		public override Statement Clone()
		{
			CaseGotoStatement caseGotoStatement = new CaseGotoStatement(this, base.UnderlyingSameMethodInstructions);
			base.CopyParentAndLabel(caseGotoStatement);
			return caseGotoStatement;
		}

		public override Statement CloneStatementOnly()
		{
			CaseGotoStatement caseGotoStatement = new CaseGotoStatement(this, (IEnumerable<Instruction>)null);
			base.CopyParentAndLabel(caseGotoStatement);
			return caseGotoStatement;
		}
	}
}