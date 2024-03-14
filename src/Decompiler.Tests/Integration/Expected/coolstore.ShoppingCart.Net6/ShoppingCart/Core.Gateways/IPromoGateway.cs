using ShoppingCart.Core.Dtos;
using System.Runtime.CompilerServices;

namespace ShoppingCart.Core.Gateways
{
	[NullableContext(1)]
	public interface IPromoGateway
	{
		CartDto ApplyCartItemPromotions(CartDto cart);

		CartDto ApplyShippingPromotions(CartDto cart);
	}
}