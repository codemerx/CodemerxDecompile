using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class DoWhileStatement : ConditionStatement
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
				DoWhileStatement doWhileStatement = null;
				yield return doWhileStatement.Condition;
				if (doWhileStatement.body != null)
				{
					yield return doWhileStatement.body;
				}
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.DoWhileStatement;
			}
		}

		public DoWhileStatement(Expression condition, BlockStatement body) : base(condition)
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
			DoWhileStatement doWhileStatement = new DoWhileStatement(base.Condition.Clone(), blockStatement)
			{
				ConditionBlock = base.ConditionBlock
			};
			base.CopyParentAndLabel(doWhileStatement);
			return doWhileStatement;
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
			DoWhileStatement doWhileStatement = new DoWhileStatement(base.Condition.CloneExpressionOnly(), blockStatement)
			{
				ConditionBlock = null
			};
			base.CopyParentAndLabel(doWhileStatement);
			return doWhileStatement;
		}
	}
}