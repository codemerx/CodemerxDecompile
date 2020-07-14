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
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.ContinueStatement;
			}
		}

		public ContinueStatement(ICollection<Instruction> jumps)
		{
			this.continueJumps = new List<Instruction>();
			if (jumps != null)
			{
				this.continueJumps.AddRange(jumps);
				this.continueJumps.Sort((Instruction x, Instruction y) => x.get_Offset().CompareTo(y.get_Offset()));
			}
		}

		public override Statement Clone()
		{
			ContinueStatement continueStatement = new ContinueStatement(this.continueJumps);
			base.CopyParentAndLabel(continueStatement);
			return continueStatement;
		}

		public override Statement CloneStatementOnly()
		{
			ContinueStatement continueStatement = new ContinueStatement(null);
			base.CopyParentAndLabel(continueStatement);
			return continueStatement;
		}

		protected override IEnumerable<Instruction> GetOwnInstructions()
		{
			ContinueStatement continueStatement = null;
			if (continueStatement.continueJumps != null)
			{
				foreach (Instruction continueJump in continueStatement.continueJumps)
				{
					yield return continueJump;
				}
			}
		}
	}
}