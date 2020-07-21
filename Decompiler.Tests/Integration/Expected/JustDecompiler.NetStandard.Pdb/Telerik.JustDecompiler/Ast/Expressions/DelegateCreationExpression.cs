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
	public class DelegateCreationExpression : Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new DelegateCreationExpression.u003cget_Childrenu003ed__3(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 21;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.get_Type();
			}
			set
			{
				throw new NotSupportedException("Delegate creation expression cannot change its type.");
			}
		}

		public override bool HasType
		{
			get
			{
				return true;
			}
		}

		public Expression MethodExpression
		{
			get;
			set;
		}

		public Expression Target
		{
			get;
			set;
		}

		public TypeReference Type
		{
			get;
			private set;
		}

		public bool TypeIsImplicitlyInferable
		{
			get;
			set;
		}

		public DelegateCreationExpression(TypeReference type, Expression method, Expression target, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Type(type);
			this.set_MethodExpression(method);
			this.set_Target(target);
			this.set_TypeIsImplicitlyInferable(DelegateCreationExpression.ContainsAnonymousType(type));
			return;
		}

		public override Expression Clone()
		{
			stackVariable10 = new DelegateCreationExpression(this.get_Type(), this.get_MethodExpression().Clone(), this.get_Target().Clone(), this.instructions);
			stackVariable10.set_TypeIsImplicitlyInferable(this.get_TypeIsImplicitlyInferable());
			return stackVariable10;
		}

		public override Expression CloneExpressionOnly()
		{
			stackVariable9 = new DelegateCreationExpression(this.get_Type(), this.get_MethodExpression().CloneExpressionOnly(), this.get_Target().CloneExpressionOnly(), null);
			stackVariable9.set_TypeIsImplicitlyInferable(this.get_TypeIsImplicitlyInferable());
			return stackVariable9;
		}

		private static bool ContainsAnonymousType(TypeReference type)
		{
			V_0 = type.Resolve();
			if (V_0 == null)
			{
				return false;
			}
			if (V_0.IsAnonymous())
			{
				return true;
			}
			return type.ContainsAnonymousType();
		}

		public override bool Equals(Expression other)
		{
			if (other as DelegateCreationExpression == null)
			{
				return false;
			}
			V_0 = other as DelegateCreationExpression;
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
			if (!this.get_MethodExpression().Equals(V_0.get_MethodExpression()))
			{
				return false;
			}
			return String.op_Equality(this.get_Type().get_FullName(), V_0.get_Type().get_FullName());
		}
	}
}