using MediatR;
using ShoppingCart.Core.Dtos;
using System;
using System.Runtime.CompilerServices;

namespace ShoppingCart.UseCases.UpdateAmountOfProductInShoppingCart
{
	public class UpdateAmountOfProductInShoppingCartCommand : IRequest<CartDto>, IBaseRequest
	{
		public Guid ProductId
		{
			get;
			set;
		}

		public int Quantity
		{
			get;
			set;
		}

		public UpdateAmountOfProductInShoppingCartCommand()
		{
		}
	}
}