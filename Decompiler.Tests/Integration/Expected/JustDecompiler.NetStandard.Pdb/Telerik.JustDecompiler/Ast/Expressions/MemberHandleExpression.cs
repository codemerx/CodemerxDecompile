using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class MemberHandleExpression : Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				return Enumerable.Empty<ICodeNode>();
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 90;
			}
		}

		public Mono.Cecil.MemberReference MemberReference
		{
			get;
			private set;
		}

		public MemberHandleExpression(Mono.Cecil.MemberReference memberRef, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_MemberReference(memberRef);
			return;
		}

		public override Expression Clone()
		{
			return new MemberHandleExpression(this.get_MemberReference(), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new MemberHandleExpression(this.get_MemberReference(), null);
		}

		public override bool Equals(Expression other)
		{
			V_0 = other as MemberHandleExpression;
			if (V_0 == null)
			{
				return false;
			}
			return String.op_Equality(this.get_MemberReference().get_FullName(), V_0.get_MemberReference().get_FullName());
		}
	}
}