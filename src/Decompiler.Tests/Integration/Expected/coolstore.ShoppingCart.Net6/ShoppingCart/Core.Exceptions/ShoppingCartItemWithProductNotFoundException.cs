using N8T.Core.Domain;
using System;
using System.Runtime.CompilerServices;

namespace ShoppingCart.Core.Exceptions
{
	public class ShoppingCartItemWithProductNotFoundException : CoreException
	{
		public ShoppingCartItemWithProductNotFoundException(Guid productId)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(53, 1);
			defaultInterpolatedStringHandler.AppendLiteral("The shopping cart item with product id=");
			defaultInterpolatedStringHandler.AppendFormatted<Guid>(productId);
			defaultInterpolatedStringHandler.AppendLiteral(" is not found.");
			this(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		[NullableContext(1)]
		public ShoppingCartItemWithProductNotFoundException(string message) : base(message)
		{
		}
	}
}