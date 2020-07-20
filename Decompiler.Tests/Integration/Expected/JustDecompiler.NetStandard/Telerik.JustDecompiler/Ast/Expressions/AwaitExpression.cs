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
	public class AwaitExpression : Telerik.JustDecompiler.Ast.Expressions.Expression
	{
		private Telerik.JustDecompiler.Ast.Expressions.Expression expression;

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new AwaitExpression.u003cget_Childrenu003ed__11(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 65;
			}
		}

		public Telerik.JustDecompiler.Ast.Expressions.Expression Expression
		{
			get
			{
				return this.expression;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("expression");
				}
				this.expression = value;
				return;
			}
		}

		public AwaitExpression(Telerik.JustDecompiler.Ast.Expressions.Expression expression, TypeReference type, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			if (expression == null)
			{
				throw new ArgumentNullException("expression");
			}
			this.expression = expression;
			this.set_ExpressionType(type);
			return;
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression Clone()
		{
			return new AwaitExpression(this.get_Expression().Clone(), this.get_ExpressionType(), this.instructions);
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression CloneExpressionOnly()
		{
			return new AwaitExpression(this.get_Expression().CloneExpressionOnly(), this.get_ExpressionType(), null);
		}

		public override bool Equals(Telerik.JustDecompiler.Ast.Expressions.Expression other)
		{
			if (other.get_CodeNodeType() != 65)
			{
				return false;
			}
			return (other as AwaitExpression).expression.Equals(this.expression);
		}
	}
}