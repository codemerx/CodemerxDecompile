using Piranha;
using Piranha.Models;
using Piranha.Services;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Piranha.Web
{
	public class PostRouter
	{
		public PostRouter()
		{
		}

		public static async Task<IRouteResponse> InvokeAsync(IApi api, string url, Guid siteId)
		{
			IRouteResponse routeResponse;
			Guid id;
			DateTime value;
			bool flag;
			HttpCacheInfo httpCacheInfo;
			bool flag1;
			if (!String.IsNullOrWhiteSpace(url) && url.Length > 1)
			{
				string str = url.Substring(1);
				Char[] chrArray = new Char[] { '/' };
				string[] strArray = str.Split(chrArray, StringSplitOptions.RemoveEmptyEntries);
				if ((int)strArray.Length >= 2)
				{
					PostInfo bySlugAsync = await api.Posts.GetBySlugAsync<PostInfo>(strArray[0], strArray[1], new Guid?(siteId));
					if (bySlugAsync == null)
					{
						bySlugAsync = null;
					}
					else
					{
						PageInfo byIdAsync = await api.Pages.GetByIdAsync<PageInfo>(bySlugAsync.BlogId);
						Site site = await api.Sites.GetByIdAsync(byIdAsync.SiteId);
						string route = bySlugAsync.Route;
						if (route == null)
						{
							route = "/post";
						}
						string str1 = route;
						DateTime? contentLastModified = site.ContentLastModified;
						if (contentLastModified.HasValue)
						{
							DateTime lastModified = bySlugAsync.LastModified;
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
						value = bySlugAsync.LastModified;
					Label1:
						DateTime dateTime = value;
						if ((int)strArray.Length > 2)
						{
							str1 = String.Concat(str1, "/", String.Join("/", strArray.Subset<string>(2, 0)));
						}
						RouteResponse routeResponse1 = new RouteResponse()
						{
							PageId = bySlugAsync.BlogId,
							Route = str1,
							QueryString = String.Format("id={0}&piranha_handled=true", bySlugAsync.Id)
						};
						contentLastModified = bySlugAsync.Published;
						if (contentLastModified.HasValue)
						{
							contentLastModified = byIdAsync.Published;
							if (contentLastModified.HasValue)
							{
								contentLastModified = bySlugAsync.Published;
								if (contentLastModified.Value > DateTime.Now)
								{
									flag = false;
									routeResponse1.IsPublished = flag;
									httpCacheInfo = new HttpCacheInfo();
									id = bySlugAsync.Id;
									httpCacheInfo.EntityTag = Utils.GenerateETag(id.ToString(), dateTime);
									httpCacheInfo.LastModified = new DateTime?(dateTime);
									routeResponse1.CacheInfo = httpCacheInfo;
									routeResponse = routeResponse1;
									return routeResponse;
								}
								contentLastModified = byIdAsync.Published;
								flag = contentLastModified.Value <= DateTime.Now;
								routeResponse1.IsPublished = flag;
								httpCacheInfo = new HttpCacheInfo();
								id = bySlugAsync.Id;
								httpCacheInfo.EntityTag = Utils.GenerateETag(id.ToString(), dateTime);
								httpCacheInfo.LastModified = new DateTime?(dateTime);
								routeResponse1.CacheInfo = httpCacheInfo;
								routeResponse = routeResponse1;
								return routeResponse;
							}
						}
						flag = false;
						routeResponse1.IsPublished = flag;
						httpCacheInfo = new HttpCacheInfo();
						id = bySlugAsync.Id;
						httpCacheInfo.EntityTag = Utils.GenerateETag(id.ToString(), dateTime);
						httpCacheInfo.LastModified = new DateTime?(dateTime);
						routeResponse1.CacheInfo = httpCacheInfo;
						routeResponse = routeResponse1;
						return routeResponse;
					}
				}
				strArray = null;
			}
			routeResponse = null;
			return routeResponse;
		}
	}
}