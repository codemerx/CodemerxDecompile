using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Ast.Statements
{
	public class LockStatement : BasePdbStatement
	{
		private BlockStatement body;

		private readonly List<Instruction> mappedFinallyInstructions;

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
				LockStatement lockStatement = null;
				if (lockStatement.Expression != null)
				{
					yield return lockStatement.Expression;
				}
				if (lockStatement.body != null)
				{
					yield return lockStatement.body;
				}
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.LockStatement;
			}
		}

		public Telerik.JustDecompiler.Ast.Expressions.Expression Expression
		{
			get;
			set;
		}

		public LockStatement(Telerik.JustDecompiler.Ast.Expressions.Expression expression, BlockStatement body, IEnumerable<Instruction> finallyInstructions)
		{
			this.Expression = expression;
			this.body = body;
			this.mappedFinallyInstructions = new List<Instruction>();
			if (finallyInstructions != null)
			{
				this.mappedFinallyInstructions.AddRange(finallyInstructions);
				this.mappedFinallyInstructions.Sort((Instruction x, Instruction y) => x.Offset.CompareTo(y.Offset));
			}
		}

		public override Statement Clone()
		{
			BlockStatement blockStatement = null;
			if (this.body != null)
			{
				blockStatement = this.body.Clone() as BlockStatement;
			}
			LockStatement lockStatement = new LockStatement(this.Expression.Clone(), blockStatement, this.mappedFinallyInstructions);
			base.CopyParentAndLabel(lockStatement);
			return lockStatement;
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
			LockStatement lockStatement = new LockStatement(this.Expression.CloneExpressionOnly(), blockStatement1, null);
			base.CopyParentAndLabel(lockStatement);
			return lockStatement;
		}

		protected override IEnumerable<Instruction> GetOwnInstructions()
		{
			LockStatement lockStatement = null;
			foreach (Instruction mappedFinallyInstruction in lockStatement.mappedFinallyInstructions)
			{
				yield return mappedFinallyInstruction;
			}
		}
	}
}