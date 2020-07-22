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
	public class TypeReferenceExpression : Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				return new TypeReferenceExpression.u003cget_Childrenu003ed__6(-2);
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 43;
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
				this.set_Type(value);
				return;
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

		public TypeReferenceExpression(TypeReference type, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Type(type);
			return;
		}

		public override Expression Clone()
		{
			return new TypeReferenceExpression(this.get_Type(), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new TypeReferenceExpression(this.get_Type(), null);
		}

		public override bool Equals(Expression other)
		{
			if (other as TypeReferenceExpression == null)
			{
				return false;
			}
			V_0 = other as TypeReferenceExpression;
			return String.op_Equality(this.get_Type().get_FullName(), V_0.get_Type().get_FullName());
		}
	}
}