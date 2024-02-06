using System;
using System.Runtime.CompilerServices;

namespace ShoppingCart.Core.Dtos
{
	[Nullable(0)]
	[NullableContext(1)]
	public class InventoryDto
	{
		public string Description { get; set; } = null;

		public Guid Id
		{
			get;
			set;
		}

		public string Location { get; set; } = null;

		public string Website { get; set; } = null;

		public InventoryDto()
		{
		}
	}
}