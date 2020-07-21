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
	public class ConditionExpression : Expression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new ConditionExpression.u003cget_Childrenu003ed__2(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 36;
			}
		}

		public Expression Condition
		{
			get;
			set;
		}

		public Expression Else
		{
			get;
			set;
		}

		public override TypeReference ExpressionType
		{
			get
			{
				return this.get_Then().get_ExpressionType();
			}
		}

		public override bool HasType
		{
			get
			{
				return this.get_Then().get_HasType();
			}
		}

		public Expression Then
		{
			get;
			set;
		}

		public ConditionExpression(Expression condition, Expression then, Expression else, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Condition(condition);
			this.set_Then(then);
			this.set_Else(else);
			return;
		}

		public override Expression Clone()
		{
			return new ConditionExpression(this.get_Condition().Clone(), this.get_Then().Clone(), this.get_Else().Clone(), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new ConditionExpression(this.get_Condition().CloneExpressionOnly(), this.get_Then().CloneExpressionOnly(), this.get_Else().CloneExpressionOnly(), null);
		}

		public override bool Equals(Expression other)
		{
			V_0 = other as ConditionExpression;
			if (V_0 == null)
			{
				return false;
			}
			if (!this.get_Condition().Equals(V_0.get_Condition()) || !this.get_Then().Equals(V_0.get_Then()))
			{
				return false;
			}
			return this.get_Else().Equals(V_0.get_Else());
		}
	}
}