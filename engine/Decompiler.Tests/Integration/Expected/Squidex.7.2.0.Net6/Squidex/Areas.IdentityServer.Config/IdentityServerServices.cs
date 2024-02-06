using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenIddict.Server;
using Squidex.Config;
using Squidex.Domain.Users;
using Squidex.Hosting;
using Squidex.Web.Pipeline;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.IdentityServer.Config
{
	[Nullable(0)]
	[NullableContext(1)]
	public static class IdentityServerServices
	{
		public static void AddSquidexIdentityServer(this IServiceCollection services)
		{
			ConfigurationServiceExtensions.Configure<KeyManagementOptions>(services, (IServiceProvider c, KeyManagementOptions options) => options.set_XmlRepository(ServiceProviderServiceExtensions.GetRequiredService<IXmlRepository>(c)));
			DataProtectionBuilderExtensions.SetApplicationName(DataProtectionServiceCollectionExtensions.AddDataProtection(services), "Squidex");
			IdentityBuilderExtensions.AddDefaultTokenProviders(IdentityServiceCollectionExtensions.AddIdentity<IdentityUser, IdentityRole>(services));
			DependencyInjectionExtensions.AddSingletonAs<DefaultXmlRepository>(services).As<IXmlRepository>();
			DependencyInjectionExtensions.AddScopedAs<DefaultUserService>(services).As<IUserService>();
			DependencyInjectionExtensions.AddScopedAs<UserClaimsPrincipalFactoryWithEmail>(services).As<IUserClaimsPrincipalFactory<IdentityUser>>();
			DependencyInjectionExtensions.AddSingletonAs<ApiPermissionUnifier>(services).As<IClaimsTransformation>();
			DependencyInjectionExtensions.AddSingletonAs<TokenStoreInitializer>(services).AsSelf();
			DependencyInjectionExtensions.AddSingletonAs<CreateAdminInitializer>(services).AsSelf();
			OptionsServiceCollectionExtensions.ConfigureOptions<DefaultKeyStore>(services);
			OptionsServiceCollectionExtensions.Configure<IdentityOptions>(services, (IdentityOptions options) => {
				options.get_ClaimsIdentity().set_UserIdClaimType("sub");
				options.get_ClaimsIdentity().set_UserNameClaimType("name");
				options.get_ClaimsIdentity().set_RoleClaimType("role");
			});
			OpenIddictValidationExtensions.AddValidation(OpenIddictServerExtensions.AddServer(OpenIddictCoreExtensions.AddCore(OpenIddictExtensions.AddOpenIddict(services), (OpenIddictCoreBuilder builder) => {
				DependencyInjectionExtensions.AddSingletonAs<IdentityServerConfiguration.Scopes>(builder.get_Services()).As<IOpenIddictScopeStore<ImmutableScope>>();
				DependencyInjectionExtensions.AddSingletonAs<DynamicApplicationStore>(builder.get_Services()).As<IOpenIddictApplicationStore<ImmutableApplication>>();
				builder.ReplaceApplicationManager(typeof(ApplicationManager<>));
			}), (OpenIddictServerBuilder builder) => {
				builder.AddEventHandler<OpenIddictServerEvents.ProcessSignInContext>((OpenIddictServerHandlerDescriptor.Builder<OpenIddictServerEvents.ProcessSignInContext> argument0) => argument0.UseSingletonHandler<AlwaysAddTokenHandler>().SetOrder(OpenIddictServerHandlers.AttachTokenParameters.get_Descriptor().get_Order() + 1));
				builder.SetAccessTokenLifetime(new TimeSpan?(TimeSpan.FromDays(30)));
				builder.DisableAccessTokenEncryption();
				builder.RegisterScopes(new string[] { "email", "profile", "roles", "squidex-api", "permissions" });
				builder.AllowClientCredentialsFlow();
				builder.AllowImplicitFlow();
				builder.AllowAuthorizationCodeFlow();
				OpenIddictServerAspNetCoreExtensions.UseAspNetCore(builder).DisableTransportSecurityRequirement().EnableAuthorizationEndpointPassthrough().EnableLogoutEndpointPassthrough().EnableStatusCodePagesIntegration().EnableTokenEndpointPassthrough().EnableUserinfoEndpointPassthrough();
			}), (OpenIddictValidationBuilder options) => {
				OpenIddictValidationServerIntegrationExtensions.UseLocalServer(options);
				OpenIddictValidationAspNetCoreExtensions.UseAspNetCore(options);
			});
			ConfigurationServiceExtensions.Configure<AntiforgeryOptions>(services, (IServiceProvider c, AntiforgeryOptions options) => options.set_SuppressXFrameOptionsHeader(ServiceProviderServiceExtensions.GetRequiredService<IOptions<MyIdentityOptions>>(c).get_Value().SuppressXFrameOptionsHeader));
			ConfigurationServiceExtensions.Configure<OpenIddictServerOptions>(services, (IServiceProvider c, OpenIddictServerOptions options) => {
				Func<string, Uri> uri;
				IUrlGenerator requiredService = ServiceProviderServiceExtensions.GetRequiredService<IUrlGenerator>(c);
				string str = "/identity-server";
				if (!ServiceProviderServiceExtensions.GetRequiredService<IOptions<MyIdentityOptions>>(c).get_Value().MultipleDomains)
				{
					uri = (string url) => new Uri(requiredService.BuildUrl(string.Concat(str, url), false));
					options.set_Issuer(new Uri(requiredService.BuildUrl(str, false)));
				}
				else
				{
					uri = (string url) => new Uri(string.Concat(str, url), UriKind.Relative);
					options.set_Issuer(new Uri(requiredService.BuildUrl()));
				}
				options.get_AuthorizationEndpointUris().SetEndpoint(uri("/connect/authorize"));
				options.get_IntrospectionEndpointUris().SetEndpoint(uri("/connect/introspect"));
				options.get_LogoutEndpointUris().SetEndpoint(uri("/connect/logout"));
				options.get_TokenEndpointUris().SetEndpoint(uri("/connect/token"));
				options.get_UserinfoEndpointUris().SetEndpoint(uri("/connect/userinfo"));
				options.get_CryptographyEndpointUris().SetEndpoint(uri("/.well-known/jwks"));
				options.get_ConfigurationEndpointUris().SetEndpoint(uri("/.well-known/openid-configuration"));
			});
		}

		private static void SetEndpoint(this List<Uri> endpointUris, Uri uri)
		{
			endpointUris.Clear();
			endpointUris.Add(uri);
		}
	}
}