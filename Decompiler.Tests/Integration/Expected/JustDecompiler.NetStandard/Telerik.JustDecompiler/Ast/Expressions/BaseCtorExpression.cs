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
				BaseCtorExpression baseCtorExpression = null;
				if (baseCtorExpression.InstanceReference != null)
				{
					yield return baseCtorExpression.InstanceReference;
				}
				foreach (ICodeNode codeNode in baseCtorExpression.u003cu003en__0())
				{
					yield return codeNode;
				}
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.BaseCtorExpression;
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

		public BaseCtorExpression(MethodReferenceExpression method, IEnumerable<Instruction> instructions) : base(method, instructions)
		{
		}

		public override Expression Clone()
		{
			Expression expression;
			BaseCtorExpression baseCtorExpression = new BaseCtorExpression(base.MethodExpression.Clone() as MethodReferenceExpression, this.instructions)
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
			baseCtorExpression.InstanceReference = expression;
			return baseCtorExpression;
		}

		public override Expression CloneExpressionOnly()
		{
			Expression expression;
			BaseCtorExpression baseCtorExpression = new BaseCtorExpression(base.MethodExpression.CloneExpressionOnly() as MethodReferenceExpression, null)
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
			baseCtorExpression.InstanceReference = expression;
			return baseCtorExpression;
		}

		public override bool Equals(Expression other)
		{
			if (!(other is BaseCtorExpression))
			{
				return false;
			}
			return base.Equals(other);
		}
	}
}