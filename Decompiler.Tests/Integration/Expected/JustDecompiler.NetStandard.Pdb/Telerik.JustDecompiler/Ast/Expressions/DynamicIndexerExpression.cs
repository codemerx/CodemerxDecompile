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
	public class DynamicIndexerExpression : Expression, IIndexerExpression
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = new DynamicIndexerExpression.u003cget_Childrenu003ed__11(-2);
				stackVariable1.u003cu003e4__this = this;
				return stackVariable1;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 61;
			}
		}

		public ExpressionCollection Indices
		{
			get;
			set;
		}

		public Expression Target
		{
			get;
			set;
		}

		public DynamicIndexerExpression(Expression target, TypeReference expressionType, IEnumerable<Instruction> instructions)
		{
			this(target, new ExpressionCollection(), expressionType, instructions);
			return;
		}

		private DynamicIndexerExpression(Expression target, ExpressionCollection indices, TypeReference expressionType, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_Target(target);
			this.set_Indices(indices);
			this.set_ExpressionType(expressionType);
			return;
		}

		public override Expression Clone()
		{
			return new DynamicIndexerExpression(this.get_Target().Clone(), this.get_Indices().Clone(), this.get_ExpressionType(), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new DynamicIndexerExpression(this.get_Target().CloneExpressionOnly(), this.get_Indices().CloneExpressionsOnly(), this.get_ExpressionType(), null);
		}

		public override bool Equals(Expression other)
		{
			if (other.get_CodeNodeType() != 61)
			{
				return false;
			}
			V_0 = other as DynamicIndexerExpression;
			if (!this.get_Target().Equals(V_0.get_Target()))
			{
				return false;
			}
			return this.get_Indices().Equals(V_0.get_Indices());
		}
	}
}