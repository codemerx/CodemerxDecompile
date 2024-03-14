using MediatR;
using System;

namespace Northwind.Application.Products.Queries.GetProductsFile
{
	public class GetProductsFileQuery : IRequest<ProductsFileVm>, IBaseRequest
	{
		public GetProductsFileQuery()
		{
		}
	}
}