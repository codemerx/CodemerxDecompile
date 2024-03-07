using Piranha;
using Piranha.Cache;
using Piranha.Extend;
using Piranha.Extend.Fields;
using Piranha.Models;
using Piranha.Repositories;
using Piranha.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Piranha.Services
{
	public class PageService : IPageService
	{
		private readonly IPageRepository _repo;

		private readonly IContentFactory _factory;

		private readonly ISiteService _siteService;

		private readonly IParamService _paramService;

		private readonly IMediaService _mediaService;

		private readonly ICache _cache;

		private readonly ISearch _search;

		public PageService(IPageRepository repo, IContentFactory factory, ISiteService siteService, IParamService paramService, IMediaService mediaService, ICache cache = null, ISearch search = null)
		{
			this._repo = repo;
			this._factory = factory;
			this._siteService = siteService;
			this._paramService = paramService;
			this._mediaService = mediaService;
			this._search = search;
			if (App.CacheLevel > CacheLevel.Basic)
			{
				this._cache = cache;
			}
		}

		public async Task<T> CopyAsync<T>(T originalPage)
		where T : PageBase
		{
			ConfiguredTaskAwaitable<T> configuredTaskAwaitable = this.GetByIdAsync<T>(originalPage.Id).ConfigureAwait(false);
			T nullable = await configuredTaskAwaitable;
			nullable.Id = Guid.NewGuid();
			nullable.OriginalPageId = new Guid?(originalPage.Id);
			nullable.Title = String.Concat("Copy of ", nullable.Title);
			nullable.NavigationTitle = null;
			nullable.Slug = null;
			object obj = nullable;
			object obj1 = nullable;
			DateTime minValue = DateTime.MinValue;
			DateTime dateTime = minValue;
			obj1.LastModified = minValue;
			obj.Created = dateTime;
			nullable.Published = null;
			return nullable;
		}

		public async Task<T> CreateAsync<T>(string typeId = null)
		where T : PageBase
		{
			T t;
			if (String.IsNullOrEmpty(typeId))
			{
				typeId = typeof(T).Name;
			}
			PageType byId = App.PageTypes.GetById(typeId);
			if (byId == null)
			{
				t = default(T);
			}
			else
			{
				ConfiguredTaskAwaitable<T> configuredTaskAwaitable = this._factory.CreateAsync<T>(byId).ConfigureAwait(false);
				T commentsEnabledForPages = await configuredTaskAwaitable;
				using (Config config = new Config(this._paramService))
				{
					commentsEnabledForPages.EnableComments = config.CommentsEnabledForPages;
					commentsEnabledForPages.CloseCommentsAfterDays = config.CommentsCloseAfterDays;
				}
				t = commentsEnabledForPages;
			}
			return t;
		}

		public async Task DeleteAsync(Guid id)
		{
			ConfiguredTaskAwaitable<PageInfo> configuredTaskAwaitable = this.GetByIdAsync<PageInfo>(id).ConfigureAwait(false);
			PageInfo pageInfo = await configuredTaskAwaitable;
			if (pageInfo != null)
			{
				await this.DeleteAsync<PageInfo>(pageInfo).ConfigureAwait(false);
			}
		}

		public async Task DeleteAsync<T>(T model)
		where T : PageBase
		{
			App.Hooks.OnBeforeDelete<PageBase>(model);
			ConfiguredTaskAwaitable<IEnumerable<Guid>> configuredTaskAwaitable = this._repo.Delete(model.Id).ConfigureAwait(false);
			await configuredTaskAwaitable;
			App.Hooks.OnAfterDelete<PageBase>(model);
			if (this._search != null)
			{
				await this._search.DeletePageAsync(model);
			}
			ConfiguredTaskAwaitable configuredTaskAwaitable1 = this.RemoveFromCache(model).ConfigureAwait(false);
			await configuredTaskAwaitable1;
			configuredTaskAwaitable1 = this._siteService.InvalidateSitemapAsync(model.SiteId, true).ConfigureAwait(false);
			await configuredTaskAwaitable1;
		}

		public async Task DeleteCommentAsync(Guid id)
		{
			ConfiguredTaskAwaitable<Comment> configuredTaskAwaitable = this.GetCommentByIdAsync(id).ConfigureAwait(false);
			Comment comment = await configuredTaskAwaitable;
			if (comment != null)
			{
				await this.DeleteCommentAsync(comment).ConfigureAwait(false);
			}
		}

		public async Task DeleteCommentAsync(Comment model)
		{
			ConfiguredTaskAwaitable<PageInfo> configuredTaskAwaitable = this.GetByIdAsync<PageInfo>(model.ContentId).ConfigureAwait(false);
			PageInfo pageInfo = await configuredTaskAwaitable;
			if (pageInfo == null)
			{
				Guid contentId = model.ContentId;
				throw new ArgumentException(String.Concat("Could not find page with id ", contentId.ToString()));
			}
			App.Hooks.OnBeforeDelete<Comment>(model);
			ConfiguredTaskAwaitable configuredTaskAwaitable1 = this._repo.DeleteComment(model.Id).ConfigureAwait(false);
			await configuredTaskAwaitable1;
			App.Hooks.OnAfterDelete<Comment>(model);
			configuredTaskAwaitable1 = this.RemoveFromCache(pageInfo).ConfigureAwait(false);
			await configuredTaskAwaitable1;
		}

		public async Task DetachAsync<T>(T model)
		where T : PageBase
		{
			Guid? originalPageId = model.OriginalPageId;
			if (!originalPageId.HasValue)
			{
				throw new ValidationException("Page is not an copy");
			}
			ConfiguredTaskAwaitable<T> configuredTaskAwaitable = this.GetByIdAsync<T>(model.Id).ConfigureAwait(false);
			T t = await configuredTaskAwaitable;
			originalPageId = null;
			t.OriginalPageId = originalPageId;
			foreach (Block block in t.Blocks)
			{
				block.Id = Guid.Empty;
				if (!(block is BlockGroup))
				{
					continue;
				}
				foreach (Block item in ((BlockGroup)block).Items)
				{
					item.Id = Guid.Empty;
				}
			}
			await this.SaveAsync<T>(t).ConfigureAwait(false);
		}

		private async Task<Guid?> EnsureSiteIdAsync(Guid? siteId)
		{
			Guid? nullable;
			if (!siteId.HasValue)
			{
				ConfiguredTaskAwaitable<Site> configuredTaskAwaitable = this._siteService.GetDefaultAsync().ConfigureAwait(false);
				Site site = await configuredTaskAwaitable;
				if (site != null)
				{
					nullable = new Guid?(site.Id);
					return nullable;
				}
			}
			nullable = siteId;
			return nullable;
		}

		public Task<IEnumerable<DynamicPage>> GetAllAsync(Guid? siteId = null)
		{
			return this.GetAllAsync<DynamicPage>(siteId);
		}

		public async Task<IEnumerable<T>> GetAllAsync<T>(Guid? siteId = null)
		where T : PageBase
		{
			List<T> ts = new List<T>();
			IPageRepository pageRepository = this._repo;
			ConfiguredTaskAwaitable<Guid?> configuredTaskAwaitable = this.EnsureSiteIdAsync(siteId).ConfigureAwait(false);
			Guid? nullable = await configuredTaskAwaitable;
			ConfiguredTaskAwaitable<IEnumerable<Guid>> configuredTaskAwaitable1 = pageRepository.GetAll(nullable.Value).ConfigureAwait(false);
			pageRepository = null;
			foreach (Guid guid in await configuredTaskAwaitable1)
			{
				ConfiguredTaskAwaitable<T> configuredTaskAwaitable2 = this.GetByIdAsync<T>(guid).ConfigureAwait(false);
				T t = await configuredTaskAwaitable2;
				if (t == null)
				{
					continue;
				}
				ts.Add(t);
			}
			return ts;
		}

		public Task<IEnumerable<DynamicPage>> GetAllBlogsAsync(Guid? siteId = null)
		{
			return this.GetAllBlogsAsync<DynamicPage>(siteId);
		}

		public async Task<IEnumerable<T>> GetAllBlogsAsync<T>(Guid? siteId = null)
		where T : PageBase
		{
			List<T> ts = new List<T>();
			IPageRepository pageRepository = this._repo;
			ConfiguredTaskAwaitable<Guid?> configuredTaskAwaitable = this.EnsureSiteIdAsync(siteId).ConfigureAwait(false);
			Guid? nullable = await configuredTaskAwaitable;
			ConfiguredTaskAwaitable<IEnumerable<Guid>> configuredTaskAwaitable1 = pageRepository.GetAllBlogs(nullable.Value).ConfigureAwait(false);
			pageRepository = null;
			foreach (Guid guid in await configuredTaskAwaitable1)
			{
				ConfiguredTaskAwaitable<T> configuredTaskAwaitable2 = this.GetByIdAsync<T>(guid).ConfigureAwait(false);
				T t = await configuredTaskAwaitable2;
				if (t == null)
				{
					continue;
				}
				ts.Add(t);
			}
			return ts;
		}

		public Task<IEnumerable<Comment>> GetAllCommentsAsync(Guid? pageId = null, bool onlyApproved = true, int? page = null, int? pageSize = null)
		{
			return this.GetAllCommentsAsync(pageId, onlyApproved, false, page, pageSize);
		}

		private async Task<IEnumerable<Comment>> GetAllCommentsAsync(Guid? pageId = null, bool onlyApproved = true, bool onlyPending = false, int? page = null, int? pageSize = null)
		{
			ConfiguredTaskAwaitable<IEnumerable<Comment>> configuredTaskAwaitable;
			if (!page.HasValue)
			{
				page = new int?(0);
			}
			if (!pageSize.HasValue)
			{
				using (Config config = new Config(this._paramService))
				{
					pageSize = new int?(config.CommentsPageSize);
				}
			}
			IEnumerable<Comment> comments = null;
			if (!onlyPending)
			{
				configuredTaskAwaitable = this._repo.GetAllComments(pageId, onlyApproved, page.Value, pageSize.Value).ConfigureAwait(false);
				comments = await configuredTaskAwaitable;
			}
			else
			{
				configuredTaskAwaitable = this._repo.GetAllPendingComments(pageId, page.Value, pageSize.Value).ConfigureAwait(false);
				comments = await configuredTaskAwaitable;
			}
			foreach (Comment comment in comments)
			{
				App.Hooks.OnLoad<Comment>(comment);
			}
			return comments;
		}

		public async Task<IEnumerable<Guid>> GetAllDraftsAsync(Guid? siteId = null)
		{
			IPageRepository pageRepository = this._repo;
			ConfiguredTaskAwaitable<Guid?> configuredTaskAwaitable = this.EnsureSiteIdAsync(siteId).ConfigureAwait(false);
			Guid? nullable = await configuredTaskAwaitable;
			pageRepository = null;
			return await pageRepository.GetAllDrafts(nullable.Value);
		}

		public Task<IEnumerable<Comment>> GetAllPendingCommentsAsync(Guid? pageId = null, int? page = null, int? pageSize = null)
		{
			return this.GetAllCommentsAsync(pageId, false, true, page, pageSize);
		}

		public Task<DynamicPage> GetByIdAsync(Guid id)
		{
			return this.GetByIdAsync<DynamicPage>(id);
		}

		public async Task<T> GetByIdAsync<T>(Guid id)
		where T : PageBase
		{
			T t;
			ConfiguredTaskAwaitable<T> configuredTaskAwaitable;
			PageBase pageBase;
			PageBase pageBase1;
			PageBase pageBase2 = null;
			if (typeof(T) == typeof(PageInfo))
			{
				ICache cache = this._cache;
				if (cache != null)
				{
					pageBase1 = cache.Get<PageInfo>(String.Concat("PageInfo_", id.ToString()));
				}
				else
				{
					pageBase1 = null;
				}
				pageBase2 = pageBase1;
			}
			else if (!typeof(DynamicPage).IsAssignableFrom(typeof(T)))
			{
				ICache cache1 = this._cache;
				if (cache1 != null)
				{
					pageBase = cache1.Get<PageBase>(id.ToString());
				}
				else
				{
					pageBase = null;
				}
				pageBase2 = pageBase;
				if (pageBase2 != null)
				{
					ConfiguredTaskAwaitable<PageBase> configuredTaskAwaitable1 = this._factory.InitAsync<PageBase>(pageBase2, App.PageTypes.GetById(pageBase2.TypeId)).ConfigureAwait(false);
					await configuredTaskAwaitable1;
				}
			}
			if (pageBase2 == null)
			{
				configuredTaskAwaitable = this._repo.GetById<T>(id).ConfigureAwait(false);
				pageBase2 = await configuredTaskAwaitable;
				await this.OnLoadAsync(pageBase2, false).ConfigureAwait(false);
			}
			if (pageBase2 == null || !(pageBase2 is T))
			{
				t = default(T);
			}
			else
			{
				configuredTaskAwaitable = this.MapOriginalAsync<T>((T)pageBase2).ConfigureAwait(false);
				t = await configuredTaskAwaitable;
			}
			return t;
		}

		public Task<DynamicPage> GetBySlugAsync(string slug, Guid? siteId = null)
		{
			return this.GetBySlugAsync<DynamicPage>(slug, siteId);
		}

		public async Task<T> GetBySlugAsync<T>(string slug, Guid? siteId = null)
		where T : PageBase
		{
			T t;
			ConfiguredTaskAwaitable<T> configuredTaskAwaitable;
			Guid? nullable;
			PageBase pageBase;
			PageBase pageBase1;
			ConfiguredTaskAwaitable<Guid?> configuredTaskAwaitable1 = this.EnsureSiteIdAsync(siteId).ConfigureAwait(false);
			Guid? nullable1 = await configuredTaskAwaitable1;
			siteId = nullable1;
			PageBase pageBase2 = null;
			ICache cache = this._cache;
			if (cache != null)
			{
				nullable = cache.Get<Guid?>(String.Format("PageId_{0}_{1}", siteId, slug));
			}
			else
			{
				nullable1 = null;
				nullable = nullable1;
			}
			Guid? nullable2 = nullable;
			if (nullable2.HasValue)
			{
				if (typeof(T) == typeof(PageInfo))
				{
					ICache cache1 = this._cache;
					if (cache1 != null)
					{
						pageBase1 = cache1.Get<PageInfo>(String.Concat("PageInfo_", nullable2.ToString()));
					}
					else
					{
						pageBase1 = null;
					}
					pageBase2 = pageBase1;
				}
				else if (!typeof(DynamicPage).IsAssignableFrom(typeof(T)))
				{
					ICache cache2 = this._cache;
					if (cache2 != null)
					{
						pageBase = cache2.Get<PageBase>(nullable2.ToString());
					}
					else
					{
						pageBase = null;
					}
					pageBase2 = pageBase;
					if (pageBase2 != null)
					{
						ConfiguredTaskAwaitable<PageBase> configuredTaskAwaitable2 = this._factory.InitAsync<PageBase>(pageBase2, App.PageTypes.GetById(pageBase2.TypeId)).ConfigureAwait(false);
						await configuredTaskAwaitable2;
					}
				}
			}
			if (pageBase2 == null)
			{
				configuredTaskAwaitable = this._repo.GetBySlug<T>(slug, siteId.Value).ConfigureAwait(false);
				pageBase2 = await configuredTaskAwaitable;
				await this.OnLoadAsync(pageBase2, false).ConfigureAwait(false);
			}
			if (pageBase2 == null || !(pageBase2 is T))
			{
				t = default(T);
			}
			else
			{
				configuredTaskAwaitable = this.MapOriginalAsync<T>((T)pageBase2).ConfigureAwait(false);
				t = await configuredTaskAwaitable;
			}
			return t;
		}

		public Task<Comment> GetCommentByIdAsync(Guid id)
		{
			return this._repo.GetCommentById(id);
		}

		public Task<DynamicPage> GetDraftByIdAsync(Guid id)
		{
			return this.GetDraftByIdAsync<DynamicPage>(id);
		}

		public async Task<T> GetDraftByIdAsync<T>(Guid id)
		where T : PageBase
		{
			ConfiguredTaskAwaitable<T> configuredTaskAwaitable = this._repo.GetDraftById<T>(id).ConfigureAwait(false);
			T t = await configuredTaskAwaitable;
			ConfiguredTaskAwaitable configuredTaskAwaitable1 = this.OnLoadAsync(t, true).ConfigureAwait(false);
			await configuredTaskAwaitable1;
			return t;
		}

		public async Task<Guid?> GetIdBySlugAsync(string slug, Guid? siteId = null)
		{
			Guid? nullable;
			ConfiguredTaskAwaitable<Guid?> configuredTaskAwaitable = this.EnsureSiteIdAsync(siteId).ConfigureAwait(false);
			Guid? nullable1 = await configuredTaskAwaitable;
			siteId = nullable1;
			ICache cache = this._cache;
			if (cache != null)
			{
				nullable = cache.Get<Guid?>(String.Format("PageId_{0}_{1}", siteId, slug));
			}
			else
			{
				nullable1 = null;
				nullable = nullable1;
			}
			Guid? nullable2 = nullable;
			if (!nullable2.HasValue)
			{
				ConfiguredTaskAwaitable<PageInfo> configuredTaskAwaitable1 = this._repo.GetBySlug<PageInfo>(slug, siteId.Value).ConfigureAwait(false);
				PageInfo pageInfo = await configuredTaskAwaitable1;
				if (pageInfo != null)
				{
					nullable2 = new Guid?(pageInfo.Id);
				}
			}
			return nullable2;
		}

		public Task<DynamicPage> GetStartpageAsync(Guid? siteId = null)
		{
			return this.GetStartpageAsync<DynamicPage>(siteId);
		}

		public async Task<T> GetStartpageAsync<T>(Guid? siteId = null)
		where T : PageBase
		{
			T t;
			ConfiguredTaskAwaitable<T> configuredTaskAwaitable;
			PageBase pageBase;
			PageBase pageBase1;
			ConfiguredTaskAwaitable<Guid?> configuredTaskAwaitable1 = this.EnsureSiteIdAsync(siteId).ConfigureAwait(false);
			siteId = await configuredTaskAwaitable1;
			PageBase pageBase2 = null;
			if (typeof(T) == typeof(PageInfo))
			{
				ICache cache = this._cache;
				if (cache != null)
				{
					pageBase1 = cache.Get<PageInfo>(String.Format("PageInfo_{0}", siteId.Value));
				}
				else
				{
					pageBase1 = null;
				}
				pageBase2 = pageBase1;
			}
			else if (!typeof(DynamicPage).IsAssignableFrom(typeof(T)))
			{
				ICache cache1 = this._cache;
				if (cache1 != null)
				{
					pageBase = cache1.Get<PageBase>(String.Format("Page_{0}", siteId.Value));
				}
				else
				{
					pageBase = null;
				}
				pageBase2 = pageBase;
				if (pageBase2 != null)
				{
					ConfiguredTaskAwaitable<PageBase> configuredTaskAwaitable2 = this._factory.InitAsync<PageBase>(pageBase2, App.PageTypes.GetById(pageBase2.TypeId)).ConfigureAwait(false);
					await configuredTaskAwaitable2;
				}
			}
			if (pageBase2 == null)
			{
				configuredTaskAwaitable = this._repo.GetStartpage<T>(siteId.Value).ConfigureAwait(false);
				pageBase2 = await configuredTaskAwaitable;
				await this.OnLoadAsync(pageBase2, false).ConfigureAwait(false);
			}
			if (pageBase2 == null || !(pageBase2 is T))
			{
				t = default(T);
			}
			else
			{
				configuredTaskAwaitable = this.MapOriginalAsync<T>((T)pageBase2).ConfigureAwait(false);
				t = await configuredTaskAwaitable;
			}
			return t;
		}

		private bool IsPublished(PageBase model)
		{
			if (model == null || !model.Published.HasValue)
			{
				return false;
			}
			return model.Published.Value <= DateTime.Now;
		}

		private async Task<T> MapOriginalAsync<T>(T model)
		where T : PageBase
		{
			T t;
			if (model == null || !model.OriginalPageId.HasValue)
			{
				t = model;
			}
			else
			{
				Guid? originalPageId = model.OriginalPageId;
				ConfiguredTaskAwaitable<T> configuredTaskAwaitable = this.GetByIdAsync<T>(originalPageId.Value).ConfigureAwait(false);
				T t1 = await configuredTaskAwaitable;
				if (t1 == null)
				{
					t = default(T);
				}
				else
				{
					T id = default(T);
					if (!((object)model is DynamicPage))
					{
						id = Utils.DeepClone<T>(t1);
						configuredTaskAwaitable = this._factory.InitAsync<T>(id, App.PageTypes.GetById(id.TypeId)).ConfigureAwait(false);
						await configuredTaskAwaitable;
					}
					else
					{
						id = t1;
					}
					id.Id = model.Id;
					id.SiteId = model.SiteId;
					id.Title = model.Title;
					id.NavigationTitle = model.NavigationTitle;
					id.Slug = model.Slug;
					id.ParentId = model.ParentId;
					id.SortOrder = model.SortOrder;
					id.IsHidden = model.IsHidden;
					id.Route = model.Route;
					id.OriginalPageId = model.OriginalPageId;
					id.Published = model.Published;
					id.Created = model.Created;
					id.LastModified = model.LastModified;
					t = id;
				}
			}
			return t;
		}

		public async Task MoveAsync<T>(T model, Guid? parentId, int sortOrder)
		where T : PageBase
		{
			App.Hooks.OnBeforeSave<PageBase>(model);
			ConfiguredTaskAwaitable<IEnumerable<Guid>> configuredTaskAwaitable = this._repo.Move<T>(model, parentId, sortOrder).ConfigureAwait(false);
			IEnumerable<Guid> guids = await configuredTaskAwaitable;
			App.Hooks.OnAfterSave<PageBase>(model);
			ConfiguredTaskAwaitable configuredTaskAwaitable1 = this.RemoveFromCache(model).ConfigureAwait(false);
			await configuredTaskAwaitable1;
			if (this._cache != null)
			{
				foreach (Guid guid in guids)
				{
					ConfiguredTaskAwaitable<PageInfo> configuredTaskAwaitable2 = this.GetByIdAsync<PageInfo>(guid).ConfigureAwait(false);
					if (await configuredTaskAwaitable2 == null)
					{
						continue;
					}
					configuredTaskAwaitable1 = this.RemoveFromCache(model).ConfigureAwait(false);
					await configuredTaskAwaitable1;
				}
			}
			configuredTaskAwaitable1 = this._siteService.InvalidateSitemapAsync(model.SiteId, true).ConfigureAwait(false);
			await configuredTaskAwaitable1;
		}

		private async Task OnLoadAsync(PageBase model, bool isDraft = false)
		{
			Guid id;
			if (model != null)
			{
				IDynamicContent dynamicContent = model as IDynamicContent;
				if (dynamicContent == null)
				{
					ConfiguredTaskAwaitable<PageBase> configuredTaskAwaitable = this._factory.InitAsync<PageBase>(model, App.PageTypes.GetById(model.TypeId)).ConfigureAwait(false);
					await configuredTaskAwaitable;
				}
				else
				{
					ConfiguredTaskAwaitable<IDynamicContent> configuredTaskAwaitable1 = this._factory.InitDynamicAsync<IDynamicContent>(dynamicContent, App.PageTypes.GetById(model.TypeId)).ConfigureAwait(false);
					await configuredTaskAwaitable1;
				}
				if (model.PrimaryImage == null)
				{
					model.PrimaryImage = new ImageField();
				}
				Guid? parentId = model.PrimaryImage.Id;
				if (parentId.HasValue)
				{
					ImageField primaryImage = model.PrimaryImage;
					IMediaService mediaService = this._mediaService;
					parentId = model.PrimaryImage.Id;
					ConfiguredTaskAwaitable<Media> configuredTaskAwaitable2 = mediaService.GetByIdAsync(parentId.Value).ConfigureAwait(false);
					primaryImage.Media = await configuredTaskAwaitable2;
					primaryImage = null;
					if (model.PrimaryImage.Media == null)
					{
						parentId = null;
						model.PrimaryImage.Id = parentId;
					}
				}
				App.Hooks.OnLoad<PageBase>(model);
				if (!isDraft && this._cache != null && !(model is DynamicPage))
				{
					if (!(model is PageInfo))
					{
						ICache cache = this._cache;
						id = model.Id;
						cache.Set<PageBase>(id.ToString(), model);
					}
					else
					{
						ICache cache1 = this._cache;
						id = model.Id;
						cache1.Set<PageBase>(String.Concat("PageInfo_", id.ToString()), model);
					}
					this._cache.Set<Guid>(String.Format("PageId_{0}_{1}", model.SiteId, model.Slug), model.Id);
					parentId = model.ParentId;
					if (!parentId.HasValue && model.SortOrder == 0)
					{
						if (!(model is PageInfo))
						{
							this._cache.Set<PageBase>(String.Format("Page_{0}", model.SiteId), model);
						}
						else
						{
							this._cache.Set<PageBase>(String.Format("PageInfo_{0}", model.SiteId), model);
						}
					}
				}
			}
		}

		private async Task RemoveFromCache(PageBase model)
		{
			if (this._cache != null)
			{
				ICache cache = this._cache;
				Guid id = model.Id;
				cache.Remove(id.ToString());
				ICache cache1 = this._cache;
				id = model.Id;
				cache1.Remove(String.Concat("PageInfo_", id.ToString()));
				this._cache.Remove(String.Format("PageId_{0}_{1}", model.SiteId, model.Slug));
				if (!model.ParentId.HasValue && model.SortOrder == 0)
				{
					this._cache.Remove(String.Format("Page_{0}", model.SiteId));
					this._cache.Remove(String.Format("PageInfo_{0}", model.SiteId));
				}
				ConfiguredTaskAwaitable configuredTaskAwaitable = this._siteService.RemoveSitemapFromCacheAsync(model.SiteId).ConfigureAwait(false);
				await configuredTaskAwaitable;
			}
		}

		public Task SaveAsync<T>(T model)
		where T : PageBase
		{
			return this.SaveAsync<T>(model, false);
		}

		private async Task SaveAsync<T>(T model, bool isDraft)
		where T : PageBase
		{
			ConfiguredTaskAwaitable<PageInfo> configuredTaskAwaitable;
			ConfiguredTaskAwaitable configuredTaskAwaitable1;
			string str;
			string slug;
			if (model.Id == Guid.Empty)
			{
				model.Id = Guid.NewGuid();
			}
			ValidationContext validationContext = new ValidationContext((object)model);
			Validator.ValidateObject(model, validationContext, true);
			if (String.IsNullOrWhiteSpace(model.Title))
			{
				throw new ValidationException("The Title field is required");
			}
			if (String.IsNullOrWhiteSpace(model.TypeId))
			{
				throw new ValidationException("The TypeId field is required");
			}
			if (!String.IsNullOrWhiteSpace(model.Slug))
			{
				model.Slug = Utils.GenerateSlug(model.Slug, true);
			}
			else
			{
				string str1 = "";
				using (Config config = new Config(this._paramService))
				{
					if (config.HierarchicalPageSlugs && model.ParentId.HasValue)
					{
						Guid? parentId = model.ParentId;
						configuredTaskAwaitable = this.GetByIdAsync<PageInfo>(parentId.Value).ConfigureAwait(false);
						PageInfo pageInfo = await configuredTaskAwaitable;
						if (pageInfo != null)
						{
							slug = pageInfo.Slug;
						}
						else
						{
							slug = null;
						}
						string str2 = slug;
						if (!String.IsNullOrWhiteSpace(str2))
						{
							str1 = String.Concat(str2, "/");
						}
					}
					object obj = model;
					string str3 = str1;
					str = (!String.IsNullOrWhiteSpace(model.NavigationTitle) ? model.NavigationTitle : model.Title);
					obj.Slug = String.Concat(str3, Utils.GenerateSlug(str, true));
				}
				config = null;
				str1 = null;
			}
			if (String.IsNullOrWhiteSpace(model.Slug))
			{
				throw new ValidationException("The generated slug is empty as the title only contains special characters, please specify a slug to save the page.");
			}
			configuredTaskAwaitable = this._repo.GetById<PageInfo>(model.Id).ConfigureAwait(false);
			PageInfo pageInfo1 = await configuredTaskAwaitable;
			bool flag = this.IsPublished(pageInfo1) != this.IsPublished(model);
			IEnumerable<Guid> guids = new Guid[0];
			App.Hooks.OnBeforeSave<PageBase>(model);
			if (!(this.IsPublished(pageInfo1) & isDraft))
			{
				if (pageInfo1 == null & isDraft)
				{
					model.Published = null;
				}
				else if (pageInfo1 != null && !isDraft)
				{
					using (config = new Config(this._paramService))
					{
						configuredTaskAwaitable1 = this._repo.DeleteDraft(model.Id).ConfigureAwait(false);
						await configuredTaskAwaitable1;
						configuredTaskAwaitable1 = this._repo.CreateRevision(model.Id, config.PageRevisions).ConfigureAwait(false);
						await configuredTaskAwaitable1;
					}
					config = null;
				}
				ConfiguredTaskAwaitable<IEnumerable<Guid>> configuredTaskAwaitable2 = this._repo.Save<T>(model).ConfigureAwait(false);
				guids = await configuredTaskAwaitable2;
			}
			else
			{
				configuredTaskAwaitable1 = this._repo.SaveDraft<T>(model).ConfigureAwait(false);
				await configuredTaskAwaitable1;
			}
			App.Hooks.OnAfterSave<PageBase>(model);
			if (!isDraft && this._search != null)
			{
				await this._search.SavePageAsync(model);
			}
			configuredTaskAwaitable1 = this.RemoveFromCache(model).ConfigureAwait(false);
			await configuredTaskAwaitable1;
			if (this._cache != null)
			{
				foreach (Guid guid in guids)
				{
					configuredTaskAwaitable = this.GetByIdAsync<PageInfo>(guid).ConfigureAwait(false);
					if (await configuredTaskAwaitable == null)
					{
						continue;
					}
					configuredTaskAwaitable1 = this.RemoveFromCache(model).ConfigureAwait(false);
					await configuredTaskAwaitable1;
				}
			}
			if (flag || guids.Count<Guid>() > 0)
			{
				configuredTaskAwaitable1 = this._siteService.InvalidateSitemapAsync(model.SiteId, true).ConfigureAwait(false);
				await configuredTaskAwaitable1;
			}
		}

		public Task SaveCommentAndVerifyAsync(Guid pageId, Comment model)
		{
			return this.SaveCommentAsync(pageId, model, true);
		}

		public Task SaveCommentAsync(Guid pageId, Comment model)
		{
			return this.SaveCommentAsync(pageId, model, false);
		}

		private async Task SaveCommentAsync(Guid pageId, Comment model, bool verify)
		{
			ConfiguredTaskAwaitable<PageInfo> configuredTaskAwaitable = this.GetByIdAsync<PageInfo>(pageId).ConfigureAwait(false);
			PageInfo pageInfo = await configuredTaskAwaitable;
			if (pageInfo == null)
			{
				throw new ArgumentException(String.Concat("Could not find page with id ", pageId.ToString()));
			}
			if (model.Id == Guid.Empty)
			{
				model.Id = Guid.NewGuid();
			}
			if (model.Created == DateTime.MinValue)
			{
				model.Created = DateTime.Now;
			}
			Validator.ValidateObject(model, new ValidationContext(model), true);
			if (verify)
			{
				using (Config config = new Config(this._paramService))
				{
					model.IsApproved = config.CommentsApprove;
				}
				App.Hooks.OnValidate<Comment>(model);
			}
			App.Hooks.OnBeforeSave<Comment>(model);
			ConfiguredTaskAwaitable configuredTaskAwaitable1 = this._repo.SaveComment(pageId, model).ConfigureAwait(false);
			await configuredTaskAwaitable1;
			App.Hooks.OnAfterSave<Comment>(model);
			configuredTaskAwaitable1 = this.RemoveFromCache(pageInfo).ConfigureAwait(false);
			await configuredTaskAwaitable1;
		}

		public Task SaveDraftAsync<T>(T model)
		where T : PageBase
		{
			return this.SaveAsync<T>(model, true);
		}
	}
}