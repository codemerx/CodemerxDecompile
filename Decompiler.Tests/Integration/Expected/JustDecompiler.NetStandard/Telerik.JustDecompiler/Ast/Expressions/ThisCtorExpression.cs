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
	public class ThisCtorExpression : MethodInvocationExpression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new ThisCtorExpression.u003cget_Childrenu003ed__7(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 53;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				throw new NotSupportedException("This constructor expression has no type.");
			}
			set
			{
				throw new NotSupportedException("This constructor expression cannot have type.");
			}
		}

		public override bool HasType
		{
			get
			{
				return false;
			}
		}

		public Expression InstanceReference
		{
			get;
			set;
		}

		public ThisCtorExpression(MethodReferenceExpression methodReferenceExpression, IEnumerable<Instruction> instructions)
		{
			base(methodReferenceExpression, instructions);
			return;
		}

		public override Expression Clone()
		{
			stackVariable6 = new ThisCtorExpression(this.get_MethodExpression().Clone() as MethodReferenceExpression, this.instructions);
			stackVariable6.set_Arguments(this.get_Arguments().Clone());
			stackVariable6.set_VirtualCall(this.get_VirtualCall());
			if (this.get_InstanceReference() != null)
			{
				stackVariable16 = this.get_InstanceReference().Clone();
			}
			else
			{
				stackVariable16 = null;
			}
			stackVariable6.set_InstanceReference(stackVariable16);
			return stackVariable6;
		}

		public override Expression CloneExpressionOnly()
		{
			stackVariable6 = new ThisCtorExpression(this.get_MethodExpression().CloneExpressionOnly() as MethodReferenceExpression, this.instructions);
			stackVariable6.set_Arguments(this.get_Arguments().CloneExpressionsOnly());
			stackVariable6.set_VirtualCall(this.get_VirtualCall());
			if (this.get_InstanceReference() != null)
			{
				stackVariable16 = this.get_InstanceReference().CloneExpressionOnly();
			}
			else
			{
				stackVariable16 = null;
			}
			stackVariable6.set_InstanceReference(stackVariable16);
			return stackVariable6;
		}

		public override bool Equals(Expression other)
		{
			if (other as ThisCtorExpression == null)
			{
				return false;
			}
			return this.Equals(other);
		}
	}
}