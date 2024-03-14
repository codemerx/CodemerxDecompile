using System;
using System.Runtime.CompilerServices;

namespace ShoppingCart.Core.Dtos
{
	[Nullable(0)]
	[NullableContext(1)]
	public class CategoryDto
	{
		public Guid Id
		{
			get;
			set;
		}

		public string Name { get; set; } = null;

		public CategoryDto()
		{
		}
	}
}