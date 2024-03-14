using MediatR;
using ShoppingCart.Core.Dtos;
using System;

namespace ShoppingCart.UseCases.GetCartByUserId
{
	public class GetCartByUserIdQuery : IRequest<CartDto>, IBaseRequest
	{
		public GetCartByUserIdQuery()
		{
		}
	}
}