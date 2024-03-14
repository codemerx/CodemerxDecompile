using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Northwind.Application.Employees.Queries.GetEmployeesList
{
	public class GetEmployeesListQuery : IRequest<EmployeesListVm>, IBaseRequest
	{
		public GetEmployeesListQuery()
		{
		}

		public class GetEmployeesListQueryHandler : IRequestHandler<GetEmployeesListQuery, EmployeesListVm>
		{
			private readonly INorthwindDbContext _context;

			private readonly IMapper _mapper;

			public GetEmployeesListQueryHandler(INorthwindDbContext context, IMapper mapper)
			{
				this._context = context;
				this._mapper = mapper;
			}

			public async Task<EmployeesListVm> Handle(GetEmployeesListQuery request, CancellationToken cancellationToken)
			{
				IQueryable<EmployeeLookupDto> employeeLookupDtos = Extensions.ProjectTo<EmployeeLookupDto>(this._context.Employees, this._mapper.get_ConfigurationProvider(), Array.Empty<Expression<Func<EmployeeLookupDto, object>>>());
				List<EmployeeLookupDto> listAsync = await EntityFrameworkQueryableExtensions.ToListAsync<EmployeeLookupDto>(
					from e in employeeLookupDtos
					orderby e.Name
					select e, cancellationToken);
				List<EmployeeLookupDto> employeeLookupDtos1 = listAsync;
				listAsync = null;
				EmployeesListVm employeesListVm = new EmployeesListVm()
				{
					Employees = employeeLookupDtos1
				};
				EmployeesListVm employeesListVm1 = employeesListVm;
				employeeLookupDtos1 = null;
				employeesListVm = null;
				return employeesListVm1;
			}
		}
	}
}