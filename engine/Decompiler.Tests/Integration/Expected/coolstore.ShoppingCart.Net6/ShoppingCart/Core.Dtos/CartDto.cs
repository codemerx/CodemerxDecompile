using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ShoppingCart.Core.Dtos
{
	[Nullable(0)]
	[NullableContext(1)]
	public class CartDto
	{
		public double CartItemPromoSavings
		{
			get;
			set;
		}

		public double CartItemTotal
		{
			get;
			set;
		}

		public double CartTotal
		{
			get;
			set;
		}

		public Guid Id
		{
			get;
			set;
		}

		public bool IsCheckOut
		{
			get;
			set;
		}

		public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();

		public double ShippingPromoSavings
		{
			get;
			set;
		}

		public double ShippingTotal
		{
			get;
			set;
		}

		public string UserId { get; set; } = null;

		public CartDto()
		{
		}
	}
}