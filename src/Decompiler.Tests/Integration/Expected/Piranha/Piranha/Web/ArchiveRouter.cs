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
	public class ArchiveRouter
	{
		public ArchiveRouter()
		{
		}

		public static async Task<IRouteResponse> InvokeAsync(IApi api, string url, Guid siteId)
		{
			IRouteResponse routeResponse;
			Taxonomy taxonomy;
			ConfiguredTaskAwaitable<Taxonomy> configuredTaskAwaitable;
			bool flag;
			Guid guid;
			Guid guid1;
			if (String.IsNullOrWhiteSpace(url) || url.Length <= 1)
			{
				routeResponse = null;
			}
			else
			{
				string str = url.Substring(1);
				Char[] chrArray = new Char[] { '/' };
				string[] strArray = str.Split(chrArray, StringSplitOptions.RemoveEmptyEntries);
				if ((int)strArray.Length >= 1)
				{
					int length = (int)strArray.Length;
					while (length > 0)
					{
						string str1 = String.Join("/", strArray.Subset<string>(0, length));
						ConfiguredTaskAwaitable<PageInfo> configuredTaskAwaitable1 = api.Pages.GetBySlugAsync<PageInfo>(str1, new Guid?(siteId)).ConfigureAwait(false);
						PageInfo pageInfo = await configuredTaskAwaitable1;
						if (pageInfo == null)
						{
							length--;
						}
						else
						{
							PageType byId = App.PageTypes.GetById(pageInfo.TypeId);
							if (byId == null || !byId.IsArchive)
							{
								routeResponse = null;
								return routeResponse;
							}
							else
							{
								if ((int)strArray.Length == length + 1)
								{
									try
									{
										int num = Convert.ToInt32(strArray[length]);
										if (num < 0x76c || num > DateTime.Now.Year)
										{
											routeResponse = null;
											return routeResponse;
										}
									}
									catch
									{
										routeResponse = null;
										return routeResponse;
									}
								}
								string route = pageInfo.Route;
								if (route == null)
								{
									route = "/archive";
								}
								string str2 = route;
								int? nullable = null;
								int? nullable1 = null;
								int? nullable2 = null;
								Guid? nullable3 = null;
								Guid? nullable4 = null;
								bool flag1 = false;
								bool flag2 = false;
								bool flag3 = false;
								for (int i = length; i < (int)strArray.Length; i++)
								{
									if (strArray[i] == "category" && !flag3)
									{
										flag1 = true;
									}
									else if (strArray[i] == "tag" && !flag3 && !flag1)
									{
										flag2 = true;
									}
									else if (strArray[i] != "page")
									{
										if (flag1)
										{
											try
											{
												configuredTaskAwaitable = api.Posts.GetCategoryBySlugAsync(pageInfo.Id, strArray[i]).ConfigureAwait(false);
												taxonomy = await configuredTaskAwaitable;
												guid1 = (taxonomy != null ? taxonomy.Id : Guid.Empty);
												nullable3 = new Guid?(guid1);
											}
											finally
											{
												flag1 = false;
											}
										}
										if (flag2)
										{
											try
											{
												configuredTaskAwaitable = api.Posts.GetTagBySlugAsync(pageInfo.Id, strArray[i]).ConfigureAwait(false);
												taxonomy = await configuredTaskAwaitable;
												guid = (taxonomy != null ? taxonomy.Id : Guid.Empty);
												nullable4 = new Guid?(guid);
											}
											finally
											{
												flag2 = false;
											}
										}
										if (flag3)
										{
											try
											{
												nullable = new int?(Convert.ToInt32(strArray[i]));
												break;
											}
											catch
											{
												break;
											}
										}
										else if (nullable1.HasValue)
										{
											try
											{
												nullable2 = new int?(Math.Max(Math.Min(Convert.ToInt32(strArray[i]), 12), 1));
											}
											catch
											{
											}
										}
										else
										{
											try
											{
												nullable1 = new int?(Convert.ToInt32(strArray[i]));
												if (nullable1.Value > DateTime.Now.Year)
												{
													nullable1 = new int?(DateTime.Now.Year);
												}
											}
											catch
											{
											}
										}
									}
									else
									{
										flag3 = true;
									}
								}
								RouteResponse routeResponse1 = new RouteResponse()
								{
									PageId = pageInfo.Id,
									Route = str2
								};
								Object[] id = new Object[] { pageInfo.Id, nullable1, nullable2, nullable, nullable, nullable3, nullable4 };
								routeResponse1.QueryString = String.Format("id={0}&year={1}&month={2}&page={3}&pagenum={4}&category={5}&tag={6}&piranha_handled=true", id);
								RouteResponse routeResponse2 = routeResponse1;
								flag = (!pageInfo.Published.HasValue ? false : pageInfo.Published.Value <= DateTime.Now);
								routeResponse2.IsPublished = flag;
								HttpCacheInfo httpCacheInfo = new HttpCacheInfo();
								Guid id1 = pageInfo.Id;
								httpCacheInfo.EntityTag = Utils.GenerateETag(id1.ToString(), pageInfo.LastModified);
								httpCacheInfo.LastModified = new DateTime?(pageInfo.LastModified);
								routeResponse1.CacheInfo = httpCacheInfo;
								routeResponse = routeResponse1;
								return routeResponse;
							}
						}
					}
					routeResponse = null;
				}
				else
				{
					routeResponse = null;
				}
			}
			return routeResponse;
		}
	}
}