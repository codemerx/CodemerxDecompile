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
				ExpressionStatement expressionStatement = null;
				yield return expressionStatement.Expression;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.ExpressionStatement;
			}
		}

		public Telerik.JustDecompiler.Ast.Expressions.Expression Expression
		{
			get;
			set;
		}

		public ExpressionStatement(Telerik.JustDecompiler.Ast.Expressions.Expression expression)
		{
			this.Expression = expression;
		}

		public override Statement Clone()
		{
			ExpressionStatement expressionStatement = new ExpressionStatement(this.Expression.Clone());
			base.CopyParentAndLabel(expressionStatement);
			return expressionStatement;
		}

		public override Statement CloneStatementOnly()
		{
			ExpressionStatement expressionStatement = new ExpressionStatement(this.Expression.CloneExpressionOnly());
			base.CopyParentAndLabel(expressionStatement);
			return expressionStatement;
		}
	}
}