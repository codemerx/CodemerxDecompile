using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Extensions;
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
				DelegateCreationExpression delegateCreationExpression = null;
				yield return delegateCreationExpression.Target;
				yield return delegateCreationExpression.MethodExpression;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.DelegateCreationExpression;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.Type;
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

		public DelegateCreationExpression(TypeReference type, Expression method, Expression target, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.Type = type;
			this.MethodExpression = method;
			this.Target = target;
			this.TypeIsImplicitlyInferable = DelegateCreationExpression.ContainsAnonymousType(type);
		}

		public override Expression Clone()
		{
			return new DelegateCreationExpression(this.Type, this.MethodExpression.Clone(), this.Target.Clone(), this.instructions)
			{
				TypeIsImplicitlyInferable = this.TypeIsImplicitlyInferable
			};
		}

		public override Expression CloneExpressionOnly()
		{
			return new DelegateCreationExpression(this.Type, this.MethodExpression.CloneExpressionOnly(), this.Target.CloneExpressionOnly(), null)
			{
				TypeIsImplicitlyInferable = this.TypeIsImplicitlyInferable
			};
		}

		private static bool ContainsAnonymousType(TypeReference type)
		{
			TypeDefinition typeDefinition = type.Resolve();
			if (typeDefinition == null)
			{
				return false;
			}
			if (typeDefinition.IsAnonymous())
			{
				return true;
			}
			return type.ContainsAnonymousType();
		}

		public override bool Equals(Expression other)
		{
			if (!(other is DelegateCreationExpression))
			{
				return false;
			}
			DelegateCreationExpression delegateCreationExpression = other as DelegateCreationExpression;
			if (this.Target == null)
			{
				if (delegateCreationExpression.Target != null)
				{
					return false;
				}
			}
			else if (!this.Target.Equals(delegateCreationExpression.Target))
			{
				return false;
			}
			if (!this.MethodExpression.Equals(delegateCreationExpression.MethodExpression))
			{
				return false;
			}
			return this.Type.get_FullName() == delegateCreationExpression.Type.get_FullName();
		}
	}
}