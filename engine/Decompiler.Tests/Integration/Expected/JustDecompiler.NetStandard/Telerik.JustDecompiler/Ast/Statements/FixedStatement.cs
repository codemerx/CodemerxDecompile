using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class FixedStatement : BasePdbStatement
	{
		private BlockStatement body;

		public BlockStatement Body
		{
			get
			{
				return this.body;
			}
			set
			{
				this.body = value;
				if (this.body != null)
				{
					this.body.set_Parent(this);
				}
				return;
			}
		}

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new FixedStatement.u003cget_Childrenu003ed__3(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 37;
			}
		}

		public Telerik.JustDecompiler.Ast.Expressions.Expression Expression
		{
			get;
			set;
		}

		public FixedStatement(Telerik.JustDecompiler.Ast.Expressions.Expression expression, BlockStatement body)
		{
			base();
			this.set_Expression(expression);
			this.set_Body(body);
			return;
		}

		public override Statement Clone()
		{
			if (this.body != null)
			{
				stackVariable5 = this.body.Clone() as BlockStatement;
			}
			else
			{
				stackVariable5 = null;
			}
			V_1 = new FixedStatement(this.get_Expression().Clone(), stackVariable5);
			this.CopyParentAndLabel(V_1);
			return V_1;
		}

		public override Statement CloneStatementOnly()
		{
			if (this.body != null)
			{
				stackVariable5 = this.body.CloneStatementOnly() as BlockStatement;
			}
			else
			{
				stackVariable5 = null;
			}
			V_1 = new FixedStatement(this.get_Expression().CloneExpressionOnly(), stackVariable5);
			this.CopyParentAndLabel(V_1);
			return V_1;
		}
	}
}