using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Configuration;
using OrchardCore.Environment.Shell.Descriptor.Models;
using OrchardCore.Modules;
using OrchardCore.Modules.FileProviders;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class ServiceCollectionExtensions
	{
		private static void AddAntiForgery(OrchardCoreBuilder builder)
		{
			dummyVar0 = AntiforgeryServiceCollectionExtensions.AddAntiforgery(builder.get_ApplicationServices());
			stackVariable3 = builder;
			stackVariable4 = Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9__7_0;
			if (stackVariable4 == null)
			{
				dummyVar1 = stackVariable4;
				stackVariable4 = new Action<IServiceCollection, IServiceProvider>(Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9.u003cAddAntiForgeryu003eb__7_0);
				Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9__7_0 = stackVariable4;
			}
			dummyVar2 = stackVariable3.ConfigureServices(stackVariable4, 0);
			return;
		}

		private static void AddAuthentication(OrchardCoreBuilder builder)
		{
			dummyVar0 = AuthenticationServiceCollectionExtensions.AddAuthentication(builder.get_ApplicationServices());
			stackVariable3 = builder;
			stackVariable4 = Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9__10_0;
			if (stackVariable4 == null)
			{
				dummyVar1 = stackVariable4;
				stackVariable4 = new Action<IServiceCollection>(Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9.u003cAddAuthenticationu003eb__10_0);
				Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9__10_0 = stackVariable4;
			}
			stackVariable6 = stackVariable3.ConfigureServices(stackVariable4, 0);
			stackVariable7 = Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9__10_1;
			if (stackVariable7 == null)
			{
				dummyVar2 = stackVariable7;
				stackVariable7 = new Action<IApplicationBuilder>(Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9.u003cAddAuthenticationu003eb__10_1);
				Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9__10_1 = stackVariable7;
			}
			dummyVar3 = stackVariable6.Configure(stackVariable7, 0);
			return;
		}

		private static void AddDataProtection(OrchardCoreBuilder builder)
		{
			stackVariable0 = builder;
			stackVariable1 = Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9__11_0;
			if (stackVariable1 == null)
			{
				dummyVar0 = stackVariable1;
				stackVariable1 = new Action<IServiceCollection, IServiceProvider>(Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9.u003cAddDataProtectionu003eb__11_0);
				Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9__11_0 = stackVariable1;
			}
			dummyVar1 = stackVariable0.ConfigureServices(stackVariable1, 0);
			return;
		}

		private static void AddDefaultServices(IServiceCollection services)
		{
			dummyVar0 = LoggingServiceCollectionExtensions.AddLogging(services);
			dummyVar1 = OptionsServiceCollectionExtensions.AddOptions(services);
			dummyVar2 = LocalizationServiceCollectionExtensions.AddLocalization(services);
			dummyVar3 = ServiceCollectionServiceExtensions.AddSingleton<IStringLocalizerFactory, NullStringLocalizerFactory>(services);
			dummyVar4 = ServiceCollectionServiceExtensions.AddSingleton<IHtmlLocalizerFactory, NullHtmlLocalizerFactory>(services);
			dummyVar5 = EncoderServiceCollectionExtensions.AddWebEncoders(services);
			dummyVar6 = HttpServiceCollectionExtensions.AddHttpContextAccessor(services);
			dummyVar7 = ServiceCollectionServiceExtensions.AddSingleton<IClock, Clock>(services);
			dummyVar8 = ServiceCollectionServiceExtensions.AddScoped<ILocalClock, LocalClock>(services);
			dummyVar9 = ServiceCollectionServiceExtensions.AddScoped<ILocalizationService, DefaultLocalizationService>(services);
			dummyVar10 = ServiceCollectionServiceExtensions.AddScoped<ICalendarManager, DefaultCalendarManager>(services);
			dummyVar11 = ServiceCollectionServiceExtensions.AddScoped<ICalendarSelector, DefaultCalendarSelector>(services);
			dummyVar12 = ServiceCollectionServiceExtensions.AddSingleton<IPoweredByMiddlewareOptions, PoweredByMiddlewareOptions>(services);
			dummyVar13 = ServiceCollectionServiceExtensions.AddScoped<IOrchardHelper, DefaultOrchardHelper>(services);
			return;
		}

		private static void AddExtensionServices(OrchardCoreBuilder builder)
		{
			dummyVar0 = ServiceCollectionServiceExtensions.AddSingleton<IModuleNamesProvider, AssemblyAttributeModuleNamesProvider>(builder.get_ApplicationServices());
			dummyVar1 = ServiceCollectionServiceExtensions.AddSingleton<IApplicationContext, ModularApplicationContext>(builder.get_ApplicationServices());
			dummyVar2 = builder.get_ApplicationServices().AddExtensionManagerHost();
			stackVariable9 = builder;
			stackVariable10 = Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9__4_0;
			if (stackVariable10 == null)
			{
				dummyVar3 = stackVariable10;
				stackVariable10 = new Action<IServiceCollection>(Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9.u003cAddExtensionServicesu003eb__4_0);
				Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9__4_0 = stackVariable10;
			}
			dummyVar4 = stackVariable9.ConfigureServices(stackVariable10, 0);
			return;
		}

		public static OrchardCoreBuilder AddOrchardCore(this IServiceCollection services)
		{
			if (services == null)
			{
				throw new ArgumentNullException("services");
			}
			stackVariable1 = services;
			stackVariable2 = Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9__0_0;
			if (stackVariable2 == null)
			{
				dummyVar0 = stackVariable2;
				stackVariable2 = new Func<ServiceDescriptor, bool>(Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9.u003cAddOrchardCoreu003eb__0_0);
				Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9__0_0 = stackVariable2;
			}
			stackVariable3 = stackVariable1.LastOrDefault<ServiceDescriptor>(stackVariable2);
			if (stackVariable3 != null)
			{
				stackVariable4 = stackVariable3.get_ImplementationInstance();
			}
			else
			{
				dummyVar1 = stackVariable3;
				stackVariable4 = null;
			}
			V_0 = stackVariable4 as OrchardCoreBuilder;
			if (V_0 == null)
			{
				V_0 = new OrchardCoreBuilder(services);
				dummyVar2 = ServiceCollectionServiceExtensions.AddSingleton<OrchardCoreBuilder>(services, V_0);
				Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.AddDefaultServices(services);
				Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.AddShellServices(services);
				Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.AddExtensionServices(V_0);
				Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.AddStaticFiles(V_0);
				Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.AddRouting(V_0);
				Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.AddAntiForgery(V_0);
				Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.AddSameSiteCookieBackwardsCompatibility(V_0);
				Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.AddAuthentication(V_0);
				Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.AddDataProtection(V_0);
				dummyVar3 = ServiceCollectionServiceExtensions.AddSingleton<IServiceCollection>(services, services);
			}
			return V_0;
		}

		public static IServiceCollection AddOrchardCore(this IServiceCollection services, Action<OrchardCoreBuilder> configure)
		{
			V_0 = services.AddOrchardCore();
			if (configure != null)
			{
				configure.Invoke(V_0);
			}
			return services;
		}

		private static void AddRouting(OrchardCoreBuilder builder)
		{
			stackVariable0 = builder;
			stackVariable1 = Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9__6_0;
			if (stackVariable1 == null)
			{
				dummyVar0 = stackVariable1;
				stackVariable1 = new Action<IServiceCollection>(Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9.u003cAddRoutingu003eb__6_0);
				Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9__6_0 = stackVariable1;
			}
			dummyVar1 = stackVariable0.ConfigureServices(stackVariable1, -2147483548);
			return;
		}

		private static void AddSameSiteCookieBackwardsCompatibility(OrchardCoreBuilder builder)
		{
			stackVariable0 = builder;
			stackVariable1 = Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9__8_0;
			if (stackVariable1 == null)
			{
				dummyVar0 = stackVariable1;
				stackVariable1 = new Action<IServiceCollection>(Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9.u003cAddSameSiteCookieBackwardsCompatibilityu003eb__8_0);
				Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9__8_0 = stackVariable1;
			}
			stackVariable3 = stackVariable0.ConfigureServices(stackVariable1, 0);
			stackVariable4 = Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9__8_1;
			if (stackVariable4 == null)
			{
				dummyVar1 = stackVariable4;
				stackVariable4 = new Action<IApplicationBuilder>(Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9.u003cAddSameSiteCookieBackwardsCompatibilityu003eb__8_1);
				Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9__8_1 = stackVariable4;
			}
			dummyVar2 = stackVariable3.Configure(stackVariable4, 0);
			return;
		}

		private static void AddShellServices(IServiceCollection services)
		{
			dummyVar0 = services.AddHostingShellServices();
			dummyVar1 = services.AddAllFeaturesDescriptor();
			stackVariable4 = services;
			stackVariable5 = Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9__3_0;
			if (stackVariable5 == null)
			{
				dummyVar2 = stackVariable5;
				stackVariable5 = new Func<IServiceProvider, ShellFeature>(Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9.u003cAddShellServicesu003eb__3_0);
				Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9__3_0 = stackVariable5;
			}
			dummyVar3 = ServiceCollectionServiceExtensions.AddTransient<ShellFeature>(stackVariable4, stackVariable5);
			stackVariable7 = services;
			stackVariable8 = Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9__3_1;
			if (stackVariable8 == null)
			{
				dummyVar4 = stackVariable8;
				stackVariable8 = new Func<IServiceProvider, ShellFeature>(Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9.u003cAddShellServicesu003eb__3_1);
				Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9__3_1 = stackVariable8;
			}
			dummyVar5 = ServiceCollectionServiceExtensions.AddTransient<ShellFeature>(stackVariable7, stackVariable8);
			return;
		}

		private static void AddStaticFiles(OrchardCoreBuilder builder)
		{
			stackVariable0 = builder;
			stackVariable1 = Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9__5_0;
			if (stackVariable1 == null)
			{
				dummyVar0 = stackVariable1;
				stackVariable1 = new Action<IServiceCollection>(Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9.u003cAddStaticFilesu003eb__5_0);
				Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9__5_0 = stackVariable1;
			}
			dummyVar1 = stackVariable0.ConfigureServices(stackVariable1, 0);
			stackVariable4 = builder;
			stackVariable5 = Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9__5_1;
			if (stackVariable5 == null)
			{
				dummyVar2 = stackVariable5;
				stackVariable5 = new Action<IApplicationBuilder, IEndpointRouteBuilder, IServiceProvider>(Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9.u003cAddStaticFilesu003eb__5_1);
				Microsoft.Extensions.DependencyInjection.ServiceCollectionExtensions.u003cu003ec.u003cu003e9__5_1 = stackVariable5;
			}
			dummyVar3 = stackVariable4.Configure(stackVariable5, 0);
			return;
		}

		private static void CheckSameSiteBackwardsCompatiblity(HttpContext httpContext, CookieOptions options)
		{
			V_1 = httpContext.get_Request().get_Headers().get_Item("User-Agent");
			V_0 = V_1.ToString();
			if (options.get_SameSite() == null)
			{
				if (string.IsNullOrEmpty(V_0))
				{
					return;
				}
				if (V_0.Contains("CPU iPhone OS 12") || V_0.Contains("iPad; CPU OS 12"))
				{
					options.set_SameSite(-1);
					return;
				}
				if (V_0.Contains("Macintosh; Intel Mac OS X 10_14") && V_0.Contains("Version/") && V_0.Contains("Safari"))
				{
					options.set_SameSite(-1);
					return;
				}
				if (V_0.Contains("Chrome/5") || V_0.Contains("Chrome/6"))
				{
					options.set_SameSite(-1);
				}
			}
			return;
		}
	}
}