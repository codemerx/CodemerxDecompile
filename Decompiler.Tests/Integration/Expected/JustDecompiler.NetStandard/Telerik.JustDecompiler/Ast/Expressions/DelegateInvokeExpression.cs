using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
				DelegateInvokeExpression delegateInvokeExpression = null;
				yield return delegateInvokeExpression.Target;
				foreach (ICodeNode argument in delegateInvokeExpression.Arguments)
				{
					yield return argument;
				}
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.DelegateInvokeExpression;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.InvokeMethodReference.FixedReturnType;
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

		public DelegateInvokeExpression(Expression target, ExpressionCollection arguments, MethodReference invokeMethodReference, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Target = target;
			this.Arguments = arguments;
			this.InvokeMethodReference = invokeMethodReference;
		}

		public override Expression Clone()
		{
			return new DelegateInvokeExpression(this.Target.Clone(), this.Arguments.Clone(), this.InvokeMethodReference, this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new DelegateInvokeExpression(this.Target.CloneExpressionOnly(), this.Arguments.CloneExpressionsOnly(), this.InvokeMethodReference, null);
		}

		public override bool Equals(Expression other)
		{
			if (!(other is DelegateInvokeExpression))
			{
				return false;
			}
			DelegateInvokeExpression delegateInvokeExpression = other as DelegateInvokeExpression;
			if (this.Target == null)
			{
				if (delegateInvokeExpression.Target != null)
				{
					return false;
				}
			}
			else if (!this.Target.Equals(delegateInvokeExpression.Target))
			{
				return false;
			}
			return this.Arguments.Equals(delegateInvokeExpression.Arguments);
		}
	}
}