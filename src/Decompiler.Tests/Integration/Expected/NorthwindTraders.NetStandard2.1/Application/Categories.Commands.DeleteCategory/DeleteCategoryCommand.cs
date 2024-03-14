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

namespace Northwind.Application.Categories.Commands.DeleteCategory
{
	public class DeleteCategoryCommand : IRequest, IRequest<Unit>, IBaseRequest
	{
		public int Id
		{
			get;
			set;
		}

		public DeleteCategoryCommand()
		{
		}

		public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>, IRequestHandler<DeleteCategoryCommand, Unit>
		{
			private readonly INorthwindDbContext _context;

			public DeleteCategoryCommandHandler(INorthwindDbContext context)
			{
				this._context = context;
			}

			public async Task<Unit> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
			{
				DbSet<Category> categories = this._context.Categories;
				Object[] id = new Object[] { request.Id };
				Category category = await categories.FindAsync(id);
				Category category1 = category;
				category = null;
				if (category1 == null)
				{
					throw new NotFoundException("Category", (object)request.Id);
				}
				this._context.Categories.Remove(category1);
				await this._context.SaveChangesAsync(cancellationToken);
				Unit value = Unit.Value;
				category1 = null;
				return value;
			}
		}
	}
}