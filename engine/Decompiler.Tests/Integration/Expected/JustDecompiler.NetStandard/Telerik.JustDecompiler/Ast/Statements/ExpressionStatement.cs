using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class ExpressionStatement : BasePdbStatement
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new ExpressionStatement.u003cget_Childrenu003ed__2(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 5;
			}
		}

		public Telerik.JustDecompiler.Ast.Expressions.Expression Expression
		{
			get;
			set;
		}

		public ExpressionStatement(Telerik.JustDecompiler.Ast.Expressions.Expression expression)
		{
			base();
			this.set_Expression(expression);
			return;
		}

		public override Statement Clone()
		{
			V_0 = new ExpressionStatement(this.get_Expression().Clone());
			this.CopyParentAndLabel(V_0);
			return V_0;
		}

		public override Statement CloneStatementOnly()
		{
			V_0 = new ExpressionStatement(this.get_Expression().CloneExpressionOnly());
			this.CopyParentAndLabel(V_0);
			return V_0;
		}
	}
}