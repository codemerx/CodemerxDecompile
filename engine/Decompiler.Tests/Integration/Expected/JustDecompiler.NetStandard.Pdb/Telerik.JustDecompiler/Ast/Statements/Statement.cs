using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public abstract class Statement : BaseCodeNode
	{
		public string Label
		{
			get;
			set;
		}

		public Statement Parent
		{
			get;
			set;
		}

		public Statement()
		{
			base();
			this.set_Label(String.Empty);
			return;
		}

		public abstract Statement Clone();

		public abstract Statement CloneStatementOnly();

		protected void CopyParentAndLabel(Statement target)
		{
			target.set_Label(this.get_Label());
			target.set_Parent(this.get_Parent());
			return;
		}

		protected override IEnumerable<Instruction> GetOwnInstructions()
		{
			return new Statement.u003cGetOwnInstructionsu003ed__1(-2);
		}
	}
}