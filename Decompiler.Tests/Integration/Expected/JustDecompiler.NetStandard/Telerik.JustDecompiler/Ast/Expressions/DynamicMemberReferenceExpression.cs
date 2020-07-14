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
	public class DynamicMemberReferenceExpression : Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				DynamicMemberReferenceExpression dynamicMemberReferenceExpression = null;
				yield return dynamicMemberReferenceExpression.Target;
				if (dynamicMemberReferenceExpression.InvocationArguments != null)
				{
					foreach (ICodeNode invocationArgument in dynamicMemberReferenceExpression.InvocationArguments)
					{
						yield return invocationArgument;
					}
				}
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.DynamicMemberReferenceExpression;
			}
		}

		public List<TypeReference> GenericTypeArguments
		{
			get;
			private set;
		}

		public ExpressionCollection InvocationArguments
		{
			get;
			internal set;
		}

		public bool IsGenericMethod
		{
			get
			{
				return this.GenericTypeArguments != null;
			}
		}

		public bool IsMethodInvocation
		{
			get;
			private set;
		}

		public string MemberName
		{
			get;
			private set;
		}

		public Expression Target
		{
			get;
			internal set;
		}

		public DynamicMemberReferenceExpression(Expression target, string memberName, TypeReference type, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.IsMethodInvocation = false;
			this.Target = target;
			this.MemberName = memberName;
			this.ExpressionType = type;
		}

		public DynamicMemberReferenceExpression(Expression target, string memberName, TypeReference type, IEnumerable<Instruction> instructions, IEnumerable<Expression> invocationArguments, IEnumerable<TypeReference> genericTypeArguments = null) : base(instructions)
		{
			this.IsMethodInvocation = true;
			this.Target = target;
			this.MemberName = memberName;
			this.ExpressionType = type;
			this.InvocationArguments = new ExpressionCollection();
			foreach (Expression invocationArgument in invocationArguments)
			{
				this.InvocationArguments.Add(invocationArgument);
			}
			if (genericTypeArguments != null)
			{
				this.GenericTypeArguments = new List<TypeReference>(genericTypeArguments);
			}
		}

		public override Expression Clone()
		{
			DynamicMemberReferenceExpression dynamicMemberReferenceExpression = new DynamicMemberReferenceExpression(this.Target.Clone(), this.MemberName, this.ExpressionType, this.instructions);
			if (this.IsMethodInvocation)
			{
				dynamicMemberReferenceExpression.IsMethodInvocation = true;
				dynamicMemberReferenceExpression.InvocationArguments = this.InvocationArguments.Clone();
				if (this.IsGenericMethod)
				{
					dynamicMemberReferenceExpression.GenericTypeArguments = new List<TypeReference>(this.GenericTypeArguments);
				}
			}
			return dynamicMemberReferenceExpression;
		}

		public override Expression CloneExpressionOnly()
		{
			DynamicMemberReferenceExpression dynamicMemberReferenceExpression = new DynamicMemberReferenceExpression(this.Target.CloneExpressionOnly(), this.MemberName, this.ExpressionType, null);
			if (this.IsMethodInvocation)
			{
				dynamicMemberReferenceExpression.IsMethodInvocation = true;
				dynamicMemberReferenceExpression.InvocationArguments = this.InvocationArguments.CloneExpressionsOnly();
				if (this.IsGenericMethod)
				{
					dynamicMemberReferenceExpression.GenericTypeArguments = new List<TypeReference>(this.GenericTypeArguments);
				}
			}
			return dynamicMemberReferenceExpression;
		}

		public override bool Equals(Expression other)
		{
			bool flag;
			if (other.CodeNodeType != Telerik.JustDecompiler.Ast.CodeNodeType.DynamicMemberReferenceExpression)
			{
				return false;
			}
			DynamicMemberReferenceExpression dynamicMemberReferenceExpression = other as DynamicMemberReferenceExpression;
			if (!this.Target.Equals(dynamicMemberReferenceExpression.Target) || this.MemberName != dynamicMemberReferenceExpression.MemberName || this.IsMethodInvocation != dynamicMemberReferenceExpression.IsMethodInvocation)
			{
				return false;
			}
			if (!this.IsMethodInvocation)
			{
				return true;
			}
			if (!this.InvocationArguments.Equals(dynamicMemberReferenceExpression.InvocationArguments) || this.IsGenericMethod != dynamicMemberReferenceExpression.IsGenericMethod)
			{
				return false;
			}
			if (!this.IsGenericMethod)
			{
				return true;
			}
			using (IEnumerator<TypeReference> enumerator = this.GenericTypeArguments.GetEnumerator())
			{
				using (IEnumerator<TypeReference> enumerator1 = dynamicMemberReferenceExpression.GenericTypeArguments.GetEnumerator())
				{
					do
					{
						bool flag1 = enumerator.MoveNext();
						if (flag1 == enumerator1.MoveNext())
						{
							if (flag1)
							{
								continue;
							}
							flag = true;
							return flag;
						}
						else
						{
							flag = false;
							return flag;
						}
					}
					while ((object)enumerator.Current == (object)enumerator1.Current);
					flag = false;
				}
			}
			return flag;
		}
	}
}