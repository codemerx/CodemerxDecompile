using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class MethodReferenceExpression : MemberReferenceExpresion
	{
		private Mono.Cecil.MethodDefinition methodDefinition;

		private bool resolved;

		private TypeReference expressionType;

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.MethodReferenceExpression;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				if (this.expressionType == null)
				{
					this.expressionType = this.Method.FixedReturnType;
				}
				return this.expressionType;
			}
			set
			{
				throw new NotSupportedException("Type of MethodReferenceExpression can not be changed.");
			}
		}

		public override bool HasType
		{
			get
			{
				return true;
			}
		}

		public MethodReference Method
		{
			get;
			set;
		}

		public Mono.Cecil.MethodDefinition MethodDefinition
		{
			get
			{
				if (!this.resolved)
				{
					this.methodDefinition = this.Method.Resolve();
					this.resolved = true;
				}
				return this.methodDefinition;
			}
		}

		public MethodReferenceExpression(Expression target, MethodReference method, IEnumerable<Instruction> instructions) : base(target, method, instructions)
		{
			this.expressionType = null;
			this.Target = target;
			this.Method = method;
			base.Member = method;
		}

		public override Expression Clone()
		{
			Expression expression;
			if (this.Target != null)
			{
				expression = this.Target.Clone();
			}
			else
			{
				expression = null;
			}
			MethodReferenceExpression methodReferenceExpression = new MethodReferenceExpression(expression, this.Method, this.instructions);
			this.CopyFields(methodReferenceExpression);
			return methodReferenceExpression;
		}

		public override Expression CloneExpressionOnly()
		{
			Expression expression;
			if (this.Target != null)
			{
				expression = this.Target.CloneExpressionOnly();
			}
			else
			{
				expression = null;
			}
			MethodReferenceExpression methodReferenceExpression = new MethodReferenceExpression(expression, this.Method, null);
			this.CopyFields(methodReferenceExpression);
			return methodReferenceExpression;
		}

		private void CopyFields(MethodReferenceExpression clone)
		{
			clone.resolved = this.resolved;
			clone.methodDefinition = this.methodDefinition;
			clone.expressionType = this.expressionType;
		}

		public override bool Equals(Expression other)
		{
			if (other == null)
			{
				return false;
			}
			if (!(other is MethodReferenceExpression))
			{
				return false;
			}
			MethodReferenceExpression methodReferenceExpression = other as MethodReferenceExpression;
			if (this.Target == null)
			{
				if (methodReferenceExpression.Target != null)
				{
					return false;
				}
			}
			else if (!this.Target.Equals(methodReferenceExpression.Target))
			{
				return false;
			}
			if (this.Method.FullName != methodReferenceExpression.Method.FullName)
			{
				return false;
			}
			return true;
		}
	}
}