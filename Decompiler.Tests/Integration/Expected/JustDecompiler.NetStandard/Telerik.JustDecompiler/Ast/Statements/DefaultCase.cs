using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class DefaultCase : SwitchCase
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new DefaultCase.u003cget_Childrenu003ed__2(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 14;
			}
		}

		public DefaultCase(BlockStatement body)
		{
			base(body);
			return;
		}

		public override Statement Clone()
		{
			if (this.get_Body() != null)
			{
				stackVariable5 = this.get_Body().Clone() as BlockStatement;
			}
			else
			{
				stackVariable5 = null;
			}
			V_0 = new DefaultCase(stackVariable5);
			this.CopyParentAndLabel(V_0);
			return V_0;
		}

		public override Statement CloneStatementOnly()
		{
			if (this.get_Body() != null)
			{
				stackVariable5 = this.get_Body().CloneStatementOnly() as BlockStatement;
			}
			else
			{
				stackVariable5 = null;
			}
			V_0 = new DefaultCase(stackVariable5);
			this.CopyParentAndLabel(V_0);
			return V_0;
		}
	}
}