using System;
using System.Collections;
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
			base();
			return;
		}

		public override bool Match(MatchContext context, object object)
		{
			stackVariable1 = this.get_Comparer();
			if (stackVariable1 == null)
			{
				dummyVar0 = stackVariable1;
				stackVariable1 = EqualityComparer<object>.get_Default();
			}
			return stackVariable1.Equals(this.get_Value(), object);
		}
	}
}