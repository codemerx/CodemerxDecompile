using FluentValidation;
using System;
using System.Runtime.CompilerServices;

namespace ShoppingCart.UseCases.GetCartByUserId
{
	[Nullable(new byte[] { 0, 1 })]
	public class GetShoppingCartWithProductsValidator : AbstractValidator<GetCartByUserIdQuery>
	{
		public GetShoppingCartWithProductsValidator()
		{
		}
	}
}