using System;
using System.Runtime.CompilerServices;

namespace Northwind.Application.Products.Queries.GetProductsFile
{
	public class ProductsFileVm
	{
		public byte[] Content
		{
			get;
			set;
		}

		public string ContentType
		{
			get;
			set;
		}

		public string FileName
		{
			get;
			set;
		}

		public ProductsFileVm()
		{
		}
	}
}