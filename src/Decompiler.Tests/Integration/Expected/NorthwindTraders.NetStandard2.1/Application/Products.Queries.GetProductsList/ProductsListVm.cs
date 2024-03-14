using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Northwind.Application.Products.Queries.GetProductsList
{
	public class ProductsListVm
	{
		public bool CreateEnabled
		{
			get;
			set;
		}

		public IList<ProductDto> Products
		{
			get;
			set;
		}

		public ProductsListVm()
		{
		}
	}
}