using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Squidex.Config;
using Squidex.Hosting.Web;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Config.Authentication
{
	[Nullable(0)]
	[NullableContext(1)]
	public static class AuthenticationServices
	{
		public static void AddSquidexAuthentication(this IServiceCollection services, IConfiguration config)
		{
			MyIdentityOptions myIdentityOption = ConfigurationBinder.Get<MyIdentityOptions>(config.GetSection("identity")) ?? new MyIdentityOptions();
			IdentityServerServices.AddSquidexIdentityServerAuthentication(OidcServices.AddSquidexExternalOdic(MicrosoftAuthenticationServices.AddSquidexExternalMicrosoftAuthentication(GoogleAuthenticationServices.AddSquidexExternalGoogleAuthentication(GithubAuthenticationServices.AddSquidexExternalGithubAuthentication(AuthenticationServiceCollectionExtensions.AddAuthentication(services).AddSquidexCookies(config), myIdentityOption), myIdentityOption), myIdentityOption), myIdentityOption), myIdentityOption, config);
		}

		public static AuthenticationBuilder AddSquidexCookies(this AuthenticationBuilder builder, IConfiguration config)
		{
			UrlOptions urlOption = ConfigurationBinder.Get<UrlOptions>(config.GetSection("urls")) ?? new UrlOptions();
			IdentityServiceCollectionExtensions.ConfigureApplicationCookie(builder.get_Services(), (CookieAuthenticationOptions options) => {
				bool flag;
				options.set_AccessDeniedPath("/identity-server/account/access-denied");
				options.set_LoginPath("/identity-server/account/login");
				options.set_LogoutPath("/identity-server/account/login");
				options.get_Cookie().set_Name(".sq.auth2");
				string baseUrl = urlOption.get_BaseUrl();
				flag = (baseUrl != null ? baseUrl.StartsWith("https://", StringComparison.OrdinalIgnoreCase) : false);
				if (flag)
				{
					options.get_Cookie().set_SameSite(0);
				}
			});
			return CookieExtensions.AddCookie(builder);
		}
	}
}