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
	public class DefaultObjectExpression : Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				return new DefaultObjectExpression.u003cget_Childrenu003ed__2(-2);
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 41;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.get_Type();
			}
			set
			{
				throw new NotSupportedException("Default object creation expression cannot change its type.");
			}
		}

		public override bool HasType
		{
			get
			{
				return (object)this.get_Type() != (object)null;
			}
		}

		public TypeReference Type
		{
			get;
			set;
		}

		public DefaultObjectExpression(TypeReference type, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Type(type);
			return;
		}

		public override Expression Clone()
		{
			return new DefaultObjectExpression(this.get_Type(), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new DefaultObjectExpression(this.get_Type(), null);
		}

		public override bool Equals(Expression other)
		{
			if (other as DefaultObjectExpression == null)
			{
				return false;
			}
			return String.op_Equality(this.get_Type().get_FullName(), (other as DefaultObjectExpression).get_Type().get_FullName());
		}
	}
}