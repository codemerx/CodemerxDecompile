using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class BlockExpression : Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				BlockExpression blockExpression = null;
				foreach (ICodeNode expression in blockExpression.Expressions)
				{
					yield return expression;
				}
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.BlockExpression;
			}
		}

		public ExpressionCollection Expressions
		{
			get;
			set;
		}

		public override TypeReference ExpressionType
		{
			get
			{
				throw new NotSupportedException("Block expressions have no type.");
			}
			set
			{
				throw new NotSupportedException("Block expressions cannot have type.");
			}
		}

		public override bool HasType
		{
			get
			{
				return false;
			}
		}

		public BlockExpression(IEnumerable<Instruction> instructions) : this(new ExpressionCollection(), instructions)
		{
		}

		public BlockExpression(ExpressionCollection expressions, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Expressions = expressions;
		}

		public override Expression Clone()
		{
			return new BlockExpression(this.Expressions.Clone(), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new BlockExpression(this.Expressions.CloneExpressionsOnly(), null);
		}

		public override bool Equals(Expression other)
		{
			if (!(other is BlockExpression))
			{
				return false;
			}
			return this.Expressions.Equals((other as BlockExpression).Expressions);
		}
	}
}