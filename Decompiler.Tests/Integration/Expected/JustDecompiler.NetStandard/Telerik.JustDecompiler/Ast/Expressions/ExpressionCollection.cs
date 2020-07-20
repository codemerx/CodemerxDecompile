using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Telerik.JustDecompiler.Ast.Expressions
{
	public class ExpressionCollection : Collection<Expression>, IEquatable<ExpressionCollection>
	{
		public ExpressionCollection()
		{
			base();
			return;
		}

		public ExpressionCollection(IEnumerable<Expression> collection)
		{
			base();
			V_0 = collection.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					this.Add(V_1);
				}
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
			return;
		}

		public ExpressionCollection Clone()
		{
			V_0 = new ExpressionCollection();
			V_1 = this.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_0.Add(V_2.Clone());
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			return V_0;
		}

		public ExpressionCollection CloneExpressionsOnly()
		{
			V_0 = new ExpressionCollection();
			V_1 = this.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_0.Add(V_2.CloneExpressionOnly());
				}
			}
			finally
			{
				if (V_1 != null)
				{
					V_1.Dispose();
				}
			}
			return V_0;
		}

		public bool Equals(ExpressionCollection other)
		{
			if (other == null)
			{
				return false;
			}
			if (this.get_Count() != other.get_Count())
			{
				return false;
			}
			V_0 = 0;
			while (V_0 < this.get_Count())
			{
				if (!this.get_Item(V_0).Equals(other.get_Item(V_0)))
				{
					return false;
				}
				V_0 = V_0 + 1;
			}
			return true;
		}
	}
}