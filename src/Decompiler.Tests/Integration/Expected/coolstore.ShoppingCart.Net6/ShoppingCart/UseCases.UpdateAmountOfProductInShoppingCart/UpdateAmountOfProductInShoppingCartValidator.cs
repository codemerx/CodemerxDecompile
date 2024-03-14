using FluentValidation;
using System;
using System.Runtime.CompilerServices;

namespace ShoppingCart.UseCases.UpdateAmountOfProductInShoppingCart
{
	[Nullable(new byte[] { 0, 1 })]
	public class UpdateAmountOfProductInShoppingCartValidator : AbstractValidator<UpdateAmountOfProductInShoppingCartCommand>
	{
		public UpdateAmountOfProductInShoppingCartValidator()
		{
		}
	}
}