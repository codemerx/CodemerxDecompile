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
					this.body.Parent = this;
				}
			}
		}

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				FixedStatement fixedStatement = null;
				if (fixedStatement.body != null)
				{
					yield return fixedStatement.body;
				}
				if (fixedStatement.Expression != null)
				{
					yield return fixedStatement.Expression;
				}
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.FixedStatement;
			}
		}

		public Telerik.JustDecompiler.Ast.Expressions.Expression Expression
		{
			get;
			set;
		}

		public FixedStatement(Telerik.JustDecompiler.Ast.Expressions.Expression expression, BlockStatement body)
		{
			this.Expression = expression;
			this.Body = body;
		}

		public override Statement Clone()
		{
			BlockStatement blockStatement;
			if (this.body != null)
			{
				blockStatement = this.body.Clone() as BlockStatement;
			}
			else
			{
				blockStatement = null;
			}
			FixedStatement fixedStatement = new FixedStatement(this.Expression.Clone(), blockStatement);
			base.CopyParentAndLabel(fixedStatement);
			return fixedStatement;
		}

		public override Statement CloneStatementOnly()
		{
			BlockStatement blockStatement;
			if (this.body != null)
			{
				blockStatement = this.body.CloneStatementOnly() as BlockStatement;
			}
			else
			{
				blockStatement = null;
			}
			FixedStatement fixedStatement = new FixedStatement(this.Expression.CloneExpressionOnly(), blockStatement);
			base.CopyParentAndLabel(fixedStatement);
			return fixedStatement;
		}
	}
}