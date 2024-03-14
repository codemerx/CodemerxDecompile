using MediatR;
using System;
using System.Runtime.CompilerServices;

namespace Northwind.Application.Customers.Queries.GetCustomerDetail
{
	public class GetCustomerDetailQuery : IRequest<CustomerDetailVm>, IBaseRequest
	{
		public string Id
		{
			get;
			set;
		}

		public GetCustomerDetailQuery()
		{
		}
	}
}