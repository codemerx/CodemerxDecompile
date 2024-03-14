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

namespace Northwind.Application.Categories.Queries.GetCategoriesList
{
	public class GetCategoriesListQueryHandler : IRequestHandler<GetCategoriesListQuery, CategoriesListVm>
	{
		private readonly INorthwindDbContext _context;

		private readonly IMapper _mapper;

		public GetCategoriesListQueryHandler(INorthwindDbContext context, IMapper mapper)
		{
			this._context = context;
			this._mapper = mapper;
		}

		public async Task<CategoriesListVm> Handle(GetCategoriesListQuery request, CancellationToken cancellationToken)
		{
			List<CategoryDto> listAsync = await EntityFrameworkQueryableExtensions.ToListAsync<CategoryDto>(Extensions.ProjectTo<CategoryDto>(this._context.Categories, this._mapper.get_ConfigurationProvider(), Array.Empty<Expression<Func<CategoryDto, object>>>()), cancellationToken);
			List<CategoryDto> categoryDtos = listAsync;
			listAsync = null;
			CategoriesListVm categoriesListVm = new CategoriesListVm()
			{
				Categories = categoryDtos,
				Count = categoryDtos.Count
			};
			CategoriesListVm categoriesListVm1 = categoriesListVm;
			CategoriesListVm categoriesListVm2 = categoriesListVm1;
			categoryDtos = null;
			categoriesListVm1 = null;
			return categoriesListVm2;
		}
	}
}