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
					this.body.Parent = this;
				}
			}
		}

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				ForStatement forStatement = null;
				yield return forStatement.Condition;
				if (forStatement.Initializer != null)
				{
					yield return forStatement.Initializer;
				}
				if (forStatement.Increment != null)
				{
					yield return forStatement.Increment;
				}
				if (forStatement.Body != null)
				{
					yield return forStatement.Body;
				}
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.ForStatement;
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

		public ForStatement(Expression initializer, Expression condition, Expression increment, BlockStatement body) : base(condition)
		{
			this.Initializer = initializer;
			this.Increment = increment;
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
			BlockStatement blockStatement1 = blockStatement;
			ForStatement forStatement = new ForStatement(this.Initializer.Clone(), base.Condition.Clone(), this.Increment.Clone(), blockStatement1)
			{
				ConditionBlock = base.ConditionBlock
			};
			base.CopyParentAndLabel(forStatement);
			return forStatement;
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
			BlockStatement blockStatement1 = blockStatement;
			ForStatement forStatement = new ForStatement(this.Initializer.CloneExpressionOnly(), base.Condition.CloneExpressionOnly(), this.Increment.CloneExpressionOnly(), blockStatement1)
			{
				ConditionBlock = null
			};
			base.CopyParentAndLabel(forStatement);
			return forStatement;
		}
	}
}