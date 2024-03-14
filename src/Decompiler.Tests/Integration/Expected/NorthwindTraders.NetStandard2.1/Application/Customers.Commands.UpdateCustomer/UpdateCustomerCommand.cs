using MediatR;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Common.Exceptions;
using Northwind.Application.Common.Interfaces;
using Northwind.Domain.Entities;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Northwind.Application.Customers.Commands.UpdateCustomer
{
	public class UpdateCustomerCommand : IRequest, IRequest<Unit>, IBaseRequest
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

		public UpdateCustomerCommand()
		{
		}

		public class Handler : IRequestHandler<UpdateCustomerCommand>, IRequestHandler<UpdateCustomerCommand, Unit>
		{
			private readonly INorthwindDbContext _context;

			public Handler(INorthwindDbContext context)
			{
				this._context = context;
			}

			public async Task<Unit> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
			{
				UpdateCustomerCommand.Handler.u003cu003ec__DisplayClass2_0 variable = null;
				DbSet<Customer> customers = this._context.Customers;
				ParameterExpression parameterExpression = Expression.Parameter(typeof(Customer), "c");
				BinaryExpression binaryExpression = Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Customer).GetMethod("get_CustomerId").MethodHandle)), Expression.Property(Expression.Field(Expression.Constant(variable, typeof(UpdateCustomerCommand.Handler.u003cu003ec__DisplayClass2_0)), FieldInfo.GetFieldFromHandle(typeof(UpdateCustomerCommand.Handler.u003cu003ec__DisplayClass2_0).GetField("request").FieldHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(UpdateCustomerCommand).GetMethod("get_Id").MethodHandle)));
				ParameterExpression[] parameterExpressionArray = new ParameterExpression[] { parameterExpression };
				Customer customer = await EntityFrameworkQueryableExtensions.SingleOrDefaultAsync<Customer>(customers, Expression.Lambda<Func<Customer, bool>>(binaryExpression, parameterExpressionArray), cancellationToken);
				Customer customer1 = customer;
				customer = null;
				if (customer1 == null)
				{
					throw new NotFoundException("Customer", request.Id);
				}
				customer1.set_Address(request.Address);
				customer1.set_City(request.City);
				customer1.set_CompanyName(request.CompanyName);
				customer1.set_ContactName(request.ContactName);
				customer1.set_ContactTitle(request.ContactTitle);
				customer1.set_Country(request.Country);
				customer1.set_Fax(request.Fax);
				customer1.set_Phone(request.Phone);
				customer1.set_PostalCode(request.PostalCode);
				await this._context.SaveChangesAsync(cancellationToken);
				Unit value = Unit.Value;
				variable = null;
				customer1 = null;
				return value;
			}
		}
	}
}