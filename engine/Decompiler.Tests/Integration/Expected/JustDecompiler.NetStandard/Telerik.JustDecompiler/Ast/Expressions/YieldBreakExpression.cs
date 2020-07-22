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
				return new YieldBreakExpression.u003cget_Childrenu003ed__2(-2);
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 55;
			}
		}

		public YieldBreakExpression(IEnumerable<Instruction> instructions)
		{
			base(instructions);
			return;
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
			return other as YieldBreakExpression != null;
		}
	}
}