using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Northwind.Application.Categories.Queries.GetCategoriesList
{
	public class CategoriesListVm
	{
		public IList<CategoryDto> Categories
		{
			get;
			set;
		}

		public int Count
		{
			get;
			set;
		}

		public CategoriesListVm()
		{
		}
	}
}