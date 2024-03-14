using MediatR;
using System;

namespace Northwind.Application.Products.Queries.GetProductsList
{
	public class GetProductsListQuery : IRequest<ProductsListVm>, IBaseRequest
	{
		public GetProductsListQuery()
		{
		}
	}
}