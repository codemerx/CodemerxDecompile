using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class ParenthesesExpression : Telerik.JustDecompiler.Ast.Expressions.Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new ParenthesesExpression.u003cget_Childrenu003ed__12(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 87;
			}
		}

		public Telerik.JustDecompiler.Ast.Expressions.Expression Expression
		{
			get;
			set;
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.get_Expression().get_ExpressionType();
			}
			set
			{
				throw new NotSupportedException("Parentheses Expression cannot change its type.");
			}
		}

		public ParenthesesExpression(Telerik.JustDecompiler.Ast.Expressions.Expression expression)
		{
			this(expression, Enumerable.Empty<Instruction>());
			return;
		}

		public ParenthesesExpression(Telerik.JustDecompiler.Ast.Expressions.Expression expression, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Expression(expression);
			return;
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression Clone()
		{
			return new ParenthesesExpression(this.get_Expression().Clone(), this.instructions);
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression CloneExpressionOnly()
		{
			return new ParenthesesExpression(this.get_Expression().Clone(), null);
		}

		public override bool Equals(Telerik.JustDecompiler.Ast.Expressions.Expression other)
		{
			V_0 = other as ParenthesesExpression;
			if (V_0 == null)
			{
				return false;
			}
			return this.get_Expression().Equals(V_0.get_Expression());
		}
	}
}