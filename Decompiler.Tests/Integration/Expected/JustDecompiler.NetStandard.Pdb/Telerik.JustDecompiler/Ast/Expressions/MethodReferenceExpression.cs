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
				return 20;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				if (this.expressionType == null)
				{
					this.expressionType = this.get_Method().get_FixedReturnType();
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
					this.methodDefinition = this.get_Method().Resolve();
					this.resolved = true;
				}
				return this.methodDefinition;
			}
		}

		public MethodReferenceExpression(Expression target, MethodReference method, IEnumerable<Instruction> instructions)
		{
			base(target, method, instructions);
			this.expressionType = null;
			this.set_Target(target);
			this.set_Method(method);
			this.set_Member(method);
			return;
		}

		public override Expression Clone()
		{
			if (this.get_Target() != null)
			{
				stackVariable4 = this.get_Target().Clone();
			}
			else
			{
				stackVariable4 = null;
			}
			V_0 = new MethodReferenceExpression(stackVariable4, this.get_Method(), this.instructions);
			this.CopyFields(V_0);
			return V_0;
		}

		public override Expression CloneExpressionOnly()
		{
			if (this.get_Target() != null)
			{
				stackVariable4 = this.get_Target().CloneExpressionOnly();
			}
			else
			{
				stackVariable4 = null;
			}
			V_0 = new MethodReferenceExpression(stackVariable4, this.get_Method(), null);
			this.CopyFields(V_0);
			return V_0;
		}

		private void CopyFields(MethodReferenceExpression clone)
		{
			clone.resolved = this.resolved;
			clone.methodDefinition = this.methodDefinition;
			clone.expressionType = this.expressionType;
			return;
		}

		public override bool Equals(Expression other)
		{
			if (other == null)
			{
				return false;
			}
			if (other as MethodReferenceExpression == null)
			{
				return false;
			}
			V_0 = other as MethodReferenceExpression;
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
			if (String.op_Inequality(this.get_Method().get_FullName(), V_0.get_Method().get_FullName()))
			{
				return false;
			}
			return true;
		}
	}
}