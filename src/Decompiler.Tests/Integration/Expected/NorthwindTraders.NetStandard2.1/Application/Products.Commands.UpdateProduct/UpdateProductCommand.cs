using MediatR;
using System;
using System.Runtime.CompilerServices;

namespace Northwind.Application.Products.Commands.UpdateProduct
{
	public class UpdateProductCommand : IRequest, IRequest<Unit>, IBaseRequest
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

		public int ProductId
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

		public UpdateProductCommand()
		{
		}
	}
}