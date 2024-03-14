using ShoppingCart.Core.Dtos;
using System.Runtime.CompilerServices;

namespace ShoppingCart.Core.Gateways
{
	[NullableContext(1)]
	public interface IShippingGateway
	{
		CartDto CalculateShipping(CartDto cart);
	}
}