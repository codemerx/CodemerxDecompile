using MediatR;
using System;
using System.Runtime.CompilerServices;

namespace Northwind.Application.Products.Queries.GetProductDetail
{
	public class GetProductDetailQuery : IRequest<ProductDetailVm>, IBaseRequest
	{
		public int Id
		{
			get;
			set;
		}

		public GetProductDetailQuery()
		{
		}
	}
}