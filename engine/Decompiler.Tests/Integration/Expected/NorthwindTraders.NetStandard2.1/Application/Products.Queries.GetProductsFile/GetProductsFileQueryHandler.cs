using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Common.Interfaces;
using Northwind.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Northwind.Application.Products.Queries.GetProductsFile
{
	public class GetProductsFileQueryHandler : IRequestHandler<GetProductsFileQuery, ProductsFileVm>
	{
		private readonly INorthwindDbContext _context;

		private readonly ICsvFileBuilder _fileBuilder;

		private readonly IMapper _mapper;

		private readonly IDateTime _dateTime;

		public GetProductsFileQueryHandler(INorthwindDbContext context, ICsvFileBuilder fileBuilder, IMapper mapper, IDateTime dateTime)
		{
			this._context = context;
			this._fileBuilder = fileBuilder;
			this._mapper = mapper;
			this._dateTime = dateTime;
		}

		public async Task<ProductsFileVm> Handle(GetProductsFileQuery request, CancellationToken cancellationToken)
		{
			List<ProductRecordDto> listAsync = await EntityFrameworkQueryableExtensions.ToListAsync<ProductRecordDto>(Extensions.ProjectTo<ProductRecordDto>(this._context.Products, this._mapper.get_ConfigurationProvider(), Array.Empty<Expression<Func<ProductRecordDto, object>>>()), cancellationToken);
			List<ProductRecordDto> productRecordDtos = listAsync;
			listAsync = null;
			byte[] numArray = this._fileBuilder.BuildProductsFile(productRecordDtos);
			ProductsFileVm productsFileVm = new ProductsFileVm()
			{
				Content = numArray,
				ContentType = "text/csv",
				FileName = String.Format("{0:yyyy-MM-dd}-Products.csv", this._dateTime.get_Now())
			};
			ProductsFileVm productsFileVm1 = productsFileVm;
			ProductsFileVm productsFileVm2 = productsFileVm1;
			productRecordDtos = null;
			numArray = null;
			productsFileVm1 = null;
			return productsFileVm2;
		}
	}
}