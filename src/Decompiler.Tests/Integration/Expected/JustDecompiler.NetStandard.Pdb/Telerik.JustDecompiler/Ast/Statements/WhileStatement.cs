using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class WhileStatement : ConditionStatement
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
				WhileStatement whileStatement = null;
				yield return whileStatement.Condition;
				if (whileStatement.body != null)
				{
					yield return whileStatement.body;
				}
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.WhileStatement;
			}
		}

		public WhileStatement(Expression condition, BlockStatement body) : base(condition)
		{
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
			WhileStatement whileStatement = new WhileStatement(base.Condition.Clone(), blockStatement);
			base.CopyParentAndLabel(whileStatement);
			return whileStatement;
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
			WhileStatement whileStatement = new WhileStatement(base.Condition.CloneExpressionOnly(), blockStatement);
			base.CopyParentAndLabel(whileStatement);
			return whileStatement;
		}
	}
}