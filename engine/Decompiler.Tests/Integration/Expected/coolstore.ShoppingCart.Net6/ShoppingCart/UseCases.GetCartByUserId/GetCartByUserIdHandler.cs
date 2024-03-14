using Dapr;
using Dapr.Client;
using MediatR;
using N8T.Infrastructure.Auth;
using ShoppingCart.Core.Dtos;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingCart.UseCases.GetCartByUserId
{
	[Nullable(0)]
	[NullableContext(1)]
	public class GetCartByUserIdHandler : IRequestHandler<GetCartByUserIdQuery, CartDto>
	{
		private readonly DaprClient _daprClient;

		private readonly ISecurityContextAccessor _securityContextAccessor;

		public GetCartByUserIdHandler(DaprClient daprClient, ISecurityContextAccessor securityContextAccessor)
		{
			DaprClient daprClient1 = daprClient;
			if (daprClient1 == null)
			{
				throw new ArgumentNullException("daprClient");
			}
			this._daprClient = daprClient1;
			ISecurityContextAccessor securityContextAccessor1 = securityContextAccessor;
			if (securityContextAccessor1 == null)
			{
				throw new ArgumentNullException("securityContextAccessor");
			}
			this._securityContextAccessor = securityContextAccessor1;
		}

		public async Task<CartDto> Handle(GetCartByUserIdQuery request, CancellationToken cancellationToken)
		{
			CartDto value;
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
				stateEntry.set_Value(new CartDto());
				await stateEntry.SaveAsync(null, null, cancellationToken);
				value = stateEntry.get_Value();
			}
			else
			{
				value = stateEntry.get_Value();
			}
			userId = null;
			stateEntry = null;
			return value;
		}
	}
}