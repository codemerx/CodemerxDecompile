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
	public class DelegateInvokeExpression : Expression
	{
		public ExpressionCollection Arguments
		{
			get;
			set;
		}

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new DelegateInvokeExpression.u003cget_Childrenu003ed__2(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 51;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.get_InvokeMethodReference().get_FixedReturnType();
			}
			set
			{
				throw new NotSupportedException("Expression type of delegate invocation expression can not be changed.");
			}
		}

		public MethodReference InvokeMethodReference
		{
			get;
			set;
		}

		public Expression Target
		{
			get;
			set;
		}

		public DelegateInvokeExpression(Expression target, ExpressionCollection arguments, MethodReference invokeMethodReference, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Target(target);
			this.set_Arguments(arguments);
			this.set_InvokeMethodReference(invokeMethodReference);
			return;
		}

		public override Expression Clone()
		{
			return new DelegateInvokeExpression(this.get_Target().Clone(), this.get_Arguments().Clone(), this.get_InvokeMethodReference(), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new DelegateInvokeExpression(this.get_Target().CloneExpressionOnly(), this.get_Arguments().CloneExpressionsOnly(), this.get_InvokeMethodReference(), null);
		}

		public override bool Equals(Expression other)
		{
			if (other as DelegateInvokeExpression == null)
			{
				return false;
			}
			V_0 = other as DelegateInvokeExpression;
			if (this.get_Target() != null)
			{
				if (!this.get_Target().Equals(V_0.get_Target()))
				{
					return false;
				}
			}
			else
			{
				if (V_0.get_Target() != null)
				{
					return false;
				}
			}
			return this.get_Arguments().Equals(V_0.get_Arguments());
		}
	}
}