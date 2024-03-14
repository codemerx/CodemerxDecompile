using Piranha;
using Piranha.Models;
using Piranha.Services;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Piranha.Web
{
	public class AliasRouter
	{
		public AliasRouter()
		{
		}

		public static async Task<IRouteResponse> InvokeAsync(IApi api, string url, Guid siteId)
		{
			IRouteResponse routeResponse;
			if (!String.IsNullOrWhiteSpace(url) && url.Length > 1)
			{
				ConfiguredTaskAwaitable<Alias> configuredTaskAwaitable = api.Aliases.GetByAliasUrlAsync(url, new Guid?(siteId)).ConfigureAwait(false);
				Alias alia = await configuredTaskAwaitable;
				if (alia != null)
				{
					RouteResponse routeResponse1 = new RouteResponse()
					{
						IsPublished = true,
						RedirectUrl = alia.RedirectUrl,
						RedirectType = alia.Type
					};
					routeResponse = routeResponse1;
					return routeResponse;
				}
			}
			routeResponse = null;
			return routeResponse;
		}
	}
}