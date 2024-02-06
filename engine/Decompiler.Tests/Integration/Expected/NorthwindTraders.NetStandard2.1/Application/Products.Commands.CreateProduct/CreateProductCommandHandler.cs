using MediatR;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Common.Interfaces;
using Northwind.Domain.Entities;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Northwind.Application.Products.Commands.CreateProduct
{
	public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
	{
		private readonly INorthwindDbContext _context;

		public CreateProductCommandHandler(INorthwindDbContext context)
		{
			this._context = context;
		}

		public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
		{
			Product product = new Product();
			product.set_ProductName(request.ProductName);
			product.set_CategoryId(request.CategoryId);
			product.set_SupplierId(request.SupplierId);
			product.set_UnitPrice(request.UnitPrice);
			product.set_Discontinued(request.Discontinued);
			Product product1 = product;
			this._context.Products.Add(product1);
			await this._context.SaveChangesAsync(cancellationToken);
			int productId = product1.get_ProductId();
			product1 = null;
			return productId;
		}
	}
}