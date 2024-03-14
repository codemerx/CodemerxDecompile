using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Scope;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace OrchardCore.Modules
{
	public class ModularTenantContainerMiddleware
	{
		private readonly RequestDelegate _next;

		private readonly IShellHost _shellHost;

		private readonly IRunningShellTable _runningShellTable;

		public ModularTenantContainerMiddleware(RequestDelegate next, IShellHost shellHost, IRunningShellTable runningShellTable)
		{
			this._next = next;
			this._shellHost = shellHost;
			this._runningShellTable = runningShellTable;
		}

		public async Task Invoke(HttpContext httpContext)
		{
			await this._shellHost.InitializeAsync();
			ShellSettings shellSetting = this._runningShellTable.Match(httpContext);
			if (shellSetting != null)
			{
				if (shellSetting.get_State() != 1)
				{
					HttpContextExtensions.UseShellScopeServices(httpContext);
					ShellScope scopeAsync = await this._shellHost.GetScopeAsync(shellSetting);
					IFeatureCollection features = httpContext.get_Features();
					ShellContextFeature shellContextFeature = new ShellContextFeature();
					shellContextFeature.set_ShellContext(scopeAsync.get_ShellContext());
					shellContextFeature.set_OriginalPath(httpContext.get_Request().get_Path());
					shellContextFeature.set_OriginalPathBase(httpContext.get_Request().get_PathBase());
					features.Set<ShellContextFeature>(shellContextFeature);
					await scopeAsync.UsingAsync((ShellScope scope) => this._next.Invoke(httpContext));
				}
				else
				{
					httpContext.get_Response().get_Headers().Add(HeaderNames.RetryAfter, "10");
					httpContext.get_Response().set_StatusCode(0x1f7);
					HttpResponse response = httpContext.get_Response();
					await HttpResponseWritingExtensions.WriteAsync(response, "The requested tenant is currently initializing.", new CancellationToken());
					return;
				}
			}
		}
	}
}