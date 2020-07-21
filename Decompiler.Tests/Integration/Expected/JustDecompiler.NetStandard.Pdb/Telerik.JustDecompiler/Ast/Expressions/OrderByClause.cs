using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast;
using Telerik.JustDecompiler.Common;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class OrderByClause : QueryClause
	{
		public override IEnumerable<ICodeNode> Children
		{
			get
			{
				stackVariable1 = this.get_ExpressionToOrderDirectionMap();
				stackVariable2 = OrderByClause.u003cu003ec.u003cu003e9__11_0;
				if (stackVariable2 == null)
				{
					dummyVar0 = stackVariable2;
					stackVariable2 = new Func<KeyValuePair<Expression, OrderDirection>, ICodeNode>(OrderByClause.u003cu003ec.u003cu003e9.u003cget_Childrenu003eb__11_0);
					OrderByClause.u003cu003ec.u003cu003e9__11_0 = stackVariable2;
				}
				return stackVariable1.Select<KeyValuePair<Expression, OrderDirection>, ICodeNode>(stackVariable2);
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return 77;
			}
		}

		public PairList<Expression, OrderDirection> ExpressionToOrderDirectionMap
		{
			get;
			set;
		}

		public OrderByClause(PairList<Expression, OrderDirection> expressionToOrderMap, IEnumerable<Instruction> instructions)
		{
			base(instructions);
			this.set_ExpressionToOrderDirectionMap(expressionToOrderMap);
			return;
		}

		public override Expression Clone()
		{
			stackVariable1 = this.get_ExpressionToOrderDirectionMap();
			stackVariable2 = OrderByClause.u003cu003ec.u003cu003e9__6_0;
			if (stackVariable2 == null)
			{
				dummyVar0 = stackVariable2;
				stackVariable2 = new Func<KeyValuePair<Expression, OrderDirection>, KeyValuePair<Expression, OrderDirection>>(OrderByClause.u003cu003ec.u003cu003e9.u003cCloneu003eb__6_0);
				OrderByClause.u003cu003ec.u003cu003e9__6_0 = stackVariable2;
			}
			return new OrderByClause(new PairList<Expression, OrderDirection>(stackVariable1.Select<KeyValuePair<Expression, OrderDirection>, KeyValuePair<Expression, OrderDirection>>(stackVariable2)), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			stackVariable1 = this.get_ExpressionToOrderDirectionMap();
			stackVariable2 = OrderByClause.u003cu003ec.u003cu003e9__7_0;
			if (stackVariable2 == null)
			{
				dummyVar0 = stackVariable2;
				stackVariable2 = new Func<KeyValuePair<Expression, OrderDirection>, KeyValuePair<Expression, OrderDirection>>(OrderByClause.u003cu003ec.u003cu003e9.u003cCloneExpressionOnlyu003eb__7_0);
				OrderByClause.u003cu003ec.u003cu003e9__7_0 = stackVariable2;
			}
			return new OrderByClause(new PairList<Expression, OrderDirection>(stackVariable1.Select<KeyValuePair<Expression, OrderDirection>, KeyValuePair<Expression, OrderDirection>>(stackVariable2)), null);
		}

		public override bool Equals(Expression other)
		{
			V_0 = other as OrderByClause;
			if (V_0 == null || this.get_ExpressionToOrderDirectionMap().get_Count() != V_0.get_ExpressionToOrderDirectionMap().get_Count())
			{
				return false;
			}
			V_1 = 0;
			while (V_1 < this.get_ExpressionToOrderDirectionMap().get_Count())
			{
				V_2 = this.get_ExpressionToOrderDirectionMap().get_Item(V_1);
				V_3 = V_0.get_ExpressionToOrderDirectionMap().get_Item(V_1);
				if (!V_2.get_Key().Equals(V_3.get_Key()) || V_2.get_Value() != V_3.get_Value())
				{
					return false;
				}
				V_1 = V_1 + 1;
			}
			return true;
		}
	}
}