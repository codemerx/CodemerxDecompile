using System.Collections.Generic;
using Mono.Cecil.Cil;
namespace Telerik.JustDecompiler.Ast.Statements
{
	public class CaseGotoStatement : GotoStatement 
	{
		public CaseGotoStatement(GotoStatement transformedGoto, SwitchCase targetedCase) : base(transformedGoto.Label, transformedGoto.UnderlyingSameMethodInstructions)
		{
			this.TargetedSwitchCase = targetedCase;
		}

		public override CodeNodeType CodeNodeType
		{
			get
			{
				return CodeNodeType.CaseGotoStatement;
			}
		}

		public SwitchCase TargetedSwitchCase { get; private set; }

		/// <summary>
		/// Copy-constructor for cloning purposes.
		/// </summary>
		/// <param name="toClone">The statement to clone.</param>
		private CaseGotoStatement(CaseGotoStatement toClone, IEnumerable<Instruction> instructions) : base(toClone.Label, instructions)
		{
			this.TargetedSwitchCase = toClone.TargetedSwitchCase;
		}

		public override Statement Clone()
		{
			CaseGotoStatement result = new CaseGotoStatement(this, this.UnderlyingSameMethodInstructions);
			CopyParentAndLabel(result);
			return result;
		}

        public override Statement CloneStatementOnly()
        {
            CaseGotoStatement result = new CaseGotoStatement(this, null);
            CopyParentAndLabel(result);
            return result;
        }
	}
}