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
	public class SizeOfExpression : Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				return new SizeOfExpression.u003cget_Childrenu003ed__6(-2);
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 46;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.get_Type().get_Module().get_TypeSystem().get_UInt32();
			}
			set
			{
				throw new NotSupportedException("The return value of sizeof is always unsigned int");
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

		public SizeOfExpression(TypeReference type, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Type(type);
			return;
		}

		public override Expression Clone()
		{
			return new SizeOfExpression(this.get_Type(), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			stackVariable3 = new SizeOfExpression(this.get_Type(), null);
			stackVariable3.set_Type(this.get_Type());
			return stackVariable3;
		}

		public override bool Equals(Expression other)
		{
			if (other as SizeOfExpression == null)
			{
				return false;
			}
			return String.op_Equality(this.get_Type().get_FullName(), (other as SizeOfExpression).get_Type().get_FullName());
		}
	}
}