using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Builders;
using OrchardCore.Environment.Shell.Scope;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace OrchardCore.Modules
{
	public class ModularTenantRouterMiddleware
	{
		private readonly IFeatureCollection _features;

		private readonly ILogger _logger;

		private readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores = new ConcurrentDictionary<string, SemaphoreSlim>();

		public ModularTenantRouterMiddleware(IFeatureCollection features, RequestDelegate next, ILogger<ModularTenantRouterMiddleware> logger)
		{
			this._features = features;
			this._logger = logger;
		}

		private IShellPipeline BuildTenantPipeline()
		{
			ApplicationBuilder applicationBuilder = new ApplicationBuilder(ShellScope.get_Context().get_ServiceProvider(), this._features);
			IEnumerable<IStartupFilter> service = ServiceProviderServiceExtensions.GetService<IEnumerable<IStartupFilter>>(applicationBuilder.get_ApplicationServices());
			ShellRequestPipeline shellRequestPipeline = new ShellRequestPipeline();
			Action<IApplicationBuilder> action = (IApplicationBuilder builder) => this.ConfigureTenantPipeline(builder);
			foreach (IStartupFilter startupFilter in service.Reverse<IStartupFilter>())
			{
				action = startupFilter.Configure(action);
			}
			action(applicationBuilder);
			shellRequestPipeline.set_Next(applicationBuilder.Build());
			return shellRequestPipeline;
		}

		private void ConfigureTenantPipeline(IApplicationBuilder appBuilder)
		{
			IEnumerable<IStartup> services = ServiceProviderServiceExtensions.GetServices<IStartup>(appBuilder.get_ApplicationServices());
			services = 
				from s in services
				orderby s.get_ConfigureOrder()
				select s;
			EndpointRoutingApplicationBuilderExtensions.UseEndpoints(EndpointRoutingApplicationBuilderExtensions.UseRouting(appBuilder), (IEndpointRouteBuilder routes) => {
				foreach (IStartup startup in services)
				{
					startup.Configure(appBuilder, routes, ShellScope.get_Services());
				}
			});
		}

		private async Task InitializePipelineAsync(ShellContext shellContext)
		{
			ConcurrentDictionary<string, SemaphoreSlim> strs = this._semaphores;
			string name = shellContext.get_Settings().get_Name();
			SemaphoreSlim orAdd = strs.GetOrAdd(name, (string _) => new SemaphoreSlim(1));
			await orAdd.WaitAsync();
			try
			{
				if (shellContext.get_Pipeline() == null)
				{
					shellContext.set_Pipeline(this.BuildTenantPipeline());
				}
			}
			finally
			{
				orAdd.Release();
			}
		}

		public async Task Invoke(HttpContext httpContext)
		{
			PathString pathString = new PathString();
			if (this._logger.IsEnabled(2))
			{
				LoggerExtensions.LogInformation(this._logger, "Begin Routing Request", Array.Empty<object>());
			}
			ShellContext context = ShellScope.get_Context();
			if (!string.IsNullOrEmpty(context.get_Settings().get_RequestUrlPrefix()))
			{
				PathString pathString1 = string.Concat("/", context.get_Settings().get_RequestUrlPrefix());
				HttpRequest request = httpContext.get_Request();
				request.set_PathBase(request.get_PathBase() + pathString1);
				PathString path = httpContext.get_Request().get_Path();
				path.StartsWithSegments(pathString1, StringComparison.OrdinalIgnoreCase, ref pathString);
				httpContext.get_Request().set_Path(pathString);
			}
			if (context.get_Pipeline() == null)
			{
				await this.InitializePipelineAsync(context);
			}
			await context.get_Pipeline().Invoke(httpContext);
		}
	}
}