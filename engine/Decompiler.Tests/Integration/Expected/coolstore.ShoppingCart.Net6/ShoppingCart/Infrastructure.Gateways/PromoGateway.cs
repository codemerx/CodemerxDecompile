using ShoppingCart.Core.Dtos;
using ShoppingCart.Core.Gateways;
using System;
using System.Runtime.CompilerServices;

namespace ShoppingCart.Infrastructure.Gateways
{
	public class PromoGateway : IPromoGateway
	{
		public PromoGateway()
		{
		}

		[NullableContext(1)]
		public CartDto ApplyCartItemPromotions(CartDto cart)
		{
			return cart;
		}

		[NullableContext(1)]
		public CartDto ApplyShippingPromotions(CartDto cart)
		{
			return cart;
		}
	}
}