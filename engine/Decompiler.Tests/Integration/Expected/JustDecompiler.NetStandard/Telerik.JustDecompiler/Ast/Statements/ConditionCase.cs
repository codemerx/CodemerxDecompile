using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class ConditionCase : SwitchCase
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new ConditionCase.u003cget_Childrenu003ed__3(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 13;
			}
		}

		public Expression Condition
		{
			get;
			set;
		}

		public ConditionCase()
		{
			base();
			return;
		}

		public ConditionCase(Expression condition, BlockStatement body)
		{
			base(body);
			this.set_Condition(condition);
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
			V_1 = new ConditionCase(this.get_Condition().Clone(), stackVariable5);
			this.CopyParentAndLabel(V_1);
			return V_1;
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
			V_1 = new ConditionCase(this.get_Condition().CloneExpressionOnly(), stackVariable5);
			this.CopyParentAndLabel(V_1);
			return V_1;
		}
	}
}