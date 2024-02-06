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

namespace Northwind.Application.Products.Queries.GetProductsList
{
	public class GetProductsListQueryHandler : IRequestHandler<GetProductsListQuery, ProductsListVm>
	{
		private readonly INorthwindDbContext _context;

		private readonly IMapper _mapper;

		public GetProductsListQueryHandler(INorthwindDbContext context, IMapper mapper)
		{
			this._context = context;
			this._mapper = mapper;
		}

		public async Task<ProductsListVm> Handle(GetProductsListQuery request, CancellationToken cancellationToken)
		{
			IQueryable<ProductDto> productDtos = Extensions.ProjectTo<ProductDto>(this._context.Products, this._mapper.get_ConfigurationProvider(), Array.Empty<Expression<Func<ProductDto, object>>>());
			List<ProductDto> listAsync = await EntityFrameworkQueryableExtensions.ToListAsync<ProductDto>(
				from p in productDtos
				orderby p.ProductName
				select p, cancellationToken);
			List<ProductDto> productDtos1 = listAsync;
			listAsync = null;
			ProductsListVm productsListVm = new ProductsListVm()
			{
				Products = productDtos1,
				CreateEnabled = true
			};
			ProductsListVm productsListVm1 = productsListVm;
			ProductsListVm productsListVm2 = productsListVm1;
			productDtos1 = null;
			productsListVm1 = null;
			return productsListVm2;
		}
	}
}