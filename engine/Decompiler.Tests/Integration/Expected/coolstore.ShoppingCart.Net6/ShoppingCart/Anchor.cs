using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ShoppingCart
{
	[Nullable(0)]
	[NullableContext(1)]
	public class Anchor : IEquatable<Anchor>
	{
		[CompilerGenerated]
		protected virtual Type EqualityContract
		{
			get
			{
				return typeof(Anchor);
			}
		}

		public Anchor()
		{
		}
	}
}