using Northwind.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Northwind.Persistence
{
	internal static class EmployeeExtensions
	{
		public static Employee AddEmployeeTerritories(this Employee employee, params EmployeeTerritory[] employeeTerritories)
		{
			EmployeeTerritory[] employeeTerritoryArray = employeeTerritories;
			for (int i = 0; i < (int)employeeTerritoryArray.Length; i++)
			{
				EmployeeTerritory employeeTerritory = employeeTerritoryArray[i];
				employee.get_EmployeeTerritories().Add(employeeTerritory);
			}
			return employee;
		}
	}
}