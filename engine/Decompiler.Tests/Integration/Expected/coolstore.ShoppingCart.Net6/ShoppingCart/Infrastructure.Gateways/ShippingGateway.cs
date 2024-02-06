using ShoppingCart.Core.Dtos;
using ShoppingCart.Core.Gateways;
using System;
using System.Runtime.CompilerServices;

namespace ShoppingCart.Infrastructure.Gateways
{
	public class ShippingGateway : IShippingGateway
	{
		public ShippingGateway()
		{
		}

		[NullableContext(1)]
		public CartDto CalculateShipping(CartDto cart)
		{
			return cart;
		}
	}
}