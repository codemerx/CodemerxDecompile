using N8T.Core.Domain;
using System;
using System.Runtime.CompilerServices;

namespace ShoppingCart.Core.Exceptions
{
	public class ProductNotFoundException : CoreException
	{
		public ProductNotFoundException(Guid id)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 1);
			defaultInterpolatedStringHandler.AppendLiteral("The product with id=");
			defaultInterpolatedStringHandler.AppendFormatted<Guid>(id);
			defaultInterpolatedStringHandler.AppendLiteral(" is not found.");
			this(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		[NullableContext(1)]
		public ProductNotFoundException(string message) : base(message)
		{
		}
	}
}