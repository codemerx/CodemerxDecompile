using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Telerik.JustDecompiler.Ast.Expressions
{
    public abstract class MemberReferenceExpresion : Expression
    {
		private Expression target;

		public MemberReferenceExpresion(Expression target, MemberReference memberReference, IEnumerable<Instruction> instructions)
            :base(instructions)
		{
            this.Target = target;
		    this.Member = memberReference;
        }

        public override IEnumerable<ICodeNode> Children
        {
            get
            {
                if(target != null)
                {
                    yield return target;
                }
            }
        }

		public virtual Expression Target
		{
			get
			{
				return target;
			}
			internal set
			{
				target = value;
			}
		}

        public MemberReference Member { get; protected set; }
    }
}
