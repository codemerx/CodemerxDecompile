using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Linq;
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
				return Telerik.JustDecompiler.Ast.CodeNodeType.MemberHandleExpression;
			}
		}

		public Mono.Cecil.MemberReference MemberReference
		{
			get;
			private set;
		}

		public MemberHandleExpression(Mono.Cecil.MemberReference memberRef, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.MemberReference = memberRef;
		}

		public override Expression Clone()
		{
			return new MemberHandleExpression(this.MemberReference, this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new MemberHandleExpression(this.MemberReference, null);
		}

		public override bool Equals(Expression other)
		{
			MemberHandleExpression memberHandleExpression = other as MemberHandleExpression;
			if (memberHandleExpression == null)
			{
				return false;
			}
			return this.MemberReference.get_FullName() == memberHandleExpression.MemberReference.get_FullName();
		}
	}
}