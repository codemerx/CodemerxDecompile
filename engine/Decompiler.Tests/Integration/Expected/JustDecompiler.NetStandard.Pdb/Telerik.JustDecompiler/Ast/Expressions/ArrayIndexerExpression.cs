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
	public class ArrayIndexerExpression : Expression, IIndexerExpression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new ArrayIndexerExpression.u003cget_Childrenu003ed__2(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 39;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				V_0 = this.get_Target().get_ExpressionType();
				if (V_0 == null)
				{
					return null;
				}
				if (this.get_Target() as ArgumentReferenceExpression != null)
				{
					V_1 = (this.get_Target() as ArgumentReferenceExpression).get_ExpressionType();
					if (V_1.get_IsByReference())
					{
						V_0 = (V_1 as ByReferenceType).get_ElementType();
					}
				}
				if (V_0.get_IsOptionalModifier())
				{
					V_0 = (V_0 as OptionalModifierType).get_ElementType();
				}
				if (V_0.get_IsRequiredModifier())
				{
					V_0 = (V_0 as RequiredModifierType).get_ElementType();
				}
				if (V_0.get_IsArray())
				{
					return (V_0 as ArrayType).get_ElementType();
				}
				if (String.op_Equality(V_0.get_FullName(), "System.Array"))
				{
					return V_0.get_Module().get_TypeSystem().get_Object();
				}
				if (!String.op_Equality(V_0.get_FullName(), "System.String"))
				{
					throw new ArgumentOutOfRangeException("Target of array indexer expression is not an array.");
				}
				return V_0.get_Module().get_TypeSystem().get_Char();
			}
			set
			{
				throw new NotSupportedException("Array indexer expression cannot change its type.");
			}
		}

		public override bool HasType
		{
			get
			{
				return this.get_Target().get_HasType();
			}
		}

		public ExpressionCollection Indices
		{
			get;
			set;
		}

		internal bool IsSimpleStore
		{
			get;
			set;
		}

		public Expression Target
		{
			get;
			set;
		}

		public ArrayIndexerExpression(Expression target, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Target(target);
			this.set_Indices(new ExpressionCollection());
			return;
		}

		public override Expression Clone()
		{
			stackVariable5 = new ArrayIndexerExpression(this.get_Target().Clone(), this.instructions);
			stackVariable5.set_Indices(this.get_Indices().Clone());
			stackVariable5.set_IsSimpleStore(this.get_IsSimpleStore());
			return stackVariable5;
		}

		public override Expression CloneExpressionOnly()
		{
			stackVariable4 = new ArrayIndexerExpression(this.get_Target().CloneExpressionOnly(), null);
			stackVariable4.set_Indices(this.get_Indices().CloneExpressionsOnly());
			stackVariable4.set_IsSimpleStore(this.get_IsSimpleStore());
			return stackVariable4;
		}

		public override bool Equals(Expression other)
		{
			if (other as ArrayIndexerExpression == null)
			{
				return false;
			}
			V_0 = other as ArrayIndexerExpression;
			if (!this.get_Target().Equals(V_0.get_Target()))
			{
				return false;
			}
			return this.get_Indices().Equals(V_0.get_Indices());
		}
	}
}