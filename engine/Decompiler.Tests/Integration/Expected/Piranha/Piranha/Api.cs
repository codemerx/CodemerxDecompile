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
			get;
		}

		public IArchiveService Archives
		{
			get;
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
			get;
		}

		public IPageService Pages
		{
			get;
		}

		public IPageTypeService PageTypes
		{
			get;
		}

		public IParamService Params
		{
			get;
		}

		public IPostService Posts
		{
			get;
		}

		public IPostTypeService PostTypes
		{
			get;
		}

		public ISiteService Sites
		{
			get;
		}

		public ISiteTypeService SiteTypes
		{
			get;
		}

		public Api(IContentFactory contentFactory, IAliasRepository aliasRepository, IArchiveRepository archiveRepository, IMediaRepository mediaRepository, IPageRepository pageRepository, IPageTypeRepository pageTypeRepository, IParamRepository paramRepository, IPostRepository postRepository, IPostTypeRepository postTypeRepository, ISiteRepository siteRepository, ISiteTypeRepository siteTypeRepository, ICache cache = null, IStorage storage = null, IImageProcessor processor = null, ISearch search = null)
		{
			this._cache = cache;
			this.PageTypes = new PageTypeService(pageTypeRepository, cache);
			this.Params = new ParamService(paramRepository, cache);
			this.PostTypes = new PostTypeService(postTypeRepository, cache);
			this.Sites = new SiteService(siteRepository, contentFactory, cache);
			this.SiteTypes = new SiteTypeService(siteTypeRepository, cache);
			this.Aliases = new AliasService(aliasRepository, this.Sites, cache);
			this.Media = new MediaService(mediaRepository, this.Params, storage, processor, cache);
			this.Pages = new PageService(pageRepository, contentFactory, this.Sites, this.Params, this.Media, cache, search);
			this.Posts = new PostService(postRepository, contentFactory, this.Sites, this.Pages, this.Params, this.Media, cache, search);
			this.Archives = new ArchiveService(archiveRepository, this.Params, this.Posts);
		}

		public void Dispose()
		{
		}
	}
}