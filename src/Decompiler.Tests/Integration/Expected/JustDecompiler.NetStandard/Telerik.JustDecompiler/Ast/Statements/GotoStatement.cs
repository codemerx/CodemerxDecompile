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
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.GotoStatement;
			}
		}

		public string TargetLabel
		{
			get;
			set;
		}

		public GotoStatement(string label, IEnumerable<Instruction> gotoJumps)
		{
			this.TargetLabel = label;
			this.jumps = new List<Instruction>();
			if (gotoJumps != null)
			{
				this.jumps.AddRange(gotoJumps);
				this.jumps.Sort((Instruction x, Instruction y) => x.get_Offset().CompareTo(y.get_Offset()));
			}
		}

		public override Statement Clone()
		{
			GotoStatement gotoStatement = new GotoStatement(this.TargetLabel, this.jumps);
			base.CopyParentAndLabel(gotoStatement);
			return gotoStatement;
		}

		public override Statement CloneStatementOnly()
		{
			GotoStatement gotoStatement = new GotoStatement(this.TargetLabel, null);
			base.CopyParentAndLabel(gotoStatement);
			return gotoStatement;
		}

		protected override IEnumerable<Instruction> GetOwnInstructions()
		{
			GotoStatement gotoStatement = null;
			foreach (Instruction jump in gotoStatement.jumps)
			{
				yield return jump;
			}
		}
	}
}