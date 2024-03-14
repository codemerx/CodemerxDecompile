using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Telerik.JustDecompiler.Pattern
{
	public class Constant : CodePattern
	{
		public IEqualityComparer Comparer
		{
			get;
			set;
		}

		public object Value
		{
			get;
			set;
		}

		public Constant()
		{
		}

		public override bool Match(MatchContext context, object @object)
		{
			object comparer = this.Comparer;
			if (comparer == null)
			{
				comparer = EqualityComparer<object>.Default;
			}
			return comparer.Equals(this.Value, @object);
		}
	}
}