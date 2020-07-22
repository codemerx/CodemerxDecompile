using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class ImplicitCastExpression : CastExpressionBase
	{
		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 32;
			}
		}

		public ImplicitCastExpression(Telerik.JustDecompiler.Ast.Expressions.Expression expression, TypeReference targetType, IEnumerable<Instruction> instructions)
		{
			base(expression, targetType, instructions);
			return;
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression Clone()
		{
			return new ImplicitCastExpression(this.get_Expression().Clone(), this.get_TargetType(), this.instructions);
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression CloneExpressionOnly()
		{
			return new ImplicitCastExpression(this.get_Expression().Clone(), this.get_TargetType(), null);
		}
	}
}