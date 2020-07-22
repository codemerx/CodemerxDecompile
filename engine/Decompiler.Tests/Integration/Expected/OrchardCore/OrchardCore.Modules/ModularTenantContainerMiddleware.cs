using Microsoft.AspNetCore.Http;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Scope;
using System;
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
			base();
			this._next = next;
			this._shellHost = shellHost;
			this._runningShellTable = runningShellTable;
			return;
		}

		public async Task Invoke(HttpContext httpContext)
		{
			V_0.u003cu003e4__this = this;
			V_0.httpContext = httpContext;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ModularTenantContainerMiddleware.u003cInvokeu003ed__4>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}