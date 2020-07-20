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
				this.body.set_Parent(this);
				return;
			}
		}

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new FinallyClause.u003cget_Childrenu003ed__10(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 70;
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
			base();
			if (body == null)
			{
				throw new ArgumentNullException("body");
			}
			this.body = body;
			body.set_Parent(this);
			if (mappedInstructions != null)
			{
				this.mappedInstructions = new List<Instruction>(mappedInstructions);
				stackVariable11 = this.mappedInstructions;
				stackVariable12 = FinallyClause.u003cu003ec.u003cu003e9__2_0;
				if (stackVariable12 == null)
				{
					dummyVar0 = stackVariable12;
					stackVariable12 = new Comparison<Instruction>(FinallyClause.u003cu003ec.u003cu003e9.u003cu002ectoru003eb__2_0);
					FinallyClause.u003cu003ec.u003cu003e9__2_0 = stackVariable12;
				}
				stackVariable11.Sort(stackVariable12);
			}
			return;
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
			stackVariable1 = new FinallyClause.u003cGetOwnInstructionsu003ed__8(-2);
			stackVariable1.u003cu003e4__this = this;
			return stackVariable1;
		}
	}
}