using N8T.Core.Domain;
using ShoppingCart.Core.Dtos;
using System;
using System.Runtime.CompilerServices;

namespace ShoppingCart.Core.Events
{
	[Nullable(0)]
	[NullableContext(1)]
	public class ShoppingCartCheckedOut : EventBase
	{
		public CartDto Cart { get; set; } = null;

		public ShoppingCartCheckedOut()
		{
		}

		public override void Flatten()
		{
		}
	}
}