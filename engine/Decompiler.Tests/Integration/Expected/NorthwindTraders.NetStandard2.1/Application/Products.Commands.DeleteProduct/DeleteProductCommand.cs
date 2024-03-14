using MediatR;
using System;
using System.Runtime.CompilerServices;

namespace Northwind.Application.Products.Commands.DeleteProduct
{
	public class DeleteProductCommand : IRequest, IRequest<Unit>, IBaseRequest
	{
		public int Id
		{
			get;
			set;
		}

		public DeleteProductCommand()
		{
		}
	}
}