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
	public class ArrayLengthExpression : Expression
	{
		private readonly TypeSystem theTypeSystem;

		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new ArrayLengthExpression.u003cget_Childrenu003ed__12(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 66;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.theTypeSystem.get_Int32();
			}
			set
			{
				this.set_ExpressionType(value);
				return;
			}
		}

		public Expression Target
		{
			get;
			set;
		}

		public ArrayLengthExpression(Expression target, TypeSystem theTypeSystem, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Target(target);
			this.theTypeSystem = theTypeSystem;
			return;
		}

		public override Expression Clone()
		{
			return new ArrayLengthExpression(this.get_Target().Clone(), this.theTypeSystem, this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new ArrayLengthExpression(this.get_Target().CloneExpressionOnly(), this.theTypeSystem, null);
		}

		public override bool Equals(Expression other)
		{
			if (other == null || other as ArrayLengthExpression == null)
			{
				return false;
			}
			return this.get_Target().Equals((other as ArrayLengthExpression).get_Target());
		}
	}
}