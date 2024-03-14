using Dapr.Client;
using Microsoft.Extensions.Logging;
using N8T.Core.Domain;
using ShoppingCart.Core.Dtos;
using ShoppingCart.Core.Gateways;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingCart.Infrastructure.Gateways
{
	[Nullable(0)]
	[NullableContext(1)]
	public class ProductCatalogGateway : IProductCatalogGateway
	{
		private readonly DaprClient _daprClient;

		private readonly ILogger<ProductCatalogGateway> _logger;

		public ProductCatalogGateway(DaprClient daprClient, ILogger<ProductCatalogGateway> logger)
		{
			DaprClient daprClient1 = daprClient;
			if (daprClient1 == null)
			{
				throw new ArgumentNullException("daprClient");
			}
			this._daprClient = daprClient1;
			ILogger<ProductCatalogGateway> logger1 = logger;
			if (logger1 == null)
			{
				throw new ArgumentNullException("logger");
			}
			this._logger = logger1;
		}

		[return: Nullable(new byte[] { 1, 2 })]
		public async Task<ProductDto> GetProductByIdAsync(Guid id, CancellationToken cancellationToken = null)
		{
			ProductDto productDto;
			ProductByIdRequest productByIdRequest;
			ILogger<ProductCatalogGateway> logger = this._logger;
			object[] objArray = new object[] { "ProductCatalogGateway", id };
			LoggerExtensions.LogInformation(logger, "{Prefix}: GetProductByIdAsync by id={Id}", objArray);
			DaprClient daprClient = this._daprClient;
			DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 1);
			defaultInterpolatedStringHandler.AppendLiteral("product-");
			defaultInterpolatedStringHandler.AppendFormatted<Guid>(id);
			string stringAndClear = defaultInterpolatedStringHandler.ToStringAndClear();
			CancellationToken cancellationToken1 = cancellationToken;
			ConsistencyMode? nullable = null;
			ProductDto stateAsync = await daprClient.GetStateAsync<ProductDto>("statestore", stringAndClear, nullable, null, cancellationToken1);
			ProductDto productDto1 = stateAsync;
			stateAsync = null;
			if (productDto1 == null)
			{
				productByIdRequest = new ProductByIdRequest()
				{
					Id = id
				};
				ProductDto productDto2 = await this._daprClient.InvokeMethodAsync<ProductByIdRequest, ProductDto>("productcatalogapp", "get-product-by-id", productByIdRequest, cancellationToken);
				productDto1 = productDto2;
				productDto2 = null;
				if (productDto1 == null)
				{
					defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(34, 1);
					defaultInterpolatedStringHandler.AppendLiteral("Couldn't find out product with id=");
					defaultInterpolatedStringHandler.AppendFormatted<Guid>(id);
					throw new CoreException(defaultInterpolatedStringHandler.ToStringAndClear());
				}
				DaprClient daprClient1 = this._daprClient;
				defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 1);
				defaultInterpolatedStringHandler.AppendLiteral("product-");
				defaultInterpolatedStringHandler.AppendFormatted<Guid>(id);
				await daprClient1.SaveStateAsync<ProductDto>("statestore", defaultInterpolatedStringHandler.ToStringAndClear(), productDto1, null, null, cancellationToken);
				productDto = productDto1;
			}
			else
			{
				productDto = productDto1;
			}
			productDto1 = null;
			productByIdRequest = null;
			return productDto;
		}
	}
}