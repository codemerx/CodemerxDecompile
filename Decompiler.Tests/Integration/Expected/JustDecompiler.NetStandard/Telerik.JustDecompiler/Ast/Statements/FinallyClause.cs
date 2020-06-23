using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class FinallyClause : Statement
	{
		private readonly List<Instruction> mappedInstructions;

		private BlockStatement body;

		public BlockStatement Body
		{
			get
			{
				return this.body;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}
				this.body = value;
				this.body.Parent = this;
			}
		}

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				FinallyClause finallyClause = null;
				if (!finallyClause.IsSpecialYieldFinally)
				{
					yield return finallyClause.body;
				}
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.FinallyClause;
			}
		}

		internal bool IsSpecialYieldFinally
		{
			get
			{
				return this.mappedInstructions != null;
			}
		}

		public FinallyClause(BlockStatement body, IEnumerable<Instruction> mappedInstructions = null)
		{
			if (body == null)
			{
				throw new ArgumentNullException("body");
			}
			this.body = body;
			body.Parent = this;
			if (mappedInstructions != null)
			{
				this.mappedInstructions = new List<Instruction>(mappedInstructions);
				this.mappedInstructions.Sort((Instruction x, Instruction y) => x.Offset.CompareTo(y.Offset));
			}
		}

		public override Statement Clone()
		{
			return new FinallyClause(this.body.Clone() as BlockStatement, this.mappedInstructions);
		}

		public override Statement CloneStatementOnly()
		{
			return new FinallyClause(this.body.CloneStatementOnly() as BlockStatement, null);
		}

		protected override IEnumerable<Instruction> GetOwnInstructions()
		{
			FinallyClause finallyClause = null;
			if (finallyClause.IsSpecialYieldFinally)
			{
				foreach (Instruction mappedInstruction in finallyClause.mappedInstructions)
				{
					yield return mappedInstruction;
				}
			}
		}
	}
}