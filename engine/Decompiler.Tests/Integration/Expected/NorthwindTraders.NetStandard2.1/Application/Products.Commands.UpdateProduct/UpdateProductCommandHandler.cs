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

namespace Northwind.Application.Products.Commands.UpdateProduct
{
	public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand>, IRequestHandler<UpdateProductCommand, Unit>
	{
		private readonly INorthwindDbContext _context;

		public UpdateProductCommandHandler(INorthwindDbContext context)
		{
			this._context = context;
		}

		public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
		{
			DbSet<Product> products = this._context.Products;
			Object[] productId = new Object[] { request.ProductId };
			Product product = await products.FindAsync(productId);
			Product product1 = product;
			product = null;
			if (product1 == null)
			{
				throw new NotFoundException("Product", (object)request.ProductId);
			}
			product1.set_ProductId(request.ProductId);
			product1.set_ProductName(request.ProductName);
			product1.set_CategoryId(request.CategoryId);
			product1.set_SupplierId(request.SupplierId);
			product1.set_UnitPrice(request.UnitPrice);
			product1.set_Discontinued(request.Discontinued);
			await this._context.SaveChangesAsync(cancellationToken);
			Unit value = Unit.Value;
			product1 = null;
			return value;
		}
	}
}