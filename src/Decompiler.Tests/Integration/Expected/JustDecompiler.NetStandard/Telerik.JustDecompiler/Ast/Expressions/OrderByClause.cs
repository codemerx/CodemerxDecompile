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
				return 
					from pair in this.ExpressionToOrderDirectionMap
					select pair.Key;
			}
		}

		public override Telerik.JustDecompiler.Ast.CodeNodeType CodeNodeType
		{
			get
			{
				return Telerik.JustDecompiler.Ast.CodeNodeType.OrderByClause;
			}
		}

		public PairList<Expression, OrderDirection> ExpressionToOrderDirectionMap
		{
			get;
			set;
		}

		public OrderByClause(PairList<Expression, OrderDirection> expressionToOrderMap, IEnumerable<Instruction> instructions) : base(instructions)
		{
			this.ExpressionToOrderDirectionMap = expressionToOrderMap;
		}

		public override Expression Clone()
		{
			return new OrderByClause(new PairList<Expression, OrderDirection>(
				from pair in this.ExpressionToOrderDirectionMap
				select new KeyValuePair<Expression, OrderDirection>(pair.Key.Clone(), pair.Value)), this.instructions);
		}

		public override Expression CloneExpressionOnly()
		{
			return new OrderByClause(new PairList<Expression, OrderDirection>(
				from pair in this.ExpressionToOrderDirectionMap
				select new KeyValuePair<Expression, OrderDirection>(pair.Key.CloneExpressionOnly(), pair.Value)), null);
		}

		public override bool Equals(Expression other)
		{
			OrderByClause orderByClause = other as OrderByClause;
			if (orderByClause == null || this.ExpressionToOrderDirectionMap.Count != orderByClause.ExpressionToOrderDirectionMap.Count)
			{
				return false;
			}
			for (int i = 0; i < this.ExpressionToOrderDirectionMap.Count; i++)
			{
				KeyValuePair<Expression, OrderDirection> item = this.ExpressionToOrderDirectionMap[i];
				KeyValuePair<Expression, OrderDirection> keyValuePair = orderByClause.ExpressionToOrderDirectionMap[i];
				if (!item.Key.Equals(keyValuePair.Key) || item.Value != keyValuePair.Value)
				{
					return false;
				}
			}
			return true;
		}
	}
}