using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Northwind.Application.Customers.Queries.GetCustomersList
{
	public class GetCustomersListQueryHandler : IRequestHandler<GetCustomersListQuery, CustomersListVm>
	{
		private readonly INorthwindDbContext _context;

		private readonly IMapper _mapper;

		public GetCustomersListQueryHandler(INorthwindDbContext context, IMapper mapper)
		{
			this._context = context;
			this._mapper = mapper;
		}

		public async Task<CustomersListVm> Handle(GetCustomersListQuery request, CancellationToken cancellationToken)
		{
			List<CustomerLookupDto> listAsync = await EntityFrameworkQueryableExtensions.ToListAsync<CustomerLookupDto>(Extensions.ProjectTo<CustomerLookupDto>(this._context.Customers, this._mapper.get_ConfigurationProvider(), Array.Empty<Expression<Func<CustomerLookupDto, object>>>()), cancellationToken);
			List<CustomerLookupDto> customerLookupDtos = listAsync;
			listAsync = null;
			CustomersListVm customersListVm = new CustomersListVm()
			{
				Customers = customerLookupDtos
			};
			CustomersListVm customersListVm1 = customersListVm;
			customerLookupDtos = null;
			customersListVm = null;
			return customersListVm1;
		}
	}
}