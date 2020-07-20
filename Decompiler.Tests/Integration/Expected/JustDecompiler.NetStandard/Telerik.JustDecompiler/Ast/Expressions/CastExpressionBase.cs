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
	public abstract class CastExpressionBase : Telerik.JustDecompiler.Ast.Expressions.Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new CastExpressionBase.u003cget_Childrenu003ed__15(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
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
				return true;
			}
		}

		public TypeReference TargetType
		{
			get;
			set;
		}

		public CastExpressionBase(Telerik.JustDecompiler.Ast.Expressions.Expression expression, TypeReference targetType, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Expression(expression);
			this.set_TargetType(targetType);
			return;
		}

		public override bool Equals(Telerik.JustDecompiler.Ast.Expressions.Expression other)
		{
			if (other as CastExpressionBase == null)
			{
				return false;
			}
			if (String.op_Inequality(this.get_TargetType().get_FullName(), (other as CastExpressionBase).get_TargetType().get_FullName()))
			{
				return false;
			}
			return this.get_Expression().Equals((other as CastExpressionBase).get_Expression());
		}
	}
}