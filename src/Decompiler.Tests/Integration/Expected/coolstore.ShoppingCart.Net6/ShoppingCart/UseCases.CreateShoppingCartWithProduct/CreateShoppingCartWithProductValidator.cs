using FluentValidation;
using System;
using System.Runtime.CompilerServices;

namespace ShoppingCart.UseCases.CreateShoppingCartWithProduct
{
	[Nullable(new byte[] { 0, 1 })]
	public class CreateShoppingCartWithProductValidator : AbstractValidator<CreateShoppingCartWithProductCommand>
	{
		public CreateShoppingCartWithProductValidator()
		{
		}
	}
}