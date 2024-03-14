using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.DependencyInjection;
using Squidex.Config;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Config.Authentication
{
	public static class MicrosoftAuthenticationServices
	{
		[NullableContext(1)]
		public static AuthenticationBuilder AddSquidexExternalMicrosoftAuthentication(AuthenticationBuilder authBuilder, MyIdentityOptions identityOptions)
		{
			if (identityOptions.IsMicrosoftAuthConfigured())
			{
				MicrosoftAccountExtensions.AddMicrosoftAccount(authBuilder, (MicrosoftAccountOptions options) => {
					options.set_ClientId(identityOptions.MicrosoftClient);
					options.set_ClientSecret(identityOptions.MicrosoftSecret);
					options.set_Events(new MicrosoftHandler());
					string microsoftTenant = identityOptions.MicrosoftTenant;
					if (!string.IsNullOrEmpty(microsoftTenant))
					{
						string str = "https://graph.microsoft.com";
						options.set_AuthorizationEndpoint(string.Concat("https://login.microsoftonline.com/", microsoftTenant, "/oauth2/authorize?resource=", str));
						options.set_TokenEndpoint(string.Concat("https://login.microsoftonline.com/", microsoftTenant, "/oauth2/token?resource=", str));
					}
				});
			}
			return authBuilder;
		}
	}
}