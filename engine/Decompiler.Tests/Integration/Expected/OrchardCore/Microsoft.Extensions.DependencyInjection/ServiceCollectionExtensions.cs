using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.AspNetCore.StaticFiles.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using OrchardCore.Environment.Extensions;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Builders;
using OrchardCore.Environment.Shell.Configuration;
using OrchardCore.Environment.Shell.Descriptor.Models;
using OrchardCore.Modules;
using OrchardCore.Modules.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class ServiceCollectionExtensions
	{
		private static void AddAntiForgery(OrchardCoreBuilder builder)
		{
			AntiforgeryServiceCollectionExtensions.AddAntiforgery(builder.get_ApplicationServices());
			builder.ConfigureServices((IServiceCollection services, IServiceProvider serviceProvider) => {
				ShellSettings requiredService = ServiceProviderServiceExtensions.GetRequiredService<ShellSettings>(serviceProvider);
				IHostEnvironment hostEnvironment = ServiceProviderServiceExtensions.GetRequiredService<IHostEnvironment>(serviceProvider);
				string str = string.Concat("orchantiforgery_", HttpUtility.UrlEncode(string.Concat(requiredService.get_Name(), hostEnvironment.get_ContentRootPath())));
				if (requiredService.get_State() != null)
				{
					ServiceCollectionDescriptorExtensions.Add(services, AntiforgeryServiceCollectionExtensions.AddAntiforgery(new ServiceCollection(), (AntiforgeryOptions options) => options.get_Cookie().set_Name(str)));
					return;
				}
				IHttpContextAccessor httpContextAccessor = ServiceProviderServiceExtensions.GetRequiredService<IHttpContextAccessor>(serviceProvider);
				if (!httpContextAccessor.get_HttpContext().get_Response().get_HasStarted())
				{
					httpContextAccessor.get_HttpContext().get_Response().get_Cookies().Delete(str);
				}
			}, 0);
		}

		private static void AddAuthentication(OrchardCoreBuilder builder)
		{
			AuthenticationServiceCollectionExtensions.AddAuthentication(builder.get_ApplicationServices());
			builder.ConfigureServices((IServiceCollection services) => {
				AuthenticationServiceCollectionExtensions.AddAuthentication(services);
				ServiceCollectionServiceExtensions.AddSingleton<IAuthenticationSchemeProvider, AuthenticationSchemeProvider>(services);
			}, 0).Configure((IApplicationBuilder app) => AuthAppBuilderExtensions.UseAuthentication(app), 0);
		}

		private static void AddDataProtection(OrchardCoreBuilder builder)
		{
			// 
			// Current member / type: System.Void Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions::AddDataProtection(Microsoft.Extensions.DependencyInjection.OrchardCoreBuilder)
			// Exception in: System.Void AddDataProtection(Microsoft.Extensions.DependencyInjection.OrchardCoreBuilder)
			// Object reference not set to an instance of an object.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		private static void AddDefaultServices(IServiceCollection services)
		{
			LoggingServiceCollectionExtensions.AddLogging(services);
			OptionsServiceCollectionExtensions.AddOptions(services);
			LocalizationServiceCollectionExtensions.AddLocalization(services);
			ServiceCollectionServiceExtensions.AddSingleton<IStringLocalizerFactory, NullStringLocalizerFactory>(services);
			ServiceCollectionServiceExtensions.AddSingleton<IHtmlLocalizerFactory, NullHtmlLocalizerFactory>(services);
			EncoderServiceCollectionExtensions.AddWebEncoders(services);
			HttpServiceCollectionExtensions.AddHttpContextAccessor(services);
			ServiceCollectionServiceExtensions.AddSingleton<IClock, Clock>(services);
			ServiceCollectionServiceExtensions.AddScoped<ILocalClock, LocalClock>(services);
			ServiceCollectionServiceExtensions.AddScoped<ILocalizationService, DefaultLocalizationService>(services);
			ServiceCollectionServiceExtensions.AddScoped<ICalendarManager, DefaultCalendarManager>(services);
			ServiceCollectionServiceExtensions.AddScoped<ICalendarSelector, DefaultCalendarSelector>(services);
			ServiceCollectionServiceExtensions.AddSingleton<IPoweredByMiddlewareOptions, PoweredByMiddlewareOptions>(services);
			ServiceCollectionServiceExtensions.AddScoped<IOrchardHelper, DefaultOrchardHelper>(services);
		}

		private static void AddExtensionServices(OrchardCoreBuilder builder)
		{
			ServiceCollectionServiceExtensions.AddSingleton<IModuleNamesProvider, AssemblyAttributeModuleNamesProvider>(builder.get_ApplicationServices());
			ServiceCollectionServiceExtensions.AddSingleton<IApplicationContext, ModularApplicationContext>(builder.get_ApplicationServices());
			builder.get_ApplicationServices().AddExtensionManagerHost();
			builder.ConfigureServices((IServiceCollection services) => services.AddExtensionManager(), 0);
		}

		public static OrchardCoreBuilder AddOrchardCore(this IServiceCollection services)
		{
			object implementationInstance;
			if (services == null)
			{
				throw new ArgumentNullException("services");
			}
			ServiceDescriptor serviceDescriptor = services.LastOrDefault<ServiceDescriptor>((ServiceDescriptor d) => d.get_ServiceType() == typeof(OrchardCoreBuilder));
			if (serviceDescriptor != null)
			{
				implementationInstance = serviceDescriptor.get_ImplementationInstance();
			}
			else
			{
				implementationInstance = null;
			}
			OrchardCoreBuilder orchardCoreBuilder = implementationInstance as OrchardCoreBuilder;
			if (orchardCoreBuilder == null)
			{
				orchardCoreBuilder = new OrchardCoreBuilder(services);
				ServiceCollectionServiceExtensions.AddSingleton<OrchardCoreBuilder>(services, orchardCoreBuilder);
				Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.AddDefaultServices(services);
				Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.AddShellServices(services);
				Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.AddExtensionServices(orchardCoreBuilder);
				Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.AddStaticFiles(orchardCoreBuilder);
				Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.AddRouting(orchardCoreBuilder);
				Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.AddAntiForgery(orchardCoreBuilder);
				Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.AddSameSiteCookieBackwardsCompatibility(orchardCoreBuilder);
				Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.AddAuthentication(orchardCoreBuilder);
				Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.AddDataProtection(orchardCoreBuilder);
				ServiceCollectionServiceExtensions.AddSingleton<IServiceCollection>(services, services);
			}
			return orchardCoreBuilder;
		}

		public static IServiceCollection AddOrchardCore(this IServiceCollection services, Action<OrchardCoreBuilder> configure)
		{
			OrchardCoreBuilder orchardCoreBuilder = services.AddOrchardCore();
			if (configure != null)
			{
				configure(orchardCoreBuilder);
			}
			return services;
		}

		private static void AddRouting(OrchardCoreBuilder builder)
		{
			builder.ConfigureServices((IServiceCollection collection) => {
				Type[] array = RoutingServiceCollectionExtensions.AddRouting(new ServiceCollection()).Where<ServiceDescriptor>((ServiceDescriptor sd) => {
					if (sd.get_Lifetime() == null)
					{
						return true;
					}
					return sd.get_ServiceType() == typeof(IConfigureOptions<RouteOptions>);
				}).Select<ServiceDescriptor, Type>((ServiceDescriptor sd) => ServiceDescriptorExtensions.GetImplementationType(sd)).ToArray<Type>();
				ServiceDescriptor[] serviceDescriptorArray = collection.Where<ServiceDescriptor>((ServiceDescriptor sd) => {
					if (!(sd is ClonedSingletonDescriptor) && !(sd.get_ServiceType() == typeof(IConfigureOptions<RouteOptions>)))
					{
						return false;
					}
					return array.Contains<Type>(ServiceDescriptorExtensions.GetImplementationType(sd));
				}).ToArray<ServiceDescriptor>();
				for (int i = 0; i < (int)serviceDescriptorArray.Length; i++)
				{
					collection.Remove(serviceDescriptorArray[i]);
				}
				RoutingServiceCollectionExtensions.AddRouting(collection);
			}, -2147483548);
		}

		private static void AddSameSiteCookieBackwardsCompatibility(OrchardCoreBuilder builder)
		{
			builder.ConfigureServices((IServiceCollection services) => OptionsServiceCollectionExtensions.Configure<CookiePolicyOptions>(services, (CookiePolicyOptions options) => {
				options.set_MinimumSameSitePolicy(-1);
				options.set_OnAppendCookie((AppendCookieContext cookieContext) => Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.CheckSameSiteBackwardsCompatiblity(cookieContext.get_Context(), cookieContext.get_CookieOptions()));
				options.set_OnDeleteCookie((DeleteCookieContext cookieContext) => Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.CheckSameSiteBackwardsCompatiblity(cookieContext.get_Context(), cookieContext.get_CookieOptions()));
			}), 0).Configure((IApplicationBuilder app) => CookiePolicyAppBuilderExtensions.UseCookiePolicy(app), 0);
		}

		private static void AddShellServices(IServiceCollection services)
		{
			services.AddHostingShellServices();
			services.AddAllFeaturesDescriptor();
			ServiceCollectionServiceExtensions.AddTransient<ShellFeature>(services, (IServiceProvider sp) => new ShellFeature(ServiceProviderServiceExtensions.GetRequiredService<IHostEnvironment>(sp).get_ApplicationName(), true));
			ServiceCollectionServiceExtensions.AddTransient<ShellFeature>(services, (IServiceProvider sp) => new ShellFeature("Application.Default", true));
		}

		private static void AddStaticFiles(OrchardCoreBuilder builder)
		{
			builder.ConfigureServices((IServiceCollection services) => {
				ServiceCollectionServiceExtensions.AddSingleton<IModuleStaticFileProvider>(services, (IServiceProvider serviceProvider) => {
					IHostEnvironment requiredService = ServiceProviderServiceExtensions.GetRequiredService<IHostEnvironment>(serviceProvider);
					IApplicationContext applicationContext = ServiceProviderServiceExtensions.GetRequiredService<IApplicationContext>(serviceProvider);
					return (!HostEnvironmentEnvExtensions.IsDevelopment(requiredService) ? new ModuleEmbeddedStaticFileProvider(applicationContext) : new ModuleCompositeStaticFileProvider(new List<IStaticFileProvider>()
					{
						new ModuleProjectStaticFileProvider(applicationContext),
						new ModuleEmbeddedStaticFileProvider(applicationContext)
					}));
				});
				ServiceCollectionServiceExtensions.AddSingleton<IStaticFileProvider>(services, (IServiceProvider serviceProvider) => ServiceProviderServiceExtensions.GetRequiredService<IModuleStaticFileProvider>(serviceProvider));
			}, 0);
			builder.Configure((IApplicationBuilder app, IEndpointRouteBuilder routes, IServiceProvider serviceProvider) => {
				IModuleStaticFileProvider requiredService = ServiceProviderServiceExtensions.GetRequiredService<IModuleStaticFileProvider>(serviceProvider);
				StaticFileOptions value = ServiceProviderServiceExtensions.GetRequiredService<IOptions<StaticFileOptions>>(serviceProvider).get_Value();
				value.set_RequestPath("");
				value.set_FileProvider(requiredService);
				string str = ConfigurationBinder.GetValue<string>(ServiceProviderServiceExtensions.GetRequiredService<IShellConfiguration>(serviceProvider), "StaticFileOptions:CacheControl", "public, max-age=2592000, s-max-age=31557600");
				value.set_OnPrepareResponse((StaticFileResponseContext ctx) => ctx.get_Context().get_Response().get_Headers().set_Item(HeaderNames.CacheControl, str));
				StaticFileExtensions.UseStaticFiles(app, value);
			}, 0);
		}

		private static void CheckSameSiteBackwardsCompatiblity(HttpContext httpContext, CookieOptions options)
		{
			StringValues item = httpContext.get_Request().get_Headers().get_Item("User-Agent");
			string str = item.ToString();
			if (options.get_SameSite() == null)
			{
				if (string.IsNullOrEmpty(str))
				{
					return;
				}
				if (str.Contains("CPU iPhone OS 12") || str.Contains("iPad; CPU OS 12"))
				{
					options.set_SameSite(-1);
					return;
				}
				if (str.Contains("Macintosh; Intel Mac OS X 10_14") && str.Contains("Version/") && str.Contains("Safari"))
				{
					options.set_SameSite(-1);
					return;
				}
				if (str.Contains("Chrome/5") || str.Contains("Chrome/6"))
				{
					options.set_SameSite(-1);
				}
			}
		}
	}
}