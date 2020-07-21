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
	public class FieldReferenceExpression : Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new FieldReferenceExpression.u003cget_Childrenu003ed__6(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 30;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.get_Field().get_FieldType();
			}
			set
			{
				throw new NotSupportedException("Field expression cannot change its type.");
			}
		}

		public FieldReference Field
		{
			get;
			private set;
		}

		public override bool HasType
		{
			get
			{
				return true;
			}
		}

		internal bool IsSimpleStore
		{
			get;
			set;
		}

		public bool IsStatic
		{
			get
			{
				return this.get_Target() == null;
			}
		}

		public Expression Target
		{
			get;
			set;
		}

		public FieldReferenceExpression(Expression target, FieldReference field, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Target(target);
			this.set_Field(field);
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
			stackVariable9 = new FieldReferenceExpression(stackVariable4, this.get_Field(), this.instructions);
			stackVariable9.set_IsSimpleStore(this.get_IsSimpleStore());
			return stackVariable9;
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
			stackVariable8 = new FieldReferenceExpression(stackVariable4, this.get_Field(), null);
			stackVariable8.set_IsSimpleStore(this.get_IsSimpleStore());
			return stackVariable8;
		}

		public override bool Equals(Expression other)
		{
			if (other as FieldReferenceExpression == null)
			{
				return false;
			}
			V_0 = other as FieldReferenceExpression;
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
			return String.op_Equality(this.get_Field().get_FullName(), V_0.get_Field().get_FullName());
		}
	}
}