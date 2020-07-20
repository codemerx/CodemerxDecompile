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
	public class YieldReturnExpression : Telerik.JustDecompiler.Ast.Expressions.Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new YieldReturnExpression.u003cget_Childrenu003ed__6(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 54;
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
				this.get_Expression().set_ExpressionType(value);
				return;
			}
		}

		public YieldReturnExpression(Telerik.JustDecompiler.Ast.Expressions.Expression expression, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Expression(expression);
			return;
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression Clone()
		{
			return new YieldReturnExpression(this.get_Expression().Clone(), this.instructions);
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression CloneExpressionOnly()
		{
			return new YieldReturnExpression(this.get_Expression().CloneExpressionOnly(), null);
		}

		public override bool Equals(Telerik.JustDecompiler.Ast.Expressions.Expression other)
		{
			if (other as YieldReturnExpression == null)
			{
				return false;
			}
			V_0 = other as YieldReturnExpression;
			if (this.get_Expression() == null)
			{
				return V_0.get_Expression() == null;
			}
			return this.get_Expression().Equals(V_0.get_Expression());
		}
	}
}