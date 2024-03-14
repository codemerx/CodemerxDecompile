using Piranha;
using Piranha.Models;
using Piranha.Runtime;
using Piranha.Services;
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
		}

		public static async Task<IRouteResponse> InvokeAsync(IApi api, string url, Guid siteId)
		{
			IRouteResponse routeResponse;
			DateTime value;
			bool flag;
			bool flag1;
			if (String.IsNullOrWhiteSpace(url) || url == "/")
			{
				ConfiguredTaskAwaitable<PageInfo> configuredTaskAwaitable = api.Pages.GetStartpageAsync<PageInfo>(new Guid?(siteId)).ConfigureAwait(false);
				PageInfo pageInfo = await configuredTaskAwaitable;
				if (pageInfo != null)
				{
					PageType byId = App.PageTypes.GetById(pageInfo.TypeId);
					if (byId != null)
					{
						if (byId.IsArchive)
						{
							ConfiguredTaskAwaitable<IRouteResponse> configuredTaskAwaitable1 = ArchiveRouter.InvokeAsync(api, String.Concat("/", pageInfo.Slug), siteId).ConfigureAwait(false);
							routeResponse = await configuredTaskAwaitable1;
							return routeResponse;
						}
						else
						{
							ConfiguredTaskAwaitable<Site> configuredTaskAwaitable2 = api.Sites.GetByIdAsync(siteId).ConfigureAwait(false);
							Site site = await configuredTaskAwaitable2;
							DateTime? contentLastModified = site.ContentLastModified;
							if (contentLastModified.HasValue)
							{
								DateTime lastModified = pageInfo.LastModified;
								contentLastModified = site.ContentLastModified;
								flag1 = (contentLastModified.HasValue ? lastModified > contentLastModified.GetValueOrDefault() : false);
								if (flag1)
								{
									goto Label2;
								}
								contentLastModified = site.ContentLastModified;
								value = contentLastModified.Value;
								goto Label1;
							}
						Label2:
							value = pageInfo.LastModified;
						Label1:
							DateTime dateTime = value;
							RouteResponse routeResponse1 = new RouteResponse()
							{
								PageId = pageInfo.Id
							};
							string route = pageInfo.Route;
							if (route == null)
							{
								route = "/page";
							}
							routeResponse1.Route = route;
							Guid id = pageInfo.Id;
							routeResponse1.QueryString = String.Concat("id=", id.ToString(), "&startpage=true&piranha_handled=true");
							contentLastModified = pageInfo.Published;
							if (!contentLastModified.HasValue)
							{
								flag = false;
							}
							else
							{
								contentLastModified = pageInfo.Published;
								flag = contentLastModified.Value <= DateTime.Now;
							}
							routeResponse1.IsPublished = flag;
							HttpCacheInfo httpCacheInfo = new HttpCacheInfo();
							id = pageInfo.Id;
							httpCacheInfo.EntityTag = Utils.GenerateETag(id.ToString(), dateTime);
							httpCacheInfo.LastModified = new DateTime?(dateTime);
							routeResponse1.CacheInfo = httpCacheInfo;
							routeResponse = routeResponse1;
							return routeResponse;
						}
					}
				}
				pageInfo = null;
			}
			routeResponse = null;
			return routeResponse;
		}
	}
}