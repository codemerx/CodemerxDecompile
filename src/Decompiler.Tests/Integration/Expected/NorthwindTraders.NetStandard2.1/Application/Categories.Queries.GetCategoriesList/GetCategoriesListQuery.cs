using MediatR;
using System;

namespace Northwind.Application.Categories.Queries.GetCategoriesList
{
	public class GetCategoriesListQuery : IRequest<CategoriesListVm>, IBaseRequest
	{
		public GetCategoriesListQuery()
		{
		}
	}
}