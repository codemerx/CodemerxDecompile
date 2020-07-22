using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class ForStatement : ConditionStatement
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
				stackVariable1 = new ForStatement.u003cget_Childrenu003ed__3(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 11;
			}
		}

		public Expression Increment
		{
			get;
			set;
		}

		public Expression Initializer
		{
			get;
			set;
		}

		public ForStatement(Expression initializer, Expression condition, Expression increment, BlockStatement body)
		{
			base(condition);
			this.set_Initializer(initializer);
			this.set_Increment(increment);
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
			V_0 = stackVariable5;
			V_1 = new ForStatement(this.get_Initializer().Clone(), this.get_Condition().Clone(), this.get_Increment().Clone(), V_0);
			V_1.set_ConditionBlock(this.get_ConditionBlock());
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
			V_0 = stackVariable5;
			V_1 = new ForStatement(this.get_Initializer().CloneExpressionOnly(), this.get_Condition().CloneExpressionOnly(), this.get_Increment().CloneExpressionOnly(), V_0);
			V_1.set_ConditionBlock(null);
			this.CopyParentAndLabel(V_1);
			return V_1;
		}
	}
}