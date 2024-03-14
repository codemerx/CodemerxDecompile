using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.Extensions.DependencyInjection;
using Squidex.Config;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Config.Authentication
{
	public static class GoogleAuthenticationServices
	{
		[NullableContext(1)]
		public static AuthenticationBuilder AddSquidexExternalGoogleAuthentication(AuthenticationBuilder authBuilder, MyIdentityOptions identityOptions)
		{
			if (identityOptions.IsGoogleAuthConfigured())
			{
				GoogleExtensions.AddGoogle(authBuilder, (GoogleOptions options) => {
					options.set_ClientId(identityOptions.GoogleClient);
					options.set_ClientSecret(identityOptions.GoogleSecret);
					options.set_Events(new GoogleHandler());
				});
			}
			return authBuilder;
		}
	}
}