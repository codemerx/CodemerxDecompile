using System;
using System.Collections;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Steps
{
	internal class ExpressionComparer : IEqualityComparer
	{
		public ExpressionComparer()
		{
		}

		public new bool Equals(object x, object y)
		{
			if ((object)x == (object)y)
			{
				return true;
			}
			if (x == null)
			{
				return (object)y == (object)null;
			}
			Expression expression = x as Expression;
			Expression expression1 = y as Expression;
			if (expression == null || expression1 == null)
			{
				return false;
			}
			return expression.Equals(expression1);
		}

		public int GetHashCode(object obj)
		{
			return obj.GetHashCode();
		}
	}
}