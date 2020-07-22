using Piranha.Models;
using Piranha.Services;
using System;
using System.Runtime.CompilerServices;

namespace Piranha
{
	public sealed class Config : IDisposable
	{
		private readonly IParamService _service;

		public readonly static string ARCHIVE_PAGE_SIZE;

		public readonly static string CACHE_EXPIRES_PAGES;

		public readonly static string CACHE_EXPIRES_POSTS;

		public readonly static string COMMENTS_PAGE_SIZE;

		public readonly static string COMMENTS_APPROVE;

		public readonly static string COMMENTS_POSTS_ENABLED;

		public readonly static string COMMENTS_PAGES_ENABLED;

		public readonly static string COMMENTS_CLOSE_AFTER_DAYS;

		public readonly static string HTML_EXCERPT;

		public readonly static string MEDIA_CDN_URL;

		public readonly static string MANAGER_EXPANDED_SITEMAP_LEVELS;

		public readonly static string MANAGER_PAGE_SIZE;

		public readonly static string MANAGER_DEFAULT_COLLAPSED_BLOCKS;

		public readonly static string MANAGER_DEFAULT_COLLAPSED_BLOCKGROUPHEADERS;

		public readonly static string PAGES_HIERARCHICAL_SLUGS;

		public readonly static string PAGE_REVISIONS;

		public readonly static string POST_REVISIONS;

		public int ArchivePageSize
		{
			get
			{
				V_1 = this._service.GetByKeyAsync(Config.ARCHIVE_PAGE_SIZE).GetAwaiter();
				V_0 = V_1.GetResult();
				if (V_0 == null)
				{
					return 0;
				}
				return Convert.ToInt32(V_0.get_Value());
			}
			set
			{
				V_1 = this._service.GetByKeyAsync(Config.ARCHIVE_PAGE_SIZE).GetAwaiter();
				V_0 = V_1.GetResult();
				if (V_0 == null)
				{
					stackVariable17 = new Param();
					stackVariable17.set_Key(Config.ARCHIVE_PAGE_SIZE);
					V_0 = stackVariable17;
				}
				V_0.set_Value(value.ToString());
				V_2 = this._service.SaveAsync(V_0).GetAwaiter();
				V_2.GetResult();
				return;
			}
		}

		public int CacheExpiresPages
		{
			get
			{
				V_1 = this._service.GetByKeyAsync(Config.CACHE_EXPIRES_PAGES).GetAwaiter();
				V_0 = V_1.GetResult();
				if (V_0 == null)
				{
					return 0;
				}
				return Convert.ToInt32(V_0.get_Value());
			}
			set
			{
				V_1 = this._service.GetByKeyAsync(Config.CACHE_EXPIRES_PAGES).GetAwaiter();
				V_0 = V_1.GetResult();
				if (V_0 == null)
				{
					stackVariable17 = new Param();
					stackVariable17.set_Key(Config.CACHE_EXPIRES_PAGES);
					V_0 = stackVariable17;
				}
				V_0.set_Value(value.ToString());
				V_2 = this._service.SaveAsync(V_0).GetAwaiter();
				V_2.GetResult();
				return;
			}
		}

		public int CacheExpiresPosts
		{
			get
			{
				V_1 = this._service.GetByKeyAsync(Config.CACHE_EXPIRES_POSTS).GetAwaiter();
				V_0 = V_1.GetResult();
				if (V_0 == null)
				{
					return 0;
				}
				return Convert.ToInt32(V_0.get_Value());
			}
			set
			{
				V_1 = this._service.GetByKeyAsync(Config.CACHE_EXPIRES_POSTS).GetAwaiter();
				V_0 = V_1.GetResult();
				if (V_0 == null)
				{
					stackVariable17 = new Param();
					stackVariable17.set_Key(Config.CACHE_EXPIRES_POSTS);
					V_0 = stackVariable17;
				}
				V_0.set_Value(value.ToString());
				V_2 = this._service.SaveAsync(V_0).GetAwaiter();
				V_2.GetResult();
				return;
			}
		}

		public bool CommentsApprove
		{
			get
			{
				V_1 = this._service.GetByKeyAsync(Config.COMMENTS_APPROVE).GetAwaiter();
				V_0 = V_1.GetResult();
				if (V_0 == null)
				{
					return true;
				}
				return Convert.ToBoolean(V_0.get_Value());
			}
			set
			{
				V_1 = this._service.GetByKeyAsync(Config.COMMENTS_APPROVE).GetAwaiter();
				V_0 = V_1.GetResult();
				if (V_0 == null)
				{
					stackVariable17 = new Param();
					stackVariable17.set_Key(Config.COMMENTS_APPROVE);
					V_0 = stackVariable17;
				}
				V_0.set_Value(value.ToString());
				V_2 = this._service.SaveAsync(V_0).GetAwaiter();
				V_2.GetResult();
				return;
			}
		}

		public int CommentsCloseAfterDays
		{
			get
			{
				V_1 = this._service.GetByKeyAsync(Config.COMMENTS_CLOSE_AFTER_DAYS).GetAwaiter();
				V_0 = V_1.GetResult();
				if (V_0 == null)
				{
					return 0;
				}
				return Convert.ToInt32(V_0.get_Value());
			}
			set
			{
				V_1 = this._service.GetByKeyAsync(Config.COMMENTS_CLOSE_AFTER_DAYS).GetAwaiter();
				V_0 = V_1.GetResult();
				if (V_0 == null)
				{
					stackVariable17 = new Param();
					stackVariable17.set_Key(Config.COMMENTS_CLOSE_AFTER_DAYS);
					V_0 = stackVariable17;
				}
				V_0.set_Value(value.ToString());
				V_2 = this._service.SaveAsync(V_0).GetAwaiter();
				V_2.GetResult();
				return;
			}
		}

		public bool CommentsEnabledForPages
		{
			get
			{
				V_1 = this._service.GetByKeyAsync(Config.COMMENTS_PAGES_ENABLED).GetAwaiter();
				V_0 = V_1.GetResult();
				if (V_0 == null)
				{
					return false;
				}
				return Convert.ToBoolean(V_0.get_Value());
			}
			set
			{
				V_1 = this._service.GetByKeyAsync(Config.COMMENTS_PAGES_ENABLED).GetAwaiter();
				V_0 = V_1.GetResult();
				if (V_0 == null)
				{
					stackVariable17 = new Param();
					stackVariable17.set_Key(Config.COMMENTS_PAGES_ENABLED);
					V_0 = stackVariable17;
				}
				V_0.set_Value(value.ToString());
				V_2 = this._service.SaveAsync(V_0).GetAwaiter();
				V_2.GetResult();
				return;
			}
		}

		public bool CommentsEnabledForPosts
		{
			get
			{
				V_1 = this._service.GetByKeyAsync(Config.COMMENTS_POSTS_ENABLED).GetAwaiter();
				V_0 = V_1.GetResult();
				if (V_0 == null)
				{
					return true;
				}
				return Convert.ToBoolean(V_0.get_Value());
			}
			set
			{
				V_1 = this._service.GetByKeyAsync(Config.COMMENTS_POSTS_ENABLED).GetAwaiter();
				V_0 = V_1.GetResult();
				if (V_0 == null)
				{
					stackVariable17 = new Param();
					stackVariable17.set_Key(Config.COMMENTS_POSTS_ENABLED);
					V_0 = stackVariable17;
				}
				V_0.set_Value(value.ToString());
				V_2 = this._service.SaveAsync(V_0).GetAwaiter();
				V_2.GetResult();
				return;
			}
		}

		public int CommentsPageSize
		{
			get
			{
				V_1 = this._service.GetByKeyAsync(Config.COMMENTS_PAGE_SIZE).GetAwaiter();
				V_0 = V_1.GetResult();
				if (V_0 == null)
				{
					return 0;
				}
				return Convert.ToInt32(V_0.get_Value());
			}
			set
			{
				V_1 = this._service.GetByKeyAsync(Config.COMMENTS_PAGE_SIZE).GetAwaiter();
				V_0 = V_1.GetResult();
				if (V_0 == null)
				{
					stackVariable17 = new Param();
					stackVariable17.set_Key(Config.COMMENTS_PAGE_SIZE);
					V_0 = stackVariable17;
				}
				V_0.set_Value(value.ToString());
				V_2 = this._service.SaveAsync(V_0).GetAwaiter();
				V_2.GetResult();
				return;
			}
		}

		public bool HierarchicalPageSlugs
		{
			get
			{
				V_1 = this._service.GetByKeyAsync(Config.PAGES_HIERARCHICAL_SLUGS).GetAwaiter();
				V_0 = V_1.GetResult();
				if (V_0 == null)
				{
					return true;
				}
				return Convert.ToBoolean(V_0.get_Value());
			}
			set
			{
				V_1 = this._service.GetByKeyAsync(Config.PAGES_HIERARCHICAL_SLUGS).GetAwaiter();
				V_0 = V_1.GetResult();
				if (V_0 == null)
				{
					stackVariable17 = new Param();
					stackVariable17.set_Key(Config.PAGES_HIERARCHICAL_SLUGS);
					V_0 = stackVariable17;
				}
				V_0.set_Value(value.ToString());
				V_2 = this._service.SaveAsync(V_0).GetAwaiter();
				V_2.GetResult();
				return;
			}
		}

		public bool HtmlExcerpt
		{
			get
			{
				V_1 = this._service.GetByKeyAsync(Config.HTML_EXCERPT).GetAwaiter();
				V_0 = V_1.GetResult();
				if (V_0 == null)
				{
					return false;
				}
				return Convert.ToBoolean(V_0.get_Value());
			}
			set
			{
				V_1 = this._service.GetByKeyAsync(Config.HTML_EXCERPT).GetAwaiter();
				V_0 = V_1.GetResult();
				if (V_0 == null)
				{
					stackVariable17 = new Param();
					stackVariable17.set_Key(Config.HTML_EXCERPT);
					V_0 = stackVariable17;
				}
				V_0.set_Value(value.ToString());
				V_2 = this._service.SaveAsync(V_0).GetAwaiter();
				V_2.GetResult();
				return;
			}
		}

		public bool ManagerDefaultCollapsedBlockGroupHeaders
		{
			get
			{
				V_1 = this._service.GetByKeyAsync(Config.MANAGER_DEFAULT_COLLAPSED_BLOCKGROUPHEADERS).GetAwaiter();
				V_0 = V_1.GetResult();
				if (V_0 == null)
				{
					return false;
				}
				return Convert.ToBoolean(V_0.get_Value());
			}
			set
			{
				V_1 = this._service.GetByKeyAsync(Config.MANAGER_DEFAULT_COLLAPSED_BLOCKGROUPHEADERS).GetAwaiter();
				V_0 = V_1.GetResult();
				if (V_0 == null)
				{
					stackVariable17 = new Param();
					stackVariable17.set_Key(Config.MANAGER_DEFAULT_COLLAPSED_BLOCKGROUPHEADERS);
					V_0 = stackVariable17;
				}
				V_0.set_Value(value.ToString());
				V_2 = this._service.SaveAsync(V_0).GetAwaiter();
				V_2.GetResult();
				return;
			}
		}

		public bool ManagerDefaultCollapsedBlocks
		{
			get
			{
				V_1 = this._service.GetByKeyAsync(Config.MANAGER_DEFAULT_COLLAPSED_BLOCKS).GetAwaiter();
				V_0 = V_1.GetResult();
				if (V_0 == null)
				{
					return false;
				}
				return Convert.ToBoolean(V_0.get_Value());
			}
			set
			{
				V_1 = this._service.GetByKeyAsync(Config.MANAGER_DEFAULT_COLLAPSED_BLOCKS).GetAwaiter();
				V_0 = V_1.GetResult();
				if (V_0 == null)
				{
					stackVariable17 = new Param();
					stackVariable17.set_Key(Config.MANAGER_DEFAULT_COLLAPSED_BLOCKS);
					V_0 = stackVariable17;
				}
				V_0.set_Value(value.ToString());
				V_2 = this._service.SaveAsync(V_0).GetAwaiter();
				V_2.GetResult();
				return;
			}
		}

		public int ManagerExpandedSitemapLevels
		{
			get
			{
				V_1 = this._service.GetByKeyAsync(Config.MANAGER_EXPANDED_SITEMAP_LEVELS).GetAwaiter();
				V_0 = V_1.GetResult();
				if (V_0 == null)
				{
					return 0;
				}
				return Convert.ToInt32(V_0.get_Value());
			}
			set
			{
				V_1 = this._service.GetByKeyAsync(Config.MANAGER_EXPANDED_SITEMAP_LEVELS).GetAwaiter();
				V_0 = V_1.GetResult();
				if (V_0 == null)
				{
					stackVariable17 = new Param();
					stackVariable17.set_Key(Config.MANAGER_EXPANDED_SITEMAP_LEVELS);
					V_0 = stackVariable17;
				}
				V_0.set_Value(value.ToString());
				V_2 = this._service.SaveAsync(V_0).GetAwaiter();
				V_2.GetResult();
				return;
			}
		}

		public int ManagerPageSize
		{
			get
			{
				V_1 = this._service.GetByKeyAsync(Config.MANAGER_PAGE_SIZE).GetAwaiter();
				V_0 = V_1.GetResult();
				if (V_0 == null)
				{
					return 15;
				}
				return Convert.ToInt32(V_0.get_Value());
			}
			set
			{
				V_1 = this._service.GetByKeyAsync(Config.MANAGER_PAGE_SIZE).GetAwaiter();
				V_0 = V_1.GetResult();
				if (V_0 == null)
				{
					stackVariable17 = new Param();
					stackVariable17.set_Key(Config.MANAGER_PAGE_SIZE);
					V_0 = stackVariable17;
				}
				V_0.set_Value(value.ToString());
				V_2 = this._service.SaveAsync(V_0).GetAwaiter();
				V_2.GetResult();
				return;
			}
		}

		public string MediaCDN
		{
			get
			{
				V_1 = this._service.GetByKeyAsync(Config.MEDIA_CDN_URL).GetAwaiter();
				V_0 = V_1.GetResult();
				if (V_0 == null)
				{
					return null;
				}
				return V_0.get_Value();
			}
			set
			{
				V_1 = this._service.GetByKeyAsync(Config.MEDIA_CDN_URL).GetAwaiter();
				V_0 = V_1.GetResult();
				if (V_0 == null)
				{
					stackVariable24 = new Param();
					stackVariable24.set_Key(Config.MEDIA_CDN_URL);
					V_0 = stackVariable24;
				}
				if (!String.IsNullOrWhiteSpace(value) && !value.EndsWith("/"))
				{
					value = String.Concat(value, "/");
				}
				V_0.set_Value(value);
				V_2 = this._service.SaveAsync(V_0).GetAwaiter();
				V_2.GetResult();
				return;
			}
		}

		public int PageRevisions
		{
			get
			{
				V_1 = this._service.GetByKeyAsync(Config.PAGE_REVISIONS).GetAwaiter();
				V_0 = V_1.GetResult();
				if (V_0 == null)
				{
					return 10;
				}
				return Convert.ToInt32(V_0.get_Value());
			}
			set
			{
				V_1 = this._service.GetByKeyAsync(Config.PAGE_REVISIONS).GetAwaiter();
				V_0 = V_1.GetResult();
				if (V_0 == null)
				{
					stackVariable17 = new Param();
					stackVariable17.set_Key(Config.PAGE_REVISIONS);
					V_0 = stackVariable17;
				}
				V_0.set_Value(value.ToString());
				V_2 = this._service.SaveAsync(V_0).GetAwaiter();
				V_2.GetResult();
				return;
			}
		}

		public int PostRevisions
		{
			get
			{
				V_1 = this._service.GetByKeyAsync(Config.POST_REVISIONS).GetAwaiter();
				V_0 = V_1.GetResult();
				if (V_0 == null)
				{
					return 10;
				}
				return Convert.ToInt32(V_0.get_Value());
			}
			set
			{
				V_1 = this._service.GetByKeyAsync(Config.POST_REVISIONS).GetAwaiter();
				V_0 = V_1.GetResult();
				if (V_0 == null)
				{
					stackVariable17 = new Param();
					stackVariable17.set_Key(Config.POST_REVISIONS);
					V_0 = stackVariable17;
				}
				V_0.set_Value(value.ToString());
				V_2 = this._service.SaveAsync(V_0).GetAwaiter();
				V_2.GetResult();
				return;
			}
		}

		static Config()
		{
			Config.ARCHIVE_PAGE_SIZE = "ArchivePageSize";
			Config.CACHE_EXPIRES_PAGES = "CacheExpiresPages";
			Config.CACHE_EXPIRES_POSTS = "CacheExpiresPosts";
			Config.COMMENTS_PAGE_SIZE = "CommentsPageSize";
			Config.COMMENTS_APPROVE = "CommentsApprove";
			Config.COMMENTS_POSTS_ENABLED = "CommentsPostsEnabled";
			Config.COMMENTS_PAGES_ENABLED = "CommentsPagesEnabled";
			Config.COMMENTS_CLOSE_AFTER_DAYS = "CommentsCloseAfterDays";
			Config.HTML_EXCERPT = "HtmlExcerpt";
			Config.MEDIA_CDN_URL = "MediaCdnUrl";
			Config.MANAGER_EXPANDED_SITEMAP_LEVELS = "ManagerExpandedSitemapLevels";
			Config.MANAGER_PAGE_SIZE = "ManagerPageSize";
			Config.MANAGER_DEFAULT_COLLAPSED_BLOCKS = "ManagerDefaultCollapsedBlocks";
			Config.MANAGER_DEFAULT_COLLAPSED_BLOCKGROUPHEADERS = "ManagerDefaultCollapsedBlockGroupHeaders";
			Config.PAGES_HIERARCHICAL_SLUGS = "HierarchicalPageSlugs";
			Config.PAGE_REVISIONS = "PageRevisions";
			Config.POST_REVISIONS = "PostRevisions";
			return;
		}

		public Config(IParamService paramService)
		{
			base();
			this._service = paramService;
			return;
		}

		public Config(IApi api)
		{
			base();
			this._service = api.get_Params();
			return;
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
			return;
		}
	}
}