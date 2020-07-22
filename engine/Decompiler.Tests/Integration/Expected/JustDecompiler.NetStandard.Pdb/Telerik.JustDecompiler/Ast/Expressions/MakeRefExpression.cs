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
	public class MakeRefExpression : Telerik.JustDecompiler.Ast.Expressions.Expression
	{
		private readonly TypeReference theType;

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new MakeRefExpression.u003cget_Childrenu003ed__3(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 47;
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
				return this.theType;
			}
			set
			{
				throw new NotSupportedException("Makeref expression cannot change its type.");
			}
		}

		public override bool HasType
		{
			get
			{
				return true;
			}
		}

		public MakeRefExpression(Telerik.JustDecompiler.Ast.Expressions.Expression expression, TypeReference makeRefType, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Expression(expression);
			this.theType = makeRefType;
			return;
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression Clone()
		{
			return new MakeRefExpression(this.get_Expression().Clone(), this.theType, this.instructions);
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression CloneExpressionOnly()
		{
			return new MakeRefExpression(this.get_Expression().CloneExpressionOnly(), this.theType, null);
		}

		public override bool Equals(Telerik.JustDecompiler.Ast.Expressions.Expression other)
		{
			if (other as MakeRefExpression == null)
			{
				return false;
			}
			V_0 = other as MakeRefExpression;
			if (String.op_Inequality(this.theType.get_FullName(), V_0.theType.get_FullName()))
			{
				return false;
			}
			return this.get_Expression().Equals(V_0.get_Expression());
		}
	}
}