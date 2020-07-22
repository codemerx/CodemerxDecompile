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
	public class SafeCastExpression : Telerik.JustDecompiler.Ast.Expressions.Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new SafeCastExpression.u003cget_Childrenu003ed__2(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 33;
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
				return this.get_TargetType();
			}
			set
			{
				this.set_TargetType(value);
				return;
			}
		}

		public override bool HasType
		{
			get
			{
				return (object)this.get_TargetType() != (object)null;
			}
		}

		public TypeReference TargetType
		{
			get;
			private set;
		}

		public SafeCastExpression(Telerik.JustDecompiler.Ast.Expressions.Expression expression, TypeReference targetType, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Expression(expression);
			this.set_TargetType(targetType);
			return;
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression Clone()
		{
			return new SafeCastExpression(this.get_Expression().Clone(), this.get_TargetType(), this.instructions);
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression CloneExpressionOnly()
		{
			return new SafeCastExpression(this.get_Expression().CloneExpressionOnly(), this.get_TargetType(), null);
		}

		public override bool Equals(Telerik.JustDecompiler.Ast.Expressions.Expression other)
		{
			if (other as SafeCastExpression == null)
			{
				return false;
			}
			V_0 = other as SafeCastExpression;
			if (String.op_Inequality(this.get_TargetType().get_FullName(), V_0.get_TargetType().get_FullName()))
			{
				return false;
			}
			return this.get_Expression().Equals(V_0.get_Expression());
		}
	}
}