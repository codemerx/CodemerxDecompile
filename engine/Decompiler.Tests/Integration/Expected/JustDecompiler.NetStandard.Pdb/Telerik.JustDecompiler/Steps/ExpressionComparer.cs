using System;
using System.Collections;
using Telerik.JustDecompiler.Ast.Expressions;

namespace Telerik.JustDecompiler.Steps
{
	internal class ExpressionComparer : IEqualityComparer
	{
		public ExpressionComparer()
		{
			base();
			return;
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
			V_0 = x as Expression;
			V_1 = y as Expression;
			if (V_0 == null || V_1 == null)
			{
				return false;
			}
			return V_0.Equals(V_1);
		}

		public int GetHashCode(object obj)
		{
			return obj.GetHashCode();
		}
	}
}