using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class IfStatement : ConditionStatement
	{
		private BlockStatement then;

		private BlockStatement else;

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new IfStatement.u003cget_Childrenu003ed__4(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 3;
			}
		}

		public BlockStatement Else
		{
			get
			{
				return this.else;
			}
			set
			{
				this.else = value;
				if (this.else != null)
				{
					this.else.set_Parent(this);
				}
				return;
			}
		}

		public BlockStatement Then
		{
			get
			{
				return this.then;
			}
			set
			{
				this.then = value;
				if (this.then != null)
				{
					this.then.set_Parent(this);
				}
				return;
			}
		}

		public IfStatement(Expression condition, BlockStatement then, BlockStatement else)
		{
			base(condition);
			this.set_Then(then);
			this.set_Else(else);
			return;
		}

		public override Statement Clone()
		{
			V_0 = null;
			if (this.then != null)
			{
				V_0 = this.then.Clone() as BlockStatement;
			}
			V_1 = null;
			if (this.else != null)
			{
				V_1 = this.else.Clone() as BlockStatement;
			}
			V_2 = new IfStatement(this.get_Condition().Clone(), V_0, V_1);
			this.CopyParentAndLabel(V_2);
			return V_2;
		}

		public override Statement CloneStatementOnly()
		{
			if (this.then != null)
			{
				stackVariable5 = this.then.CloneStatementOnly() as BlockStatement;
			}
			else
			{
				stackVariable5 = null;
			}
			V_0 = stackVariable5;
			if (this.else != null)
			{
				stackVariable11 = this.else.CloneStatementOnly() as BlockStatement;
			}
			else
			{
				stackVariable11 = null;
			}
			V_1 = stackVariable11;
			V_2 = new IfStatement(this.get_Condition().CloneExpressionOnly(), V_0, V_1);
			this.CopyParentAndLabel(V_2);
			return V_2;
		}
	}
}