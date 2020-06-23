using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class ExpressionCollection : Collection<Expression>, IEquatable<ExpressionCollection>
	{
		public ExpressionCollection()
		{
		}

		public ExpressionCollection(IEnumerable<Expression> collection)
		{
			foreach (Expression expression in collection)
			{
				base.Add(expression);
			}
		}

		public ExpressionCollection Clone()
		{
			ExpressionCollection expressionCollection = new ExpressionCollection();
			foreach (Expression expression in this)
			{
				expressionCollection.Add(expression.Clone());
			}
			return expressionCollection;
		}

		public ExpressionCollection CloneExpressionsOnly()
		{
			ExpressionCollection expressionCollection = new ExpressionCollection();
			foreach (Expression expression in this)
			{
				expressionCollection.Add(expression.CloneExpressionOnly());
			}
			return expressionCollection;
		}

		public bool Equals(ExpressionCollection other)
		{
			if (other == null)
			{
				return false;
			}
			if (base.Count != other.Count)
			{
				return false;
			}
			for (int i = 0; i < base.Count; i++)
			{
				if (!base[i].Equals(other[i]))
				{
					return false;
				}
			}
			return true;
		}
	}
}