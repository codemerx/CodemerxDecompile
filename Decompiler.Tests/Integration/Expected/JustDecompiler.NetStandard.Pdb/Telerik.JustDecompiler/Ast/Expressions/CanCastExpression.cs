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
	public class CanCastExpression : Telerik.JustDecompiler.Ast.Expressions.Expression
	{
		private Telerik.JustDecompiler.Ast.Expressions.Expression expression;

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new CanCastExpression.u003cget_Childrenu003ed__3(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 34;
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
				this.expression = value;
				return;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.get_TargetType().get_Module().get_TypeSystem().get_Boolean();
			}
			set
			{
				throw new NotSupportedException("Can cast expression is always boolean.");
			}
		}

		public override bool HasType
		{
			get
			{
				return true;
			}
		}

		public TypeReference TargetType
		{
			get;
			set;
		}

		public CanCastExpression(Telerik.JustDecompiler.Ast.Expressions.Expression expression, TypeReference targetType, IList<Instruction> instructions)
		{
			base(instructions);
			this.set_Expression(expression);
			this.set_TargetType(targetType);
			return;
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression Clone()
		{
			return new CanCastExpression(this.get_Expression().Clone(), this.get_TargetType(), this.instructions);
		}

		public override Telerik.JustDecompiler.Ast.Expressions.Expression CloneExpressionOnly()
		{
			return new CanCastExpression(this.get_Expression().CloneExpressionOnly(), this.get_TargetType(), null);
		}

		public override bool Equals(Telerik.JustDecompiler.Ast.Expressions.Expression other)
		{
			if (other as CanCastExpression == null)
			{
				return false;
			}
			V_0 = other as CanCastExpression;
			if (!String.op_Equality(this.get_TargetType().get_FullName(), V_0.get_TargetType().get_FullName()))
			{
				return false;
			}
			return this.get_Expression().Equals(V_0.get_Expression());
		}
	}
}