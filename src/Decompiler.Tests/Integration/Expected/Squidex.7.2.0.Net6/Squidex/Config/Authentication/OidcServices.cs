using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Squidex.Config;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Squidex.Config.Authentication
{
	public static class OidcServices
	{
		[NullableContext(1)]
		public static AuthenticationBuilder AddSquidexExternalOdic(AuthenticationBuilder authBuilder, MyIdentityOptions identityOptions)
		{
			if (identityOptions.IsOidcConfigured())
			{
				string str1 = (!string.IsNullOrWhiteSpace(identityOptions.OidcName) ? identityOptions.OidcName : OpenIdConnectDefaults.DisplayName);
				OpenIdConnectExtensions.AddOpenIdConnect(authBuilder, "ExternalOidc", str1, (OpenIdConnectOptions options) => {
					options.set_Authority(identityOptions.OidcAuthority);
					options.set_ClientId(identityOptions.OidcClient);
					options.set_ClientSecret(identityOptions.OidcSecret);
					options.set_RequireHttpsMetadata(identityOptions.RequiresHttps);
					options.set_Events(new OidcHandler(identityOptions));
					if (!string.IsNullOrEmpty(identityOptions.OidcMetadataAddress))
					{
						options.set_MetadataAddress(identityOptions.OidcMetadataAddress);
					}
					if (!string.IsNullOrEmpty(identityOptions.OidcResponseType))
					{
						options.set_ResponseType(identityOptions.OidcResponseType);
					}
					options.set_GetClaimsFromUserInfoEndpoint(identityOptions.OidcGetClaimsFromUserInfoEndpoint);
					if (identityOptions.OidcScopes != null)
					{
						string[] oidcScopes = identityOptions.OidcScopes;
						for (int i = 0; i < (int)oidcScopes.Length; i++)
						{
							string str = oidcScopes[i];
							options.get_Scope().Add(str);
						}
					}
				});
			}
			return authBuilder;
		}
	}
}