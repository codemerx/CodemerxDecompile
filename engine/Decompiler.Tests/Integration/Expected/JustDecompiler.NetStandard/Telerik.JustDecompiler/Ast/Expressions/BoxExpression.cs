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
	public class BoxExpression : Expression
	{
		private readonly TypeSystem theTypeSystem;

		public TypeReference BoxedAs
		{
			get;
			private set;
		}

		public Expression BoxedExpression
		{
			get;
			set;
		}

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new BoxExpression.u003cget_Childrenu003ed__15(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 62;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.theTypeSystem.get_Object();
			}
			set
			{
				throw new NotSupportedException("Expression type of box expression can not be changed.");
			}
		}

		public bool IsAutoBox
		{
			get;
			set;
		}

		public BoxExpression(Expression boxedExpression, TypeSystem theTypeSystem, TypeReference boxedAsType, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.theTypeSystem = theTypeSystem;
			this.set_BoxedExpression(boxedExpression);
			this.set_BoxedAs(boxedAsType);
			this.set_IsAutoBox(false);
			return;
		}

		public override Expression Clone()
		{
			stackVariable9 = new BoxExpression(this.get_BoxedExpression().Clone(), this.theTypeSystem, this.get_BoxedAs(), this.instructions);
			stackVariable9.set_IsAutoBox(this.get_IsAutoBox());
			return stackVariable9;
		}

		public override Expression CloneExpressionOnly()
		{
			stackVariable8 = new BoxExpression(this.get_BoxedExpression().CloneExpressionOnly(), this.theTypeSystem, this.get_BoxedAs(), null);
			stackVariable8.set_IsAutoBox(this.get_IsAutoBox());
			return stackVariable8;
		}

		public override bool Equals(Expression other)
		{
			V_0 = other as BoxExpression;
			if (V_0 == null)
			{
				return false;
			}
			if (!this.get_BoxedExpression().Equals(V_0.get_BoxedExpression()))
			{
				return false;
			}
			return String.op_Equality(this.get_BoxedAs().get_FullName(), V_0.get_BoxedAs().get_FullName());
		}
	}
}