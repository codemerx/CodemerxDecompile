using Dapr;
using Dapr.Client;
using MediatR;
using N8T.Core.Domain;
using N8T.Infrastructure.Auth;
using ShoppingCart.Core.Dtos;
using ShoppingCart.Core.Gateways;
using ShoppingCart.Infrastructure.Extensions;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingCart.UseCases.UpdateAmountOfProductInShoppingCart
{
	[Nullable(0)]
	[NullableContext(1)]
	public class UpdateAmountOfProductInShoppingCartHandler : IRequestHandler<UpdateAmountOfProductInShoppingCartCommand, CartDto>
	{
		private readonly DaprClient _daprClient;

		private readonly IProductCatalogGateway _productCatalogGateway;

		private readonly IPromoGateway _promoGateway;

		private readonly IShippingGateway _shippingGateway;

		private readonly ISecurityContextAccessor _securityContextAccessor;

		public UpdateAmountOfProductInShoppingCartHandler(DaprClient daprClient, IProductCatalogGateway productCatalogGateway, IPromoGateway promoGateway, IShippingGateway shippingGateway, ISecurityContextAccessor securityContextAccessor)
		{
			DaprClient daprClient1 = daprClient;
			if (daprClient1 == null)
			{
				throw new ArgumentNullException("daprClient");
			}
			this._daprClient = daprClient1;
			IProductCatalogGateway productCatalogGateway1 = productCatalogGateway;
			if (productCatalogGateway1 == null)
			{
				throw new ArgumentNullException("productCatalogGateway");
			}
			this._productCatalogGateway = productCatalogGateway1;
			IPromoGateway promoGateway1 = promoGateway;
			if (promoGateway1 == null)
			{
				throw new ArgumentNullException("promoGateway");
			}
			this._promoGateway = promoGateway1;
			IShippingGateway shippingGateway1 = shippingGateway;
			if (shippingGateway1 == null)
			{
				throw new ArgumentNullException("shippingGateway");
			}
			this._shippingGateway = shippingGateway1;
			ISecurityContextAccessor securityContextAccessor1 = securityContextAccessor;
			if (securityContextAccessor1 == null)
			{
				throw new ArgumentNullException("securityContextAccessor");
			}
			this._securityContextAccessor = securityContextAccessor1;
		}

		public async Task<CartDto> Handle(UpdateAmountOfProductInShoppingCartCommand request, CancellationToken cancellationToken)
		{
			string userId = this._securityContextAccessor.get_UserId();
			DaprClient daprClient = this._daprClient;
			string str = string.Concat("shopping-cart-", userId);
			CancellationToken cancellationToken1 = cancellationToken;
			ConsistencyMode? nullable = null;
			StateEntry<CartDto> stateEntryAsync = await daprClient.GetStateEntryAsync<CartDto>("statestore", str, nullable, null, cancellationToken1);
			StateEntry<CartDto> stateEntry = stateEntryAsync;
			stateEntryAsync = null;
			if (stateEntry.get_Value() == null)
			{
				throw new CoreException(string.Concat("Couldn't find cart for user_id=", userId));
			}
			CartItemDto cartItemDto = stateEntry.get_Value().Items.FirstOrDefault<CartItemDto>((CartItemDto x) => x.ProductId == request.ProductId);
			if (cartItemDto != null)
			{
				CartItemDto quantity = cartItemDto;
				quantity.Quantity = quantity.Quantity + request.Quantity;
			}
			else
			{
				await CartDtoExtensions.InsertItemToCartAsync(stateEntry.get_Value(), request.Quantity, request.ProductId, this._productCatalogGateway);
			}
			await CartDtoExtensions.CalculateCartAsync(stateEntry.get_Value(), this._productCatalogGateway, this._shippingGateway, this._promoGateway);
			await stateEntry.SaveAsync(null, null, cancellationToken);
			CartDto value = stateEntry.get_Value();
			userId = null;
			stateEntry = null;
			cartItemDto = null;
			return value;
		}
	}
}