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
			this.Label = String.Empty;
		}

		public abstract Statement Clone();

		public abstract Statement CloneStatementOnly();

		protected void CopyParentAndLabel(Statement target)
		{
			target.Label = this.Label;
			target.Parent = this.Parent;
		}

		protected override IEnumerable<Instruction> GetOwnInstructions()
		{
		}
	}
}