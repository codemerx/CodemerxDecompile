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
	public class PageRouter
	{
		public PageRouter()
		{
		}

		public static async Task<IRouteResponse> InvokeAsync(IApi api, string url, Guid siteId)
		{
			IRouteResponse routeResponse;
			DateTime? published;
			bool value;
			DateTime lastModified;
			bool flag;
			bool value1;
			bool flag1;
			if (!String.IsNullOrWhiteSpace(url) && url.Length > 1)
			{
				string str = url.Substring(1);
				Char[] chrArray = new Char[] { '/' };
				string[] strArray = str.Split(chrArray, StringSplitOptions.RemoveEmptyEntries);
				int length = (int)strArray.Length;
				int num = length;
				while (num > 0)
				{
					string str1 = String.Join("/", strArray.Subset<string>(0, num));
					ConfiguredTaskAwaitable<PageInfo> configuredTaskAwaitable = api.Pages.GetBySlugAsync<PageInfo>(str1, new Guid?(siteId)).ConfigureAwait(false);
					PageInfo pageInfo = await configuredTaskAwaitable;
					if (pageInfo == null)
					{
						pageInfo = null;
						num--;
					}
					else
					{
						PageType byId = App.PageTypes.GetById(pageInfo.TypeId);
						if (byId == null || byId.IsArchive)
						{
							routeResponse = null;
							return routeResponse;
						}
						else if (!String.IsNullOrWhiteSpace(pageInfo.RedirectUrl))
						{
							RouteResponse redirectUrl = new RouteResponse();
							published = pageInfo.Published;
							if (!published.HasValue)
							{
								value = false;
							}
							else
							{
								published = pageInfo.Published;
								value = published.Value <= DateTime.Now;
							}
							redirectUrl.IsPublished = value;
							redirectUrl.RedirectUrl = pageInfo.RedirectUrl;
							redirectUrl.RedirectType = pageInfo.RedirectType;
							routeResponse = redirectUrl;
							return routeResponse;
						}
						else
						{
							Site byIdAsync = await api.Sites.GetByIdAsync(siteId);
							string route = pageInfo.Route;
							if (route == null)
							{
								route = "/page";
							}
							string str2 = route;
							published = byIdAsync.ContentLastModified;
							if (published.HasValue)
							{
								DateTime dateTime = pageInfo.LastModified;
								published = byIdAsync.ContentLastModified;
								flag1 = (published.HasValue ? dateTime > published.GetValueOrDefault() : false);
								if (flag1)
								{
									goto Label2;
								}
								published = byIdAsync.ContentLastModified;
								lastModified = published.Value;
								goto Label1;
							}
						Label2:
							lastModified = pageInfo.LastModified;
						Label1:
							DateTime dateTime1 = lastModified;
							if (num < length)
							{
								str2 = String.Concat(str2, "/", String.Join("/", strArray.Subset<string>(num, 0)));
							}
							flag = (pageInfo.ParentId.HasValue ? false : pageInfo.SortOrder == 0);
							bool flag2 = flag;
							RouteResponse routeResponse1 = new RouteResponse()
							{
								PageId = pageInfo.Id,
								Route = str2,
								QueryString = String.Format("id={0}&startpage={1}&piranha_handled=true", pageInfo.Id, flag2.ToString().ToLower())
							};
							published = pageInfo.Published;
							if (!published.HasValue)
							{
								value1 = false;
							}
							else
							{
								published = pageInfo.Published;
								value1 = published.Value <= DateTime.Now;
							}
							routeResponse1.IsPublished = value1;
							HttpCacheInfo httpCacheInfo = new HttpCacheInfo();
							Guid id = pageInfo.Id;
							httpCacheInfo.EntityTag = Utils.GenerateETag(id.ToString(), dateTime1);
							httpCacheInfo.LastModified = new DateTime?(dateTime1);
							routeResponse1.CacheInfo = httpCacheInfo;
							routeResponse = routeResponse1;
							return routeResponse;
						}
					}
				}
				strArray = null;
			}
			routeResponse = null;
			return routeResponse;
		}
	}
}