using MediatR;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Common.Exceptions;
using Northwind.Application.Common.Interfaces;
using Northwind.Domain.Entities;
using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Northwind.Application.Customers.Commands.DeleteCustomer
{
	public class DeleteCustomerCommandHandler : IRequestHandler<DeleteCustomerCommand>, IRequestHandler<DeleteCustomerCommand, Unit>
	{
		private readonly INorthwindDbContext _context;

		public DeleteCustomerCommandHandler(INorthwindDbContext context)
		{
			this._context = context;
		}

		public async Task<Unit> Handle(DeleteCustomerCommand request, CancellationToken cancellationToken)
		{
			DeleteCustomerCommandHandler.u003cu003ec__DisplayClass2_0 variable = null;
			DbSet<Customer> customers = this._context.Customers;
			Object[] id = new Object[] { request.Id };
			Customer customer = await customers.FindAsync(id);
			Customer customer1 = customer;
			customer = null;
			if (customer1 == null)
			{
				throw new NotFoundException("Customer", request.Id);
			}
			DbSet<Order> orders = this._context.Orders;
			ParameterExpression parameterExpression = Expression.Parameter(typeof(Order), "o");
			BinaryExpression binaryExpression = Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Order).GetMethod("get_CustomerId").MethodHandle)), Expression.Property(Expression.Field(Expression.Constant(variable, typeof(DeleteCustomerCommandHandler.u003cu003ec__DisplayClass2_0)), FieldInfo.GetFieldFromHandle(typeof(DeleteCustomerCommandHandler.u003cu003ec__DisplayClass2_0).GetField("entity").FieldHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Customer).GetMethod("get_CustomerId").MethodHandle)));
			ParameterExpression[] parameterExpressionArray = new ParameterExpression[] { parameterExpression };
			bool flag = orders.Any<Order>(Expression.Lambda<Func<Order, bool>>(binaryExpression, parameterExpressionArray));
			if (flag)
			{
				throw new DeleteFailureException("Customer", request.Id, "There are existing orders associated with this customer.");
			}
			this._context.Customers.Remove(customer1);
			await this._context.SaveChangesAsync(cancellationToken);
			Unit value = Unit.Value;
			variable = null;
			return value;
		}
	}
}