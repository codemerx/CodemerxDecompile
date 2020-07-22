using Piranha;
using Piranha.Models;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Piranha.Web
{
	public class StartPageRouter
	{
		public StartPageRouter()
		{
			base();
			return;
		}

		public static async Task<IRouteResponse> InvokeAsync(IApi api, string url, Guid siteId)
		{
			V_0.api = api;
			V_0.url = url;
			V_0.siteId = siteId;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<IRouteResponse>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<StartPageRouter.u003cInvokeAsyncu003ed__0>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}