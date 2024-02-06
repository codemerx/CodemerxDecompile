using MediatR;
using ShoppingCart.Core.Dtos;
using System;
using System.Runtime.CompilerServices;

namespace ShoppingCart.UseCases.CreateShoppingCartWithProduct
{
	public class CreateShoppingCartWithProductCommand : IRequest<CartDto>, IBaseRequest
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

		public CreateShoppingCartWithProductCommand()
		{
		}
	}
}