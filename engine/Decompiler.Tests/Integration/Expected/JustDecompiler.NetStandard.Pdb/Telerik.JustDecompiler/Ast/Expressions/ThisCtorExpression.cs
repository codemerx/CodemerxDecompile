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
				ThisCtorExpression thisCtorExpression = null;
				if (thisCtorExpression.InstanceReference != null)
				{
					yield return thisCtorExpression.InstanceReference;
				}
				foreach (ICodeNode codeNode in thisCtorExpression.u003cu003en__0())
				{
					yield return codeNode;
				}
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.ThisCtorExpression;
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

		public ThisCtorExpression(MethodReferenceExpression methodReferenceExpression, IEnumerable<Instruction> instructions) : base(methodReferenceExpression, instructions)
		{
		}

		public override Expression Clone()
		{
			Expression expression;
			ThisCtorExpression thisCtorExpression = new ThisCtorExpression(base.MethodExpression.Clone() as MethodReferenceExpression, this.instructions)
			{
				Arguments = base.Arguments.Clone(),
				VirtualCall = base.VirtualCall
			};
			if (this.InstanceReference != null)
			{
				expression = this.InstanceReference.Clone();
			}
			else
			{
				expression = null;
			}
			thisCtorExpression.InstanceReference = expression;
			return thisCtorExpression;
		}

		public override Expression CloneExpressionOnly()
		{
			Expression expression;
			ThisCtorExpression thisCtorExpression = new ThisCtorExpression(base.MethodExpression.CloneExpressionOnly() as MethodReferenceExpression, this.instructions)
			{
				Arguments = base.Arguments.CloneExpressionsOnly(),
				VirtualCall = base.VirtualCall
			};
			if (this.InstanceReference != null)
			{
				expression = this.InstanceReference.CloneExpressionOnly();
			}
			else
			{
				expression = null;
			}
			thisCtorExpression.InstanceReference = expression;
			return thisCtorExpression;
		}

		public override bool Equals(Expression other)
		{
			if (!(other is ThisCtorExpression))
			{
				return false;
			}
			return base.Equals(other);
		}
	}
}