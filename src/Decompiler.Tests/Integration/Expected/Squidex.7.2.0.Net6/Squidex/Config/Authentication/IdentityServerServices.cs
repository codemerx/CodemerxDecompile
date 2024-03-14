using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Squidex.Config;
using Squidex.Hosting;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Squidex.Config.Authentication
{
	public static class IdentityServerServices
	{
		[NullableContext(1)]
		public static AuthenticationBuilder AddSquidexIdentityServerAuthentication(AuthenticationBuilder authBuilder, MyIdentityOptions identityOptions, IConfiguration config)
		{
			if (string.IsNullOrWhiteSpace(identityOptions.AuthorityUrl))
			{
				authBuilder.AddPolicyScheme("API", "API", (PolicySchemeOptions options) => options.set_ForwardDefaultSelector((HttpContext _) => "OpenIddict.Validation.AspNetCore"));
			}
			else
			{
				OpenIdConnectExtensions.AddOpenIdConnect(authBuilder, "ExternalIdentityServerSchema", (OpenIdConnectOptions options) => {
					options.set_Authority(identityOptions.AuthorityUrl);
					options.get_Scope().Add("email");
					options.get_Scope().Add("profile");
					options.get_Scope().Add("permissions");
					options.get_Scope().Add("squidex-api");
				});
				authBuilder.AddPolicyScheme("API", "API", (PolicySchemeOptions options) => options.set_ForwardDefaultSelector((HttpContext context) => "ExternalIdentityServerSchema"));
			}
			OpenIdConnectExtensions.AddOpenIdConnect(authBuilder);
			OptionsServiceCollectionExtensions.AddOptions<OpenIdConnectOptions>(authBuilder.get_Services(), "OpenIdConnect").Configure<IUrlGenerator>((OpenIdConnectOptions options, IUrlGenerator urlGenerator) => {
				if (string.IsNullOrWhiteSpace(identityOptions.AuthorityUrl))
				{
					options.set_Authority(urlGenerator.BuildUrl("/identity-server", false));
				}
				else
				{
					options.set_Authority(identityOptions.AuthorityUrl);
				}
				options.set_ClientId(Constants.ClientInternalId);
				options.set_ClientSecret(Constants.ClientInternalSecret);
				options.set_CallbackPath("/signin-internal");
				options.set_RequireHttpsMetadata(identityOptions.RequiresHttps);
				options.set_SaveTokens(true);
				options.get_Scope().Add("email");
				options.get_Scope().Add("profile");
				options.get_Scope().Add("permissions");
				options.set_SignInScheme("Cookies");
			});
			return authBuilder;
		}
	}
}