using FluentValidation;
using System;
using System.Runtime.CompilerServices;

namespace ShoppingCart.UseCases.Checkout
{
	[Nullable(new byte[] { 0, 1 })]
	public class CheckOutValidator : AbstractValidator<CheckOutCommand>
	{
		public CheckOutValidator()
		{
		}
	}
}