using MediatR;
using Microsoft.EntityFrameworkCore;
using Northwind.Application.Common.Interfaces;
using Northwind.Domain.Entities;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Northwind.Application.Categories.Commands.UpsertCategory
{
	public class UpsertCategoryCommand : IRequest<int>, IBaseRequest
	{
		public string Description
		{
			get;
			set;
		}

		public int? Id
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public byte[] Picture
		{
			get;
			set;
		}

		public UpsertCategoryCommand()
		{
		}

		public class UpsertCategoryCommandHandler : IRequestHandler<UpsertCategoryCommand, int>
		{
			private readonly INorthwindDbContext _context;

			public UpsertCategoryCommandHandler(INorthwindDbContext context)
			{
				this._context = context;
			}

			public async Task<int> Handle(UpsertCategoryCommand request, CancellationToken cancellationToken)
			{
				Category category;
				if (!request.Id.HasValue)
				{
					category = new Category();
					this._context.Categories.Add(category);
				}
				else
				{
					DbSet<Category> categories = this._context.Categories;
					Object[] value = new Object[] { request.Id.Value };
					Category category1 = await categories.FindAsync(value);
					category = category1;
					category1 = null;
				}
				category.set_CategoryName(request.Name);
				category.set_Description(request.Description);
				category.set_Picture(request.Picture);
				await this._context.SaveChangesAsync(cancellationToken);
				int categoryId = category.get_CategoryId();
				category = null;
				return categoryId;
			}
		}
	}
}