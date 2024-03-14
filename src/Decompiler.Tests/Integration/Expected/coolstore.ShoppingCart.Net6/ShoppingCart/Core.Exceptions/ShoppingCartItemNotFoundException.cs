using N8T.Core.Domain;
using System;
using System.Runtime.CompilerServices;

namespace ShoppingCart.Core.Exceptions
{
	public class ShoppingCartItemNotFoundException : CoreException
	{
		public ShoppingCartItemNotFoundException(Guid id)
		{
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(45, 1);
			defaultInterpolatedStringHandler.AppendLiteral("The shopping cart item with id=");
			defaultInterpolatedStringHandler.AppendFormatted<Guid>(id);
			defaultInterpolatedStringHandler.AppendLiteral(" is not found.");
			this(defaultInterpolatedStringHandler.ToStringAndClear());
		}

		[NullableContext(1)]
		public ShoppingCartItemNotFoundException(string message) : base(message)
		{
		}
	}
}