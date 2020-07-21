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
	public class BaseCtorExpression : MethodInvocationExpression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new BaseCtorExpression.u003cget_Childrenu003ed__7(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 52;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				throw new NotSupportedException("Base constructor expression has no type.");
			}
			set
			{
				throw new NotSupportedException("Base constructor expression cannot have type.");
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

		public BaseCtorExpression(MethodReferenceExpression method, IEnumerable<Instruction> instructions)
		{
			base(method, instructions);
			return;
		}

		public override Expression Clone()
		{
			stackVariable6 = new BaseCtorExpression(this.get_MethodExpression().Clone() as MethodReferenceExpression, this.instructions);
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
			stackVariable5 = new BaseCtorExpression(this.get_MethodExpression().CloneExpressionOnly() as MethodReferenceExpression, null);
			stackVariable5.set_Arguments(this.get_Arguments().CloneExpressionsOnly());
			stackVariable5.set_VirtualCall(this.get_VirtualCall());
			if (this.get_InstanceReference() != null)
			{
				stackVariable15 = this.get_InstanceReference().CloneExpressionOnly();
			}
			else
			{
				stackVariable15 = null;
			}
			stackVariable5.set_InstanceReference(stackVariable15);
			return stackVariable5;
		}

		public override bool Equals(Expression other)
		{
			if (other as BaseCtorExpression == null)
			{
				return false;
			}
			return this.Equals(other);
		}
	}
}