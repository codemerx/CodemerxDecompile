using System;
using System.Runtime.CompilerServices;

namespace ShoppingCart.Core.Dtos
{
	public class ProductByIdRequest
	{
		public Guid Id
		{
			get;
			set;
		}

		public ProductByIdRequest()
		{
		}
	}
}