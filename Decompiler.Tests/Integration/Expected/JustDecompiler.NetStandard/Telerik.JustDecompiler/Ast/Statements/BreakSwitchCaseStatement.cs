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
				return 68;
			}
		}

		public BreakSwitchCaseStatement()
		{
			base(null);
			return;
		}

		public override Statement Clone()
		{
			return this.CloneStatementOnly();
		}

		public override Statement CloneStatementOnly()
		{
			V_0 = new BreakSwitchCaseStatement();
			this.CopyParentAndLabel(V_0);
			return V_0;
		}
	}
}