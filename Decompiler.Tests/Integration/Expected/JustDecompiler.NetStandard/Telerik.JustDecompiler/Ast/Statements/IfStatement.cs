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

		private BlockStatement @else;

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				IfStatement ifStatement = null;
				yield return ifStatement.Condition;
				if (ifStatement.Then != null)
				{
					yield return ifStatement.Then;
				}
				if (ifStatement.Else != null)
				{
					yield return ifStatement.Else;
				}
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.IfStatement;
			}
		}

		public BlockStatement Else
		{
			get
			{
				return this.@else;
			}
			set
			{
				this.@else = value;
				if (this.@else != null)
				{
					this.@else.Parent = this;
				}
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
					this.then.Parent = this;
				}
			}
		}

		public IfStatement(Expression condition, BlockStatement then, BlockStatement @else) : base(condition)
		{
			this.Then = then;
			this.Else = @else;
		}

		public override Statement Clone()
		{
			BlockStatement blockStatement = null;
			if (this.then != null)
			{
				blockStatement = this.then.Clone() as BlockStatement;
			}
			BlockStatement blockStatement1 = null;
			if (this.@else != null)
			{
				blockStatement1 = this.@else.Clone() as BlockStatement;
			}
			IfStatement ifStatement = new IfStatement(base.Condition.Clone(), blockStatement, blockStatement1);
			base.CopyParentAndLabel(ifStatement);
			return ifStatement;
		}

		public override Statement CloneStatementOnly()
		{
			BlockStatement blockStatement;
			BlockStatement blockStatement1;
			if (this.then != null)
			{
				blockStatement = this.then.CloneStatementOnly() as BlockStatement;
			}
			else
			{
				blockStatement = null;
			}
			BlockStatement blockStatement2 = blockStatement;
			if (this.@else != null)
			{
				blockStatement1 = this.@else.CloneStatementOnly() as BlockStatement;
			}
			else
			{
				blockStatement1 = null;
			}
			BlockStatement blockStatement3 = blockStatement1;
			IfStatement ifStatement = new IfStatement(base.Condition.CloneExpressionOnly(), blockStatement2, blockStatement3);
			base.CopyParentAndLabel(ifStatement);
			return ifStatement;
		}
	}
}