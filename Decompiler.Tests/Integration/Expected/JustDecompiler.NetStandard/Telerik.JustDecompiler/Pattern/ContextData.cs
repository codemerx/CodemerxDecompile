using System;
using System.Collections;
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
			base();
			return;
		}

		public override bool Match(MatchContext context, object object)
		{
			if (!context.TryGetData(this.get_Name(), out V_0))
			{
				return false;
			}
			stackVariable6 = this.get_Comparer();
			if (stackVariable6 == null)
			{
				dummyVar0 = stackVariable6;
				stackVariable6 = EqualityComparer<object>.get_Default();
			}
			return stackVariable6.Equals(V_0, object);
		}
	}
}