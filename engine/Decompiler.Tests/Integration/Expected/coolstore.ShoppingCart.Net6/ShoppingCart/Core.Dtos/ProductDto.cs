using System;
using System.Runtime.CompilerServices;

namespace ShoppingCart.Core.Dtos
{
	[Nullable(0)]
	[NullableContext(1)]
	public class ProductDto
	{
		public string CategoryId { get; set; } = null;

		public string CategoryName { get; set; } = null;

		public string Description { get; set; } = null;

		public Guid Id
		{
			get;
			set;
		}

		public string ImageUrl { get; set; } = null;

		public string InventoryId { get; set; } = null;

		public string InventoryLocation { get; set; } = null;

		public string Name { get; set; } = null;

		public double Price
		{
			get;
			set;
		}

		public ProductDto()
		{
		}
	}
}