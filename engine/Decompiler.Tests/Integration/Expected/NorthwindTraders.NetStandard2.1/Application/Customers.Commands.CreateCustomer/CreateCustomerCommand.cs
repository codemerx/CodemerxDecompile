using MediatR;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Common.Interfaces;
using Northwind.Domain.Entities;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Northwind.Application.Customers.Commands.CreateCustomer
{
	public class CreateCustomerCommand : IRequest, IRequest<Unit>, IBaseRequest
	{
		public string Address
		{
			get;
			set;
		}

		public string City
		{
			get;
			set;
		}

		public string CompanyName
		{
			get;
			set;
		}

		public string ContactName
		{
			get;
			set;
		}

		public string ContactTitle
		{
			get;
			set;
		}

		public string Country
		{
			get;
			set;
		}

		public string Fax
		{
			get;
			set;
		}

		public string Id
		{
			get;
			set;
		}

		public string Phone
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

		public CreateCustomerCommand()
		{
		}

		public class Handler : IRequestHandler<CreateCustomerCommand>, IRequestHandler<CreateCustomerCommand, Unit>
		{
			private readonly INorthwindDbContext _context;

			private readonly IMediator _mediator;

			public Handler(INorthwindDbContext context, IMediator mediator)
			{
				this._context = context;
				this._mediator = mediator;
			}

			public async Task<Unit> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
			{
				Customer customer = new Customer();
				customer.set_CustomerId(request.Id);
				customer.set_Address(request.Address);
				customer.set_City(request.City);
				customer.set_CompanyName(request.CompanyName);
				customer.set_ContactName(request.ContactName);
				customer.set_ContactTitle(request.ContactTitle);
				customer.set_Country(request.Country);
				customer.set_Fax(request.Fax);
				customer.set_Phone(request.Phone);
				customer.set_PostalCode(request.PostalCode);
				Customer customer1 = customer;
				this._context.Customers.Add(customer1);
				await this._context.SaveChangesAsync(cancellationToken);
				IMediator mediator = this._mediator;
				CustomerCreated customerCreated = new CustomerCreated()
				{
					CustomerId = customer1.get_CustomerId()
				};
				await mediator.Publish<CustomerCreated>(customerCreated, cancellationToken);
				Unit value = Unit.Value;
				customer1 = null;
				return value;
			}
		}
	}
}