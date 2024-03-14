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
	public abstract class MemberReferenceExpresion : Expression
	{
		private Expression target;

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				MemberReferenceExpresion memberReferenceExpresion = null;
				if (memberReferenceExpresion.target != null)
				{
					yield return memberReferenceExpresion.target;
				}
			}
		}

		public MemberReference Member
		{
			get;
			protected set;
		}

		public virtual Expression Target
		{
			get
			{
				return this.target;
			}
			internal set
			{
				this.target = value;
			}
		}

		public MemberReferenceExpresion(Expression target, MemberReference memberReference, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Target = target;
			this.Member = memberReference;
		}
	}
}