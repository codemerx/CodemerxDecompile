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
	public class StackAllocExpression : Telerik.JustDecompiler.Ast.Expressions.Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new StackAllocExpression.u003cget_Childrenu003ed__2(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 45;
			}
		}

		public Telerik.JustDecompiler.Ast.Expressions.Expression Expression
		{
			get;
			set;
		}

		public override TypeReference ExpressionType
		{
			get;
			set;
		}

		public override bool HasType
		{
			get
			{
				return true;
			}
		}

		public StackAllocExpression(Telerik.JustDecompiler.Ast.Expressions.Expression expression, TypeReference expressionType, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Expression(expression);
			this.set_ExpressionType(expressionType);
			return;
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression Clone()
		{
			return new StackAllocExpression(this.get_Expression().Clone(), this.get_ExpressionType(), this.instructions);
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression CloneExpressionOnly()
		{
			return new StackAllocExpression(this.get_Expression().CloneExpressionOnly(), this.get_ExpressionType(), null);
		}

		public override bool Equals(Telerik.JustDecompiler.Ast.Expressions.Expression other)
		{
			if (other as StackAllocExpression == null)
			{
				return false;
			}
			return this.get_Expression().Equals((other as StackAllocExpression).get_Expression());
		}
	}
}