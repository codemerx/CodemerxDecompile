using Piranha.Repositories;
using Piranha.Services;
using System;
using System.Runtime.CompilerServices;

namespace Piranha
{
	public sealed class Api : IApi, IDisposable
	{
		private readonly ICache _cache;

		public IAliasService Aliases
		{
			get
			{
				return this.u003cAliasesu003ek__BackingField;
			}
		}

		public IArchiveService Archives
		{
			get
			{
				return this.u003cArchivesu003ek__BackingField;
			}
		}

		public bool IsCached
		{
			get
			{
				return this._cache != null;
			}
		}

		public IMediaService Media
		{
			get
			{
				return this.u003cMediau003ek__BackingField;
			}
		}

		public IPageService Pages
		{
			get
			{
				return this.u003cPagesu003ek__BackingField;
			}
		}

		public IPageTypeService PageTypes
		{
			get
			{
				return this.u003cPageTypesu003ek__BackingField;
			}
		}

		public IParamService Params
		{
			get
			{
				return this.u003cParamsu003ek__BackingField;
			}
		}

		public IPostService Posts
		{
			get
			{
				return this.u003cPostsu003ek__BackingField;
			}
		}

		public IPostTypeService PostTypes
		{
			get
			{
				return this.u003cPostTypesu003ek__BackingField;
			}
		}

		public ISiteService Sites
		{
			get
			{
				return this.u003cSitesu003ek__BackingField;
			}
		}

		public ISiteTypeService SiteTypes
		{
			get
			{
				return this.u003cSiteTypesu003ek__BackingField;
			}
		}

		public Api(IContentFactory contentFactory, IAliasRepository aliasRepository, IArchiveRepository archiveRepository, IMediaRepository mediaRepository, IPageRepository pageRepository, IPageTypeRepository pageTypeRepository, IParamRepository paramRepository, IPostRepository postRepository, IPostTypeRepository postTypeRepository, ISiteRepository siteRepository, ISiteTypeRepository siteTypeRepository, ICache cache = null, IStorage storage = null, IImageProcessor processor = null, ISearch search = null)
		{
			base();
			this._cache = cache;
			this.u003cPageTypesu003ek__BackingField = new PageTypeService(pageTypeRepository, cache);
			this.u003cParamsu003ek__BackingField = new ParamService(paramRepository, cache);
			this.u003cPostTypesu003ek__BackingField = new PostTypeService(postTypeRepository, cache);
			this.u003cSitesu003ek__BackingField = new SiteService(siteRepository, contentFactory, cache);
			this.u003cSiteTypesu003ek__BackingField = new SiteTypeService(siteTypeRepository, cache);
			this.u003cAliasesu003ek__BackingField = new AliasService(aliasRepository, this.get_Sites(), cache);
			this.u003cMediau003ek__BackingField = new MediaService(mediaRepository, this.get_Params(), storage, processor, cache);
			this.u003cPagesu003ek__BackingField = new PageService(pageRepository, contentFactory, this.get_Sites(), this.get_Params(), this.get_Media(), cache, search);
			this.u003cPostsu003ek__BackingField = new PostService(postRepository, contentFactory, this.get_Sites(), this.get_Pages(), this.get_Params(), this.get_Media(), cache, search);
			this.u003cArchivesu003ek__BackingField = new ArchiveService(archiveRepository, this.get_Params(), this.get_Posts());
			return;
		}

		public void Dispose()
		{
			return;
		}
	}
}