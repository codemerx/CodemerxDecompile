using MediatR;
using System;
using System.Runtime.CompilerServices;

namespace Northwind.Application.Products.Commands.CreateProduct
{
	public class CreateProductCommand : IRequest<int>, IBaseRequest
	{
		public int? CategoryId
		{
			get;
			set;
		}

		public bool Discontinued
		{
			get;
			set;
		}

		public string ProductName
		{
			get;
			set;
		}

		public int? SupplierId
		{
			get;
			set;
		}

		public Decimal? UnitPrice
		{
			get;
			set;
		}

		public CreateProductCommand()
		{
		}
	}
}