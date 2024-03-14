using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Northwind.Application.Customers.Queries.GetCustomersList
{
	public class CustomersListVm
	{
		public IList<CustomerLookupDto> Customers
		{
			get;
			set;
		}

		public CustomersListVm()
		{
		}
	}
}