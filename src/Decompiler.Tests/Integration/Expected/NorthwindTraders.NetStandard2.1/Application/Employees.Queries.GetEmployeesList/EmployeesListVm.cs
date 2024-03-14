using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Northwind.Application.Employees.Queries.GetEmployeesList
{
	public class EmployeesListVm
	{
		public IList<EmployeeLookupDto> Employees
		{
			get;
			set;
		}

		public EmployeesListVm()
		{
		}
	}
}