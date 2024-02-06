using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ShoppingCart.Core.Dtos
{
	[Nullable(0)]
	[NullableContext(1)]
	public class ProductByIdsRequest
	{
		public List<Guid> ProductIds { get; set; } = new List<Guid>();

		public ProductByIdsRequest()
		{
		}
	}
}