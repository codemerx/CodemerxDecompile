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
	public class UsingStatement : BasePdbStatement
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
				UsingStatement usingStatement = null;
				yield return usingStatement.Expression;
				if (usingStatement.body != null)
				{
					yield return usingStatement.body;
				}
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.UsingStatement;
			}
		}

		public Telerik.JustDecompiler.Ast.Expressions.Expression Expression
		{
			get;
			set;
		}

		public UsingStatement(Telerik.JustDecompiler.Ast.Expressions.Expression expression, BlockStatement body, IEnumerable<Instruction> finallyInstructions)
		{
			this.Expression = expression;
			this.Body = body;
			this.mappedFinallyInstructions = new List<Instruction>();
			if (finallyInstructions != null)
			{
				this.mappedFinallyInstructions.AddRange(finallyInstructions);
				this.mappedFinallyInstructions.Sort((Instruction x, Instruction y) => x.get_Offset().CompareTo(y.get_Offset()));
			}
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
			UsingStatement usingStatement = new UsingStatement(this.Expression.Clone(), blockStatement1, this.mappedFinallyInstructions);
			base.CopyParentAndLabel(usingStatement);
			return usingStatement;
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
			UsingStatement usingStatement = new UsingStatement(this.Expression.CloneExpressionOnly(), blockStatement1, null);
			base.CopyParentAndLabel(usingStatement);
			return usingStatement;
		}

		protected override IEnumerable<Instruction> GetOwnInstructions()
		{
			UsingStatement usingStatement = null;
			foreach (Instruction mappedFinallyInstruction in usingStatement.mappedFinallyInstructions)
			{
				yield return mappedFinallyInstruction;
			}
		}
	}
}