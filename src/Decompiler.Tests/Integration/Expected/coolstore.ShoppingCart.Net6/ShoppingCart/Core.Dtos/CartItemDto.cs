using System;
using System.Runtime.CompilerServices;

namespace ShoppingCart.Core.Dtos
{
	[Nullable(0)]
	[NullableContext(1)]
	public class CartItemDto
	{
		public Guid InventoryId
		{
			get;
			set;
		}

		public string InventoryLocation { get; set; } = null;

		public double Price
		{
			get;
			set;
		}

		public string ProductDescription { get; set; } = null;

		public Guid ProductId
		{
			get;
			set;
		}

		public string ProductImagePath { get; set; } = null;

		public string ProductName { get; set; } = null;

		public double ProductPrice
		{
			get;
			set;
		}

		public double PromoSavings
		{
			get;
			set;
		}

		public int Quantity
		{
			get;
			set;
		}

		public CartItemDto()
		{
		}
	}
}