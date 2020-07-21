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
				stackVariable1 = new MemberReferenceExpresion.u003cget_Childrenu003ed__3(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
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
				return;
			}
		}

		public MemberReferenceExpresion(Expression target, MemberReference memberReference, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Target(target);
			this.set_Member(memberReference);
			return;
		}
	}
}