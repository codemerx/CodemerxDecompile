using Dapr.Client;
using MediatR;
using N8T.Core.Helpers;
using N8T.Infrastructure.Auth;
using ShoppingCart.Core.Dtos;
using ShoppingCart.Core.Gateways;
using ShoppingCart.Infrastructure.Extensions;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingCart.UseCases.CreateShoppingCartWithProduct
{
	[Nullable(0)]
	[NullableContext(1)]
	public class CreateShoppingCartWithProductHandler : IRequestHandler<CreateShoppingCartWithProductCommand, CartDto>
	{
		private readonly DaprClient _daprClient;

		private readonly IProductCatalogGateway _productCatalogGateway;

		private readonly IPromoGateway _promoGateway;

		private readonly IShippingGateway _shippingGateway;

		private readonly ISecurityContextAccessor _securityContextAccessor;

		public CreateShoppingCartWithProductHandler(DaprClient daprClient, IProductCatalogGateway productCatalogGateway, IPromoGateway promoGateway, IShippingGateway shippingGateway, ISecurityContextAccessor securityContextAccessor)
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

		public async Task<CartDto> Handle(CreateShoppingCartWithProductCommand request, CancellationToken cancellationToken)
		{
			string userId = this._securityContextAccessor.get_UserId();
			CartDto cartDto = new CartDto()
			{
				Id = GuidHelper.NewGuid(null),
				UserId = userId
			};
			CartDto cartDto1 = cartDto;
			await CartDtoExtensions.InsertItemToCartAsync(cartDto1, request.Quantity, request.ProductId, this._productCatalogGateway);
			await CartDtoExtensions.CalculateCartAsync(cartDto1, this._productCatalogGateway, this._shippingGateway, this._promoGateway);
			await this._daprClient.SaveStateAsync<CartDto>("statestore", string.Concat("shopping-cart-", userId), cartDto1, null, null, cancellationToken);
			CartDto cartDto2 = cartDto1;
			userId = null;
			cartDto1 = null;
			return cartDto2;
		}
	}
}