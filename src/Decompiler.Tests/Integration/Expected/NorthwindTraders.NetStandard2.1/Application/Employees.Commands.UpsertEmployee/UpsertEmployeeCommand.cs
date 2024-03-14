using MediatR;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Common.Interfaces;
using Northwind.Domain.Entities;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Northwind.Application.Employees.Commands.UpsertEmployee
{
	public class UpsertEmployeeCommand : IRequest<int>, IBaseRequest
	{
		public string Address
		{
			get;
			set;
		}

		public DateTime? BirthDate
		{
			get;
			set;
		}

		public string City
		{
			get;
			set;
		}

		public string Country
		{
			get;
			set;
		}

		public string Extension
		{
			get;
			set;
		}

		public string FirstName
		{
			get;
			set;
		}

		public DateTime? HireDate
		{
			get;
			set;
		}

		public string HomePhone
		{
			get;
			set;
		}

		public int? Id
		{
			get;
			set;
		}

		public string LastName
		{
			get;
			set;
		}

		public int? ManagerId
		{
			get;
			set;
		}

		public string Notes
		{
			get;
			set;
		}

		public byte[] Photo
		{
			get;
			set;
		}

		public string Position
		{
			get;
			set;
		}

		public string PostalCode
		{
			get;
			set;
		}

		public string Region
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public UpsertEmployeeCommand()
		{
		}

		public class UpsertEmployeeCommandHandler : IRequestHandler<UpsertEmployeeCommand, int>
		{
			private readonly INorthwindDbContext _context;

			public UpsertEmployeeCommandHandler(INorthwindDbContext context)
			{
				this._context = context;
			}

			public async Task<int> Handle(UpsertEmployeeCommand request, CancellationToken cancellationToken)
			{
				Employee employee;
				if (!request.Id.HasValue)
				{
					employee = new Employee();
					this._context.Employees.Add(employee);
				}
				else
				{
					DbSet<Employee> employees = this._context.Employees;
					Object[] value = new Object[] { request.Id.Value };
					Employee employee1 = await employees.FindAsync(value);
					employee = employee1;
					employee1 = null;
				}
				employee.set_TitleOfCourtesy(request.Title);
				employee.set_FirstName(request.FirstName);
				employee.set_LastName(request.LastName);
				employee.set_BirthDate(request.BirthDate);
				employee.set_Address(request.Address);
				employee.set_City(request.City);
				employee.set_Region(request.Region);
				employee.set_PostalCode(request.PostalCode);
				employee.set_Country(request.Country);
				employee.set_HomePhone(request.HomePhone);
				employee.set_Title(request.Position);
				employee.set_Extension(request.Extension);
				employee.set_HireDate(request.HireDate);
				employee.set_Notes(request.Notes);
				employee.set_Photo(request.Photo);
				employee.set_ReportsTo(request.ManagerId);
				await this._context.SaveChangesAsync(cancellationToken);
				int employeeId = employee.get_EmployeeId();
				employee = null;
				return employeeId;
			}
		}
	}
}