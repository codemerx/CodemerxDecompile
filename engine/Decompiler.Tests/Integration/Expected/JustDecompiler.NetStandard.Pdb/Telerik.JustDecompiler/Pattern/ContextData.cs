using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Telerik.JustDecompiler.Pattern
{
	public class ContextData : CodePattern
	{
		public IEqualityComparer Comparer
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public ContextData()
		{
		}

		public override bool Match(MatchContext context, object @object)
		{
			object obj;
			if (!context.TryGetData(this.Name, out obj))
			{
				return false;
			}
			object comparer = this.Comparer;
			if (comparer == null)
			{
				comparer = EqualityComparer<object>.Default;
			}
			return comparer.Equals(obj, @object);
		}
	}
}