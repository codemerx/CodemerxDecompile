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
	public class TypeOfExpression : Expression
	{
		private readonly TypeReference typeReference;

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				return new TypeOfExpression.u003cget_Childrenu003ed__3(-2);
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 35;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.typeReference;
			}
		}

		public override bool HasType
		{
			get
			{
				return true;
			}
		}

		public TypeReference Type
		{
			get;
			set;
		}

		public TypeOfExpression(TypeReference type, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Type(type);
			this.typeReference = new TypeReference("System", "Type", this.get_Type().get_Module().get_TypeSystem().get_Boolean().get_Module(), this.get_Type().get_Module().get_TypeSystem().get_Boolean().get_Scope());
			return;
		}

		public override Expression Clone()
		{
			return new TypeOfExpression(this.get_Type(), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new TypeOfExpression(this.get_Type(), null);
		}

		public override bool Equals(Expression other)
		{
			if (other as TypeOfExpression == null)
			{
				return false;
			}
			return String.op_Equality(this.get_Type().get_FullName(), (other as TypeOfExpression).get_Type().get_FullName());
		}
	}
}