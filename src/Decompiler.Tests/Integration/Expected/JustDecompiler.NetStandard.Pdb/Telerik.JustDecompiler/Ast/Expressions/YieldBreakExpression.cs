using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class YieldBreakExpression : Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.YieldBreakExpression;
			}
		}

		public YieldBreakExpression(IEnumerable<Instruction> instructions) : base(instructions)
		{
		}

		public override Expression Clone()
		{
			return new YieldBreakExpression(this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new YieldBreakExpression(null);
		}

		public override bool Equals(Expression other)
		{
			return other is YieldBreakExpression;
		}
	}
}