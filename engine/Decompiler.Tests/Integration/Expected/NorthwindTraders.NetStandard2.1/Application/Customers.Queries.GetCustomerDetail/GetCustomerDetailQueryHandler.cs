using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Common.Exceptions;
using Northwind.Application.Common.Interfaces;
using Northwind.Domain.Entities;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Northwind.Application.Customers.Queries.GetCustomerDetail
{
	public class GetCustomerDetailQueryHandler : IRequestHandler<GetCustomerDetailQuery, CustomerDetailVm>
	{
		private readonly INorthwindDbContext _context;

		private readonly IMapper _mapper;

		public GetCustomerDetailQueryHandler(INorthwindDbContext context, IMapper mapper)
		{
			this._context = context;
			this._mapper = mapper;
		}

		public async Task<CustomerDetailVm> Handle(GetCustomerDetailQuery request, CancellationToken cancellationToken)
		{
			DbSet<Customer> customers = this._context.Customers;
			Object[] id = new Object[] { request.Id };
			Customer customer = await customers.FindAsync(id);
			Customer customer1 = customer;
			customer = null;
			if (customer1 == null)
			{
				throw new NotFoundException("Customer", request.Id);
			}
			CustomerDetailVm customerDetailVm = this._mapper.Map<CustomerDetailVm>(customer1);
			customer1 = null;
			return customerDetailVm;
		}
	}
}