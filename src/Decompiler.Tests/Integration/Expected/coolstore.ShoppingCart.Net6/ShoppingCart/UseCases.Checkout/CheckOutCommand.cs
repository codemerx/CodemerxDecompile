using MediatR;
using ShoppingCart.Core.Dtos;
using System;

namespace ShoppingCart.UseCases.Checkout
{
	public class CheckOutCommand : IRequest<CartDto>, IBaseRequest
	{
		public CheckOutCommand()
		{
		}
	}
}