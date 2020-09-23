using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using OrchardCore.Environment.Shell.Builders;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace OrchardCore.Modules
{
	public class ModularTenantRouterMiddleware
	{
		private readonly IFeatureCollection _features;

		private readonly ILogger _logger;

		private readonly ConcurrentDictionary<string, SemaphoreSlim> _semaphores;

		public ModularTenantRouterMiddleware(IFeatureCollection features, RequestDelegate next, ILogger<ModularTenantRouterMiddleware> logger)
		{
			this._semaphores = new ConcurrentDictionary<string, SemaphoreSlim>();
			base();
			this._features = features;
			this._logger = logger;
			return;
		}

		private IShellPipeline BuildTenantPipeline()
		{
			V_0 = new ApplicationBuilder(ShellScope.get_Context().get_ServiceProvider(), this._features);
			stackVariable7 = ServiceProviderServiceExtensions.GetService<IEnumerable<IStartupFilter>>(V_0.get_ApplicationServices());
			V_1 = new ShellRequestPipeline();
			V_2 = new Action<IApplicationBuilder>(this.u003cBuildTenantPipelineu003eb__6_0);
			V_3 = stackVariable7.Reverse<IStartupFilter>().GetEnumerator();
			try
			{
				while (V_3.MoveNext())
				{
					V_2 = V_3.get_Current().Configure(V_2);
				}
			}
			finally
			{
				if (V_3 != null)
				{
					V_3.Dispose();
				}
			}
			V_2.Invoke(V_0);
			V_1.set_Next(V_0.Build());
			return V_1;
		}

		private void ConfigureTenantPipeline(IApplicationBuilder appBuilder)
		{
			V_0 = new ModularTenantRouterMiddleware.u003cu003ec__DisplayClass7_0();
			V_0.appBuilder = appBuilder;
			V_0.startups = ServiceProviderServiceExtensions.GetServices<IStartup>(V_0.appBuilder.get_ApplicationServices());
			stackVariable8 = V_0;
			stackVariable10 = V_0.startups;
			stackVariable11 = ModularTenantRouterMiddleware.u003cu003ec.u003cu003e9__7_0;
			if (stackVariable11 == null)
			{
				dummyVar0 = stackVariable11;
				stackVariable11 = new Func<IStartup, int>(ModularTenantRouterMiddleware.u003cu003ec.u003cu003e9.u003cConfigureTenantPipelineu003eb__7_0);
				ModularTenantRouterMiddleware.u003cu003ec.u003cu003e9__7_0 = stackVariable11;
			}
			stackVariable8.startups = stackVariable10.OrderBy<IStartup, int>(stackVariable11);
			dummyVar1 = EndpointRoutingApplicationBuilderExtensions.UseEndpoints(EndpointRoutingApplicationBuilderExtensions.UseRouting(V_0.appBuilder), new Action<IEndpointRouteBuilder>(V_0.u003cConfigureTenantPipelineu003eb__1));
			return;
		}

		private async Task InitializePipelineAsync(ShellContext shellContext)
		{
			V_0.u003cu003e4__this = this;
			V_0.shellContext = shellContext;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ModularTenantRouterMiddleware.u003cInitializePipelineAsyncu003ed__5>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task Invoke(HttpContext httpContext)
		{
			V_0.u003cu003e4__this = this;
			V_0.httpContext = httpContext;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ModularTenantRouterMiddleware.u003cInvokeu003ed__4>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}