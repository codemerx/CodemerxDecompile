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

namespace Northwind.Application.Products.Commands.DeleteProduct
{
	public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>, IRequestHandler<DeleteProductCommand, Unit>
	{
		private readonly INorthwindDbContext _context;

		public DeleteProductCommandHandler(INorthwindDbContext context)
		{
			this._context = context;
		}

		public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
		{
			DeleteProductCommandHandler.u003cu003ec__DisplayClass2_0 variable = null;
			DbSet<Product> products = this._context.Products;
			Object[] id = new Object[] { request.Id };
			Product product = await products.FindAsync(id);
			Product product1 = product;
			product = null;
			if (product1 == null)
			{
				throw new NotFoundException("Product", (object)request.Id);
			}
			DbSet<OrderDetail> orderDetails = this._context.OrderDetails;
			ParameterExpression parameterExpression = Expression.Parameter(typeof(OrderDetail), "od");
			BinaryExpression binaryExpression = Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(OrderDetail).GetMethod("get_ProductId").MethodHandle)), Expression.Property(Expression.Field(Expression.Constant(variable, typeof(DeleteProductCommandHandler.u003cu003ec__DisplayClass2_0)), FieldInfo.GetFieldFromHandle(typeof(DeleteProductCommandHandler.u003cu003ec__DisplayClass2_0).GetField("entity").FieldHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(Product).GetMethod("get_ProductId").MethodHandle)));
			ParameterExpression[] parameterExpressionArray = new ParameterExpression[] { parameterExpression };
			bool flag = orderDetails.Any<OrderDetail>(Expression.Lambda<Func<OrderDetail, bool>>(binaryExpression, parameterExpressionArray));
			if (flag)
			{
				throw new DeleteFailureException("Product", (object)request.Id, "There are existing orders associated with this product.");
			}
			this._context.Products.Remove(product1);
			await this._context.SaveChangesAsync(cancellationToken);
			Unit value = Unit.Value;
			variable = null;
			return value;
		}
	}
}