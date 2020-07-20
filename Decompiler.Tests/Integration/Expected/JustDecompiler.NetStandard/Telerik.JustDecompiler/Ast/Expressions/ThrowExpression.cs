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
	public class ThrowExpression : Telerik.JustDecompiler.Ast.Expressions.Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new ThrowExpression.u003cget_Childrenu003ed__6(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 6;
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
				throw new NotSupportedException("Throw expressions have no type.");
			}
			set
			{
				throw new NotSupportedException("Throw expressions cannot have type.");
			}
		}

		public override bool HasType
		{
			get
			{
				return false;
			}
		}

		public ThrowExpression(Telerik.JustDecompiler.Ast.Expressions.Expression expression, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Expression(expression);
			return;
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression Clone()
		{
			if (this.get_Expression() != null)
			{
				stackVariable4 = this.get_Expression().Clone();
			}
			else
			{
				stackVariable4 = null;
			}
			return new ThrowExpression(stackVariable4, this.instructions);
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression CloneExpressionOnly()
		{
			if (this.get_Expression() != null)
			{
				stackVariable4 = this.get_Expression().CloneExpressionOnly();
			}
			else
			{
				stackVariable4 = null;
			}
			return new ThrowExpression(stackVariable4, null);
		}

		public override bool Equals(Telerik.JustDecompiler.Ast.Expressions.Expression other)
		{
			if (other as ThrowExpression == null)
			{
				return false;
			}
			return this.get_Expression().Equals((other as ThrowExpression).get_Expression());
		}
	}
}