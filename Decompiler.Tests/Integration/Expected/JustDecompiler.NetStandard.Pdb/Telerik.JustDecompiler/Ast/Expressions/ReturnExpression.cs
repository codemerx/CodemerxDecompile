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
	public class ReturnExpression : Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new ReturnExpression.u003cget_Childrenu003ed__6(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 57;
			}
		}

		public override TypeReference ExpressionType
		{
			get
			{
				throw new NotSupportedException("Return expressions have no type.");
			}
			set
			{
				throw new NotSupportedException("Return expressions cannot have type.");
			}
		}

		public override bool HasType
		{
			get
			{
				return false;
			}
		}

		public Expression Value
		{
			get;
			set;
		}

		public ReturnExpression(Expression value, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Value(value);
			return;
		}

		public override Expression Clone()
		{
			if (this.get_Value() != null)
			{
				stackVariable4 = this.get_Value().Clone();
			}
			else
			{
				stackVariable4 = null;
			}
			return new ReturnExpression(stackVariable4, this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			if (this.get_Value() != null)
			{
				stackVariable4 = this.get_Value().CloneExpressionOnly();
			}
			else
			{
				stackVariable4 = null;
			}
			return new ReturnExpression(stackVariable4, null);
		}

		public override bool Equals(Expression other)
		{
			if (other as ReturnExpression == null)
			{
				return false;
			}
			V_0 = other as ReturnExpression;
			if (this.get_Value() == null)
			{
				return V_0.get_Value() == null;
			}
			return this.get_Value().Equals(V_0.get_Value());
		}
	}
}