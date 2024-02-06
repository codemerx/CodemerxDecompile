using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Common.Exceptions;
using Northwind.Application.Common.Interfaces;
using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Northwind.Application.Products.Queries.GetProductDetail
{
	public class GetProductDetailQueryHandler : IRequestHandler<GetProductDetailQuery, ProductDetailVm>
	{
		private readonly INorthwindDbContext _context;

		private readonly IMapper _mapper;

		public GetProductDetailQueryHandler(INorthwindDbContext context, IMapper mapper)
		{
			this._context = context;
			this._mapper = mapper;
		}

		public async Task<ProductDetailVm> Handle(GetProductDetailQuery request, CancellationToken cancellationToken)
		{
			IQueryable<ProductDetailVm> productDetailVms = Extensions.ProjectTo<ProductDetailVm>(this._context.Products, this._mapper.get_ConfigurationProvider(), Array.Empty<Expression<Func<ProductDetailVm, object>>>());
			ProductDetailVm productDetailVm = await EntityFrameworkQueryableExtensions.FirstOrDefaultAsync<ProductDetailVm>(productDetailVms, (ProductDetailVm p) => p.ProductId == request.Id, cancellationToken);
			ProductDetailVm productDetailVm1 = productDetailVm;
			productDetailVm = null;
			if (productDetailVm1 == null)
			{
				throw new NotFoundException("Product", (object)request.Id);
			}
			ProductDetailVm productDetailVm2 = productDetailVm1;
			productDetailVm1 = null;
			return productDetailVm2;
		}
	}
}