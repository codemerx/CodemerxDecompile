using MediatR;
using System;
using System.Runtime.CompilerServices;

namespace Northwind.Application.Customers.Commands.DeleteCustomer
{
	public class DeleteCustomerCommand : IRequest, IRequest<Unit>, IBaseRequest
	{
		public string Id
		{
			get;
			set;
		}

		public DeleteCustomerCommand()
		{
		}
	}
}