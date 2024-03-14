using N8T.Infrastructure;
using ShoppingCart.Core.Dtos;
using ShoppingCart.Core.Exceptions;
using ShoppingCart.Core.Gateways;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingCart.Infrastructure.Extensions
{
	[Nullable(0)]
	[NullableContext(1)]
	public static class CartDtoExtensions
	{
		public static async Task<CartDto> CalculateCartAsync(CartDto cart, IProductCatalogGateway productCatalogService, IShippingGateway shippingGateway, IPromoGateway promoGateway)
		{
			CartDtoExtensions.u003cCalculateCartAsyncu003ed__1 variable = null;
			AsyncTaskMethodBuilder<CartDto> asyncTaskMethodBuilder = AsyncTaskMethodBuilder<CartDto>.Create();
			asyncTaskMethodBuilder.Start<CartDtoExtensions.u003cCalculateCartAsyncu003ed__1>(ref variable);
			return asyncTaskMethodBuilder.Task;
		}

		public static async Task<CartDto> InsertItemToCartAsync(CartDto cart, int quantity, Guid productId, IProductCatalogGateway productCatalogService)
		{
			CartItemDto cartItemDto = new CartItemDto()
			{
				Quantity = quantity,
				ProductId = productId
			};
			CartItemDto name = cartItemDto;
			ProductDto productByIdAsync = await productCatalogService.GetProductByIdAsync(productId, new CancellationToken());
			ProductDto productDto = productByIdAsync;
			productByIdAsync = null;
			if (productDto != null)
			{
				name.ProductName = productDto.Name;
				name.ProductPrice = productDto.Price;
				name.ProductImagePath = productDto.ImageUrl;
				name.ProductDescription = productDto.Description;
				name.InventoryId = N8T.Infrastructure.Extensions.ConvertTo<Guid>(productDto.InventoryId);
				name.InventoryLocation = productDto.InventoryLocation;
			}
			cart.Items.Add(name);
			CartDto cartDto = cart;
			name = null;
			productDto = null;
			return cartDto;
		}
	}
}