namespace Telerik.JustDecompiler.Ast.Statements
{
	public class BreakSwitchCaseStatement : BreakStatement
	{
		public BreakSwitchCaseStatement() : base(null)
		{
		}

		public override CodeNodeType CodeNodeType
		{
			get
			{
				return CodeNodeType.BreakSwitchCaseStatement;
			}
		}

		public override Statement Clone()
		{
            return CloneStatementOnly();
		}

        public override Statement CloneStatementOnly()
        {
            BreakSwitchCaseStatement result = new BreakSwitchCaseStatement();
            CopyParentAndLabel(result);
            return result;
        }
	}
}