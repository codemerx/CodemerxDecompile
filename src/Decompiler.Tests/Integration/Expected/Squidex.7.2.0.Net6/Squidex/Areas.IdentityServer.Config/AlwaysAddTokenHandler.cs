using OpenIddict.Abstractions;
using OpenIddict.Server;
using System;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Squidex.Areas.IdentityServer.Config
{
	public sealed class AlwaysAddTokenHandler : IOpenIddictServerHandler<OpenIddictServerEvents.ProcessSignInContext>
	{
		public AlwaysAddTokenHandler()
		{
		}

		[NullableContext(1)]
		public ValueTask HandleAsync(OpenIddictServerEvents.ProcessSignInContext context)
		{
			ValueTask valueTask;
			ImmutableArray<string> scopes;
			if (context == null)
			{
				valueTask = new ValueTask();
				return valueTask;
			}
			if (!string.IsNullOrWhiteSpace(context.get_Response().get_AccessToken()))
			{
				ClaimsPrincipal accessTokenPrincipal = context.get_AccessTokenPrincipal();
				if (accessTokenPrincipal != null)
				{
					scopes = OpenIddictExtensions.GetScopes(accessTokenPrincipal);
				}
				else
				{
					scopes = ImmutableArray<string>.Empty;
				}
				ImmutableArray<string> strs = scopes;
				context.get_Response().set_Scope(string.Join(" ", strs));
			}
			valueTask = new ValueTask();
			return valueTask;
		}
	}
}