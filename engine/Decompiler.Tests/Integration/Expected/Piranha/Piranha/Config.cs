using Piranha.Models;
using Piranha.Services;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

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
				TaskAwaiter<Param> awaiter = this._service.GetByKeyAsync(Config.ARCHIVE_PAGE_SIZE).GetAwaiter();
				Param result = awaiter.GetResult();
				if (result == null)
				{
					return 0;
				}
				return Convert.ToInt32(result.Value);
			}
			set
			{
				TaskAwaiter<Param> awaiter = this._service.GetByKeyAsync(Config.ARCHIVE_PAGE_SIZE).GetAwaiter();
				Param result = awaiter.GetResult() ?? new Param()
				{
					Key = Config.ARCHIVE_PAGE_SIZE
				};
				result.Value = value.ToString();
				this._service.SaveAsync(result).GetAwaiter().GetResult();
			}
		}

		public int CacheExpiresPages
		{
			get
			{
				TaskAwaiter<Param> awaiter = this._service.GetByKeyAsync(Config.CACHE_EXPIRES_PAGES).GetAwaiter();
				Param result = awaiter.GetResult();
				if (result == null)
				{
					return 0;
				}
				return Convert.ToInt32(result.Value);
			}
			set
			{
				TaskAwaiter<Param> awaiter = this._service.GetByKeyAsync(Config.CACHE_EXPIRES_PAGES).GetAwaiter();
				Param result = awaiter.GetResult() ?? new Param()
				{
					Key = Config.CACHE_EXPIRES_PAGES
				};
				result.Value = value.ToString();
				this._service.SaveAsync(result).GetAwaiter().GetResult();
			}
		}

		public int CacheExpiresPosts
		{
			get
			{
				TaskAwaiter<Param> awaiter = this._service.GetByKeyAsync(Config.CACHE_EXPIRES_POSTS).GetAwaiter();
				Param result = awaiter.GetResult();
				if (result == null)
				{
					return 0;
				}
				return Convert.ToInt32(result.Value);
			}
			set
			{
				TaskAwaiter<Param> awaiter = this._service.GetByKeyAsync(Config.CACHE_EXPIRES_POSTS).GetAwaiter();
				Param result = awaiter.GetResult() ?? new Param()
				{
					Key = Config.CACHE_EXPIRES_POSTS
				};
				result.Value = value.ToString();
				this._service.SaveAsync(result).GetAwaiter().GetResult();
			}
		}

		public bool CommentsApprove
		{
			get
			{
				TaskAwaiter<Param> awaiter = this._service.GetByKeyAsync(Config.COMMENTS_APPROVE).GetAwaiter();
				Param result = awaiter.GetResult();
				if (result == null)
				{
					return true;
				}
				return Convert.ToBoolean(result.Value);
			}
			set
			{
				TaskAwaiter<Param> awaiter = this._service.GetByKeyAsync(Config.COMMENTS_APPROVE).GetAwaiter();
				Param result = awaiter.GetResult() ?? new Param()
				{
					Key = Config.COMMENTS_APPROVE
				};
				result.Value = value.ToString();
				this._service.SaveAsync(result).GetAwaiter().GetResult();
			}
		}

		public int CommentsCloseAfterDays
		{
			get
			{
				TaskAwaiter<Param> awaiter = this._service.GetByKeyAsync(Config.COMMENTS_CLOSE_AFTER_DAYS).GetAwaiter();
				Param result = awaiter.GetResult();
				if (result == null)
				{
					return 0;
				}
				return Convert.ToInt32(result.Value);
			}
			set
			{
				TaskAwaiter<Param> awaiter = this._service.GetByKeyAsync(Config.COMMENTS_CLOSE_AFTER_DAYS).GetAwaiter();
				Param result = awaiter.GetResult() ?? new Param()
				{
					Key = Config.COMMENTS_CLOSE_AFTER_DAYS
				};
				result.Value = value.ToString();
				this._service.SaveAsync(result).GetAwaiter().GetResult();
			}
		}

		public bool CommentsEnabledForPages
		{
			get
			{
				TaskAwaiter<Param> awaiter = this._service.GetByKeyAsync(Config.COMMENTS_PAGES_ENABLED).GetAwaiter();
				Param result = awaiter.GetResult();
				if (result == null)
				{
					return false;
				}
				return Convert.ToBoolean(result.Value);
			}
			set
			{
				TaskAwaiter<Param> awaiter = this._service.GetByKeyAsync(Config.COMMENTS_PAGES_ENABLED).GetAwaiter();
				Param result = awaiter.GetResult() ?? new Param()
				{
					Key = Config.COMMENTS_PAGES_ENABLED
				};
				result.Value = value.ToString();
				this._service.SaveAsync(result).GetAwaiter().GetResult();
			}
		}

		public bool CommentsEnabledForPosts
		{
			get
			{
				TaskAwaiter<Param> awaiter = this._service.GetByKeyAsync(Config.COMMENTS_POSTS_ENABLED).GetAwaiter();
				Param result = awaiter.GetResult();
				if (result == null)
				{
					return true;
				}
				return Convert.ToBoolean(result.Value);
			}
			set
			{
				TaskAwaiter<Param> awaiter = this._service.GetByKeyAsync(Config.COMMENTS_POSTS_ENABLED).GetAwaiter();
				Param result = awaiter.GetResult() ?? new Param()
				{
					Key = Config.COMMENTS_POSTS_ENABLED
				};
				result.Value = value.ToString();
				this._service.SaveAsync(result).GetAwaiter().GetResult();
			}
		}

		public int CommentsPageSize
		{
			get
			{
				TaskAwaiter<Param> awaiter = this._service.GetByKeyAsync(Config.COMMENTS_PAGE_SIZE).GetAwaiter();
				Param result = awaiter.GetResult();
				if (result == null)
				{
					return 0;
				}
				return Convert.ToInt32(result.Value);
			}
			set
			{
				TaskAwaiter<Param> awaiter = this._service.GetByKeyAsync(Config.COMMENTS_PAGE_SIZE).GetAwaiter();
				Param result = awaiter.GetResult() ?? new Param()
				{
					Key = Config.COMMENTS_PAGE_SIZE
				};
				result.Value = value.ToString();
				this._service.SaveAsync(result).GetAwaiter().GetResult();
			}
		}

		public bool HierarchicalPageSlugs
		{
			get
			{
				TaskAwaiter<Param> awaiter = this._service.GetByKeyAsync(Config.PAGES_HIERARCHICAL_SLUGS).GetAwaiter();
				Param result = awaiter.GetResult();
				if (result == null)
				{
					return true;
				}
				return Convert.ToBoolean(result.Value);
			}
			set
			{
				TaskAwaiter<Param> awaiter = this._service.GetByKeyAsync(Config.PAGES_HIERARCHICAL_SLUGS).GetAwaiter();
				Param result = awaiter.GetResult() ?? new Param()
				{
					Key = Config.PAGES_HIERARCHICAL_SLUGS
				};
				result.Value = value.ToString();
				this._service.SaveAsync(result).GetAwaiter().GetResult();
			}
		}

		public bool HtmlExcerpt
		{
			get
			{
				TaskAwaiter<Param> awaiter = this._service.GetByKeyAsync(Config.HTML_EXCERPT).GetAwaiter();
				Param result = awaiter.GetResult();
				if (result == null)
				{
					return false;
				}
				return Convert.ToBoolean(result.Value);
			}
			set
			{
				TaskAwaiter<Param> awaiter = this._service.GetByKeyAsync(Config.HTML_EXCERPT).GetAwaiter();
				Param result = awaiter.GetResult() ?? new Param()
				{
					Key = Config.HTML_EXCERPT
				};
				result.Value = value.ToString();
				this._service.SaveAsync(result).GetAwaiter().GetResult();
			}
		}

		public bool ManagerDefaultCollapsedBlockGroupHeaders
		{
			get
			{
				TaskAwaiter<Param> awaiter = this._service.GetByKeyAsync(Config.MANAGER_DEFAULT_COLLAPSED_BLOCKGROUPHEADERS).GetAwaiter();
				Param result = awaiter.GetResult();
				if (result == null)
				{
					return false;
				}
				return Convert.ToBoolean(result.Value);
			}
			set
			{
				TaskAwaiter<Param> awaiter = this._service.GetByKeyAsync(Config.MANAGER_DEFAULT_COLLAPSED_BLOCKGROUPHEADERS).GetAwaiter();
				Param result = awaiter.GetResult() ?? new Param()
				{
					Key = Config.MANAGER_DEFAULT_COLLAPSED_BLOCKGROUPHEADERS
				};
				result.Value = value.ToString();
				this._service.SaveAsync(result).GetAwaiter().GetResult();
			}
		}

		public bool ManagerDefaultCollapsedBlocks
		{
			get
			{
				TaskAwaiter<Param> awaiter = this._service.GetByKeyAsync(Config.MANAGER_DEFAULT_COLLAPSED_BLOCKS).GetAwaiter();
				Param result = awaiter.GetResult();
				if (result == null)
				{
					return false;
				}
				return Convert.ToBoolean(result.Value);
			}
			set
			{
				TaskAwaiter<Param> awaiter = this._service.GetByKeyAsync(Config.MANAGER_DEFAULT_COLLAPSED_BLOCKS).GetAwaiter();
				Param result = awaiter.GetResult() ?? new Param()
				{
					Key = Config.MANAGER_DEFAULT_COLLAPSED_BLOCKS
				};
				result.Value = value.ToString();
				this._service.SaveAsync(result).GetAwaiter().GetResult();
			}
		}

		public int ManagerExpandedSitemapLevels
		{
			get
			{
				TaskAwaiter<Param> awaiter = this._service.GetByKeyAsync(Config.MANAGER_EXPANDED_SITEMAP_LEVELS).GetAwaiter();
				Param result = awaiter.GetResult();
				if (result == null)
				{
					return 0;
				}
				return Convert.ToInt32(result.Value);
			}
			set
			{
				TaskAwaiter<Param> awaiter = this._service.GetByKeyAsync(Config.MANAGER_EXPANDED_SITEMAP_LEVELS).GetAwaiter();
				Param result = awaiter.GetResult() ?? new Param()
				{
					Key = Config.MANAGER_EXPANDED_SITEMAP_LEVELS
				};
				result.Value = value.ToString();
				this._service.SaveAsync(result).GetAwaiter().GetResult();
			}
		}

		public int ManagerPageSize
		{
			get
			{
				TaskAwaiter<Param> awaiter = this._service.GetByKeyAsync(Config.MANAGER_PAGE_SIZE).GetAwaiter();
				Param result = awaiter.GetResult();
				if (result == null)
				{
					return 15;
				}
				return Convert.ToInt32(result.Value);
			}
			set
			{
				TaskAwaiter<Param> awaiter = this._service.GetByKeyAsync(Config.MANAGER_PAGE_SIZE).GetAwaiter();
				Param result = awaiter.GetResult() ?? new Param()
				{
					Key = Config.MANAGER_PAGE_SIZE
				};
				result.Value = value.ToString();
				this._service.SaveAsync(result).GetAwaiter().GetResult();
			}
		}

		public string MediaCDN
		{
			get
			{
				TaskAwaiter<Param> awaiter = this._service.GetByKeyAsync(Config.MEDIA_CDN_URL).GetAwaiter();
				Param result = awaiter.GetResult();
				if (result == null)
				{
					return null;
				}
				return result.Value;
			}
			set
			{
				TaskAwaiter<Param> awaiter = this._service.GetByKeyAsync(Config.MEDIA_CDN_URL).GetAwaiter();
				Param result = awaiter.GetResult() ?? new Param()
				{
					Key = Config.MEDIA_CDN_URL
				};
				if (!String.IsNullOrWhiteSpace(value) && !value.EndsWith("/"))
				{
					value = String.Concat(value, "/");
				}
				result.Value = value;
				this._service.SaveAsync(result).GetAwaiter().GetResult();
			}
		}

		public int PageRevisions
		{
			get
			{
				TaskAwaiter<Param> awaiter = this._service.GetByKeyAsync(Config.PAGE_REVISIONS).GetAwaiter();
				Param result = awaiter.GetResult();
				if (result == null)
				{
					return 10;
				}
				return Convert.ToInt32(result.Value);
			}
			set
			{
				TaskAwaiter<Param> awaiter = this._service.GetByKeyAsync(Config.PAGE_REVISIONS).GetAwaiter();
				Param result = awaiter.GetResult() ?? new Param()
				{
					Key = Config.PAGE_REVISIONS
				};
				result.Value = value.ToString();
				this._service.SaveAsync(result).GetAwaiter().GetResult();
			}
		}

		public int PostRevisions
		{
			get
			{
				TaskAwaiter<Param> awaiter = this._service.GetByKeyAsync(Config.POST_REVISIONS).GetAwaiter();
				Param result = awaiter.GetResult();
				if (result == null)
				{
					return 10;
				}
				return Convert.ToInt32(result.Value);
			}
			set
			{
				TaskAwaiter<Param> awaiter = this._service.GetByKeyAsync(Config.POST_REVISIONS).GetAwaiter();
				Param result = awaiter.GetResult() ?? new Param()
				{
					Key = Config.POST_REVISIONS
				};
				result.Value = value.ToString();
				this._service.SaveAsync(result).GetAwaiter().GetResult();
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
		}

		public Config(IParamService paramService)
		{
			this._service = paramService;
		}

		public Config(IApi api)
		{
			this._service = api.Params;
		}

		public void Dispose()
		{
			GC.SuppressFinalize(this);
		}
	}
}