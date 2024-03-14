using System;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class BreakSwitchCaseStatement : BreakStatement
	{
		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.BreakSwitchCaseStatement;
			}
		}

		public BreakSwitchCaseStatement() : base(null)
		{
		}

		public override Statement Clone()
		{
			return this.CloneStatementOnly();
		}

		public override Statement CloneStatementOnly()
		{
			BreakSwitchCaseStatement breakSwitchCaseStatement = new BreakSwitchCaseStatement();
			base.CopyParentAndLabel(breakSwitchCaseStatement);
			return breakSwitchCaseStatement;
		}
	}
}