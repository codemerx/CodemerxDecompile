using MediatR;
using System;

namespace Northwind.Application.Customers.Queries.GetCustomersList
{
	public class GetCustomersListQuery : IRequest<CustomersListVm>, IBaseRequest
	{
		public GetCustomersListQuery()
		{
		}
	}
}