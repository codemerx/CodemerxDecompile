using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class EmptyStatement : Statement
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				return new EmptyStatement.u003cget_Childrenu003ed__1(-2);
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 58;
			}
		}

		public EmptyStatement()
		{
			base();
			return;
		}

		public override Statement Clone()
		{
			return this.CloneStatementOnly();
		}

		public override Statement CloneStatementOnly()
		{
			V_0 = new EmptyStatement();
			this.CopyParentAndLabel(V_0);
			return V_0;
		}
	}
}