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
	public class DynamicMemberReferenceExpression : Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new DynamicMemberReferenceExpression.u003cget_Childrenu003ed__15(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 59;
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
				return this.get_GenericTypeArguments() != null;
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

		public DynamicMemberReferenceExpression(Expression target, string memberName, TypeReference type, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_IsMethodInvocation(false);
			this.set_Target(target);
			this.set_MemberName(memberName);
			this.set_ExpressionType(type);
			return;
		}

		public DynamicMemberReferenceExpression(Expression target, string memberName, TypeReference type, IEnumerable<Instruction> instructions, IEnumerable<Expression> invocationArguments, IEnumerable<TypeReference> genericTypeArguments = null)
		{
			base(instructions);
			this.set_IsMethodInvocation(true);
			this.set_Target(target);
			this.set_MemberName(memberName);
			this.set_ExpressionType(type);
			this.set_InvocationArguments(new ExpressionCollection());
			V_0 = invocationArguments.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					this.get_InvocationArguments().Add(V_1);
				}
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
			if (genericTypeArguments != null)
			{
				this.set_GenericTypeArguments(new List<TypeReference>(genericTypeArguments));
			}
			return;
		}

		public override Expression Clone()
		{
			V_0 = new DynamicMemberReferenceExpression(this.get_Target().Clone(), this.get_MemberName(), this.get_ExpressionType(), this.instructions);
			if (this.get_IsMethodInvocation())
			{
				V_0.set_IsMethodInvocation(true);
				V_0.set_InvocationArguments(this.get_InvocationArguments().Clone());
				if (this.get_IsGenericMethod())
				{
					V_0.set_GenericTypeArguments(new List<TypeReference>(this.get_GenericTypeArguments()));
				}
			}
			return V_0;
		}

		public override Expression CloneExpressionOnly()
		{
			V_0 = new DynamicMemberReferenceExpression(this.get_Target().CloneExpressionOnly(), this.get_MemberName(), this.get_ExpressionType(), null);
			if (this.get_IsMethodInvocation())
			{
				V_0.set_IsMethodInvocation(true);
				V_0.set_InvocationArguments(this.get_InvocationArguments().CloneExpressionsOnly());
				if (this.get_IsGenericMethod())
				{
					V_0.set_GenericTypeArguments(new List<TypeReference>(this.get_GenericTypeArguments()));
				}
			}
			return V_0;
		}

		public override bool Equals(Expression other)
		{
			if (other.get_CodeNodeType() != 59)
			{
				return false;
			}
			V_0 = other as DynamicMemberReferenceExpression;
			if (!this.get_Target().Equals(V_0.get_Target()) || String.op_Inequality(this.get_MemberName(), V_0.get_MemberName()) || this.get_IsMethodInvocation() != V_0.get_IsMethodInvocation())
			{
				return false;
			}
			if (!this.get_IsMethodInvocation())
			{
				return true;
			}
			if (!this.get_InvocationArguments().Equals(V_0.get_InvocationArguments()) || this.get_IsGenericMethod() != V_0.get_IsGenericMethod())
			{
				return false;
			}
			if (!this.get_IsGenericMethod())
			{
				return true;
			}
			V_1 = this.get_GenericTypeArguments().GetEnumerator();
			try
			{
				V_2 = V_0.get_GenericTypeArguments().GetEnumerator();
				try
				{
					do
					{
						V_3 = V_1.MoveNext();
						if (V_3 == V_2.MoveNext())
						{
							if (V_3)
							{
								continue;
							}
							V_5 = true;
							goto Label0;
						}
						else
						{
							V_5 = false;
							goto Label0;
						}
					}
					while ((object)V_1.get_Current() == (object)V_2.get_Current());
					V_5 = false;
				}
				finally
				{
					if (V_2 != null)
					{
						V_2.Dispose();
					}
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
		Label0:
			return V_5;
		}
	}
}