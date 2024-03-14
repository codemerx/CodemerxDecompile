using Piranha;
using Piranha.Cache;
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
	public class PostService : IPostService
	{
		private readonly IPostRepository _repo;

		private readonly IContentFactory _factory;

		private readonly ISiteService _siteService;

		private readonly IPageService _pageService;

		private readonly IParamService _paramService;

		private readonly IMediaService _mediaService;

		private readonly ICache _cache;

		private readonly ISearch _search;

		public PostService(IPostRepository repo, IContentFactory factory, ISiteService siteService, IPageService pageService, IParamService paramService, IMediaService mediaService, ICache cache = null, ISearch search = null)
		{
			this._repo = repo;
			this._factory = factory;
			this._siteService = siteService;
			this._pageService = pageService;
			this._paramService = paramService;
			this._mediaService = mediaService;
			this._search = search;
			if (App.CacheLevel > CacheLevel.Basic)
			{
				this._cache = cache;
			}
		}

		public async Task<T> CreateAsync<T>(string typeId = null)
		where T : PostBase
		{
			T t;
			if (String.IsNullOrEmpty(typeId))
			{
				typeId = typeof(T).Name;
			}
			PostType byId = App.PostTypes.GetById(typeId);
			if (byId == null)
			{
				t = default(T);
			}
			else
			{
				ConfiguredTaskAwaitable<T> configuredTaskAwaitable = this._factory.CreateAsync<T>(byId).ConfigureAwait(false);
				T commentsEnabledForPosts = await configuredTaskAwaitable;
				using (Config config = new Config(this._paramService))
				{
					commentsEnabledForPosts.EnableComments = config.CommentsEnabledForPosts;
					commentsEnabledForPosts.CloseCommentsAfterDays = config.CommentsCloseAfterDays;
				}
				t = commentsEnabledForPosts;
			}
			return t;
		}

		public async Task DeleteAsync(Guid id)
		{
			ConfiguredTaskAwaitable<PostInfo> configuredTaskAwaitable = this.GetByIdAsync<PostInfo>(id).ConfigureAwait(false);
			PostInfo postInfo = await configuredTaskAwaitable;
			if (postInfo != null)
			{
				await this.DeleteAsync<PostInfo>(postInfo).ConfigureAwait(false);
			}
		}

		public async Task DeleteAsync<T>(T model)
		where T : PostBase
		{
			App.Hooks.OnBeforeDelete<PostBase>(model);
			ConfiguredTaskAwaitable configuredTaskAwaitable = this._repo.Delete(model.Id).ConfigureAwait(false);
			await configuredTaskAwaitable;
			App.Hooks.OnAfterDelete<PostBase>(model);
			if (this._search != null)
			{
				await this._search.DeletePostAsync(model);
			}
			this.RemoveFromCache(model);
		}

		public async Task DeleteCommentAsync(Guid id)
		{
			Comment commentByIdAsync = await this.GetCommentByIdAsync(id);
			if (commentByIdAsync != null)
			{
				await this.DeleteCommentAsync(commentByIdAsync).ConfigureAwait(false);
			}
		}

		public async Task DeleteCommentAsync(Comment model)
		{
			ConfiguredTaskAwaitable<PostInfo> configuredTaskAwaitable = this.GetByIdAsync<PostInfo>(model.ContentId).ConfigureAwait(false);
			PostInfo postInfo = await configuredTaskAwaitable;
			if (postInfo == null)
			{
				Guid contentId = model.ContentId;
				throw new ArgumentException(String.Concat("Could not find post with id ", contentId.ToString()));
			}
			App.Hooks.OnBeforeDelete<Comment>(model);
			await this._repo.DeleteComment(model.Id);
			App.Hooks.OnAfterDelete<Comment>(model);
			this.RemoveFromCache(postInfo);
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

		public Task<IEnumerable<DynamicPost>> GetAllAsync(Guid blogId, int? index = null, int? pageSize = null)
		{
			return this.GetAllAsync<DynamicPost>(blogId, index, pageSize);
		}

		public async Task<IEnumerable<T>> GetAllAsync<T>(Guid blogId, int? index = null, int? pageSize = null)
		where T : PostBase
		{
			if (index.HasValue && !pageSize.HasValue)
			{
				using (Config config = new Config(this._paramService))
				{
					pageSize = new int?(config.ArchivePageSize);
				}
			}
			List<T> ts = new List<T>();
			ConfiguredTaskAwaitable<IEnumerable<Guid>> configuredTaskAwaitable = this._repo.GetAll(blogId, index, pageSize).ConfigureAwait(false);
			IEnumerable<Guid> guids = await configuredTaskAwaitable;
			List<PageInfo> pageInfos = new List<PageInfo>();
			foreach (Guid guid in guids)
			{
				ConfiguredTaskAwaitable<T> configuredTaskAwaitable1 = this.GetByIdAsync<T>(guid, pageInfos).ConfigureAwait(false);
				T t = await configuredTaskAwaitable1;
				if (t == null)
				{
					continue;
				}
				ts.Add(t);
			}
			return ts;
		}

		public Task<IEnumerable<DynamicPost>> GetAllAsync(string slug, Guid? siteId = null)
		{
			return this.GetAllAsync<DynamicPost>(slug, siteId);
		}

		public async Task<IEnumerable<T>> GetAllAsync<T>(string slug, Guid? siteId = null)
		where T : PostBase
		{
			IEnumerable<T> ts;
			ConfiguredTaskAwaitable<Guid?> configuredTaskAwaitable = this.EnsureSiteIdAsync(siteId).ConfigureAwait(false);
			siteId = await configuredTaskAwaitable;
			configuredTaskAwaitable = this._pageService.GetIdBySlugAsync(slug, siteId).ConfigureAwait(false);
			Guid? nullable = await configuredTaskAwaitable;
			if (!nullable.HasValue)
			{
				ts = new List<T>();
			}
			else
			{
				int? nullable1 = null;
				int? nullable2 = nullable1;
				nullable1 = null;
				ConfiguredTaskAwaitable<IEnumerable<T>> configuredTaskAwaitable1 = this.GetAllAsync<T>(nullable.Value, nullable2, nullable1).ConfigureAwait(false);
				ts = await configuredTaskAwaitable1;
			}
			return ts;
		}

		public Task<IEnumerable<DynamicPost>> GetAllBySiteIdAsync(Guid? siteId = null)
		{
			return this.GetAllBySiteIdAsync<DynamicPost>(siteId);
		}

		public async Task<IEnumerable<T>> GetAllBySiteIdAsync<T>(Guid? siteId = null)
		where T : PostBase
		{
			List<T> ts = new List<T>();
			IPostRepository postRepository = this._repo;
			ConfiguredTaskAwaitable<Guid?> configuredTaskAwaitable = this.EnsureSiteIdAsync(siteId).ConfigureAwait(false);
			Guid? nullable = await configuredTaskAwaitable;
			ConfiguredTaskAwaitable<IEnumerable<Guid>> configuredTaskAwaitable1 = postRepository.GetAllBySiteId(nullable.Value).ConfigureAwait(false);
			postRepository = null;
			IEnumerable<Guid> guids = await configuredTaskAwaitable1;
			List<PageInfo> pageInfos = new List<PageInfo>();
			foreach (Guid guid in guids)
			{
				ConfiguredTaskAwaitable<T> configuredTaskAwaitable2 = this.GetByIdAsync<T>(guid, pageInfos).ConfigureAwait(false);
				T t = await configuredTaskAwaitable2;
				if (t == null)
				{
					continue;
				}
				ts.Add(t);
			}
			return ts;
		}

		public Task<IEnumerable<Taxonomy>> GetAllCategoriesAsync(Guid blogId)
		{
			return this._repo.GetAllCategories(blogId);
		}

		public Task<IEnumerable<Comment>> GetAllCommentsAsync(Guid? postId = null, bool onlyApproved = true, int? page = null, int? pageSize = null)
		{
			return this.GetAllCommentsAsync(postId, onlyApproved, false, page, pageSize);
		}

		private async Task<IEnumerable<Comment>> GetAllCommentsAsync(Guid? postId = null, bool onlyApproved = true, bool onlyPending = false, int? page = null, int? pageSize = null)
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
				configuredTaskAwaitable = this._repo.GetAllComments(postId, onlyApproved, page.Value, pageSize.Value).ConfigureAwait(false);
				comments = await configuredTaskAwaitable;
			}
			else
			{
				configuredTaskAwaitable = this._repo.GetAllPendingComments(postId, page.Value, pageSize.Value).ConfigureAwait(false);
				comments = await configuredTaskAwaitable;
			}
			foreach (Comment comment in comments)
			{
				App.Hooks.OnLoad<Comment>(comment);
			}
			return comments;
		}

		public Task<IEnumerable<Guid>> GetAllDraftsAsync(Guid blogId)
		{
			return this._repo.GetAllDrafts(blogId);
		}

		public Task<IEnumerable<Comment>> GetAllPendingCommentsAsync(Guid? postId = null, int? page = null, int? pageSize = null)
		{
			return this.GetAllCommentsAsync(postId, false, true, page, pageSize);
		}

		public Task<IEnumerable<Taxonomy>> GetAllTagsAsync(Guid blogId)
		{
			return this._repo.GetAllTags(blogId);
		}

		public Task<DynamicPost> GetByIdAsync(Guid id)
		{
			return this.GetByIdAsync<DynamicPost>(id);
		}

		public Task<T> GetByIdAsync<T>(Guid id)
		where T : PostBase
		{
			return this.GetByIdAsync<T>(id, new List<PageInfo>());
		}

		private async Task<T> GetByIdAsync<T>(Guid id, IList<PageInfo> blogPages)
		where T : PostBase
		{
			T t;
			PostBase postBase;
			PostBase postBase1;
			PostBase postBase2 = null;
			if (typeof(T) == typeof(PostInfo))
			{
				ICache cache = this._cache;
				if (cache != null)
				{
					postBase1 = cache.Get<PostInfo>(String.Concat("PostInfo_", id.ToString()));
				}
				else
				{
					postBase1 = null;
				}
				postBase2 = postBase1;
			}
			else if (!typeof(DynamicPost).IsAssignableFrom(typeof(T)))
			{
				ICache cache1 = this._cache;
				if (cache1 != null)
				{
					postBase = cache1.Get<PostBase>(id.ToString());
				}
				else
				{
					postBase = null;
				}
				postBase2 = postBase;
				if (postBase2 != null)
				{
					await this._factory.InitAsync<PostBase>(postBase2, App.PostTypes.GetById(postBase2.TypeId));
				}
			}
			if (postBase2 == null)
			{
				ConfiguredTaskAwaitable<T> configuredTaskAwaitable = this._repo.GetById<T>(id).ConfigureAwait(false);
				postBase2 = await configuredTaskAwaitable;
				if (postBase2 != null)
				{
					PageInfo pageInfo = blogPages.FirstOrDefault<PageInfo>((PageInfo p) => p.Id == postBase2.BlogId);
					if (pageInfo == null)
					{
						ConfiguredTaskAwaitable<PageInfo> configuredTaskAwaitable1 = this._pageService.GetByIdAsync<PageInfo>(postBase2.BlogId).ConfigureAwait(false);
						pageInfo = await configuredTaskAwaitable1;
						blogPages.Add(pageInfo);
					}
					ConfiguredTaskAwaitable configuredTaskAwaitable2 = this.OnLoadAsync(postBase2, pageInfo, false).ConfigureAwait(false);
					await configuredTaskAwaitable2;
				}
			}
			t = (postBase2 == null || !(postBase2 is T) ? default(T) : (T)postBase2);
			return t;
		}

		public Task<DynamicPost> GetBySlugAsync(string blog, string slug, Guid? siteId = null)
		{
			return this.GetBySlugAsync<DynamicPost>(blog, slug, siteId);
		}

		public async Task<T> GetBySlugAsync<T>(string blog, string slug, Guid? siteId = null)
		where T : PostBase
		{
			T t;
			ConfiguredTaskAwaitable<Guid?> configuredTaskAwaitable = this.EnsureSiteIdAsync(siteId).ConfigureAwait(false);
			siteId = await configuredTaskAwaitable;
			configuredTaskAwaitable = this._pageService.GetIdBySlugAsync(blog, siteId).ConfigureAwait(false);
			Guid? nullable = await configuredTaskAwaitable;
			if (!nullable.HasValue)
			{
				t = default(T);
			}
			else
			{
				ConfiguredTaskAwaitable<T> configuredTaskAwaitable1 = this.GetBySlugAsync<T>(nullable.Value, slug).ConfigureAwait(false);
				t = await configuredTaskAwaitable1;
			}
			return t;
		}

		public Task<DynamicPost> GetBySlugAsync(Guid blogId, string slug)
		{
			return this.GetBySlugAsync<DynamicPost>(blogId, slug);
		}

		public async Task<T> GetBySlugAsync<T>(Guid blogId, string slug)
		where T : PostBase
		{
			T t;
			Guid? nullable;
			PostBase postBase;
			PostBase postBase1;
			PostBase postBase2 = null;
			ICache cache = this._cache;
			if (cache != null)
			{
				nullable = cache.Get<Guid?>(String.Format("PostId_{0}_{1}", blogId, slug));
			}
			else
			{
				nullable = null;
			}
			Guid? nullable1 = nullable;
			if (nullable1.HasValue)
			{
				if (typeof(T) == typeof(PostInfo))
				{
					ICache cache1 = this._cache;
					if (cache1 != null)
					{
						postBase1 = cache1.Get<PostInfo>(String.Concat("PostInfo_", nullable1.ToString()));
					}
					else
					{
						postBase1 = null;
					}
					postBase2 = postBase1;
				}
				else if (!typeof(DynamicPost).IsAssignableFrom(typeof(T)))
				{
					ICache cache2 = this._cache;
					if (cache2 != null)
					{
						postBase = cache2.Get<PostBase>(nullable1.ToString());
					}
					else
					{
						postBase = null;
					}
					postBase2 = postBase;
				}
			}
			if (postBase2 == null)
			{
				ConfiguredTaskAwaitable<T> configuredTaskAwaitable = this._repo.GetBySlug<T>(blogId, slug).ConfigureAwait(false);
				postBase2 = await configuredTaskAwaitable;
				if (postBase2 != null)
				{
					ConfiguredTaskAwaitable<PageInfo> configuredTaskAwaitable1 = this._pageService.GetByIdAsync<PageInfo>(postBase2.BlogId).ConfigureAwait(false);
					PageInfo pageInfo = await configuredTaskAwaitable1;
					ConfiguredTaskAwaitable configuredTaskAwaitable2 = this.OnLoadAsync(postBase2, pageInfo, false).ConfigureAwait(false);
					await configuredTaskAwaitable2;
				}
			}
			t = (postBase2 == null || !(postBase2 is T) ? default(T) : (T)postBase2);
			return t;
		}

		public async Task<Taxonomy> GetCategoryByIdAsync(Guid id)
		{
			Taxonomy taxonomy;
			ICache cache = this._cache;
			if (cache != null)
			{
				taxonomy = cache.Get<Taxonomy>(id.ToString());
			}
			else
			{
				taxonomy = null;
			}
			Taxonomy taxonomy1 = taxonomy;
			if (taxonomy1 == null)
			{
				ConfiguredTaskAwaitable<Taxonomy> configuredTaskAwaitable = this._repo.GetCategoryById(id).ConfigureAwait(false);
				taxonomy1 = await configuredTaskAwaitable;
				if (taxonomy1 != null && this._cache != null)
				{
					this._cache.Set<Taxonomy>(taxonomy1.Id.ToString(), taxonomy1);
				}
			}
			return taxonomy1;
		}

		public async Task<Taxonomy> GetCategoryBySlugAsync(Guid blogId, string slug)
		{
			Guid value;
			Guid? nullable;
			ICache cache = this._cache;
			if (cache != null)
			{
				nullable = cache.Get<Guid?>(String.Format("Category_{0}_{1}", blogId, slug));
			}
			else
			{
				nullable = null;
			}
			Guid? nullable1 = nullable;
			Taxonomy taxonomy = null;
			if (nullable1.HasValue)
			{
				ICache cache1 = this._cache;
				value = nullable1.Value;
				taxonomy = cache1.Get<Taxonomy>(value.ToString());
			}
			if (taxonomy == null)
			{
				ConfiguredTaskAwaitable<Taxonomy> configuredTaskAwaitable = this._repo.GetCategoryBySlug(blogId, slug).ConfigureAwait(false);
				taxonomy = await configuredTaskAwaitable;
				if (taxonomy != null && this._cache != null)
				{
					ICache cache2 = this._cache;
					value = taxonomy.Id;
					cache2.Set<Taxonomy>(value.ToString(), taxonomy);
					this._cache.Set<Guid>(String.Format("Category_{0}_{1}", blogId, slug), taxonomy.Id);
				}
			}
			return taxonomy;
		}

		public Task<Comment> GetCommentByIdAsync(Guid id)
		{
			return this._repo.GetCommentById(id);
		}

		public Task<int> GetCountAsync(Guid archiveId)
		{
			return this._repo.GetCount(archiveId);
		}

		public Task<DynamicPost> GetDraftByIdAsync(Guid id)
		{
			return this.GetDraftByIdAsync<DynamicPost>(id);
		}

		public async Task<T> GetDraftByIdAsync<T>(Guid id)
		where T : PostBase
		{
			ConfiguredTaskAwaitable<T> configuredTaskAwaitable = this._repo.GetDraftById<T>(id).ConfigureAwait(false);
			T t = await configuredTaskAwaitable;
			if (t != null)
			{
				ConfiguredTaskAwaitable<PageInfo> configuredTaskAwaitable1 = this._pageService.GetByIdAsync<PageInfo>(t.BlogId).ConfigureAwait(false);
				PageInfo pageInfo = await configuredTaskAwaitable1;
				await this.OnLoadAsync(t, pageInfo, true);
			}
			return t;
		}

		public async Task<Taxonomy> GetTagByIdAsync(Guid id)
		{
			Taxonomy taxonomy;
			ICache cache = this._cache;
			if (cache != null)
			{
				taxonomy = cache.Get<Taxonomy>(id.ToString());
			}
			else
			{
				taxonomy = null;
			}
			Taxonomy taxonomy1 = taxonomy;
			if (taxonomy1 == null)
			{
				ConfiguredTaskAwaitable<Taxonomy> configuredTaskAwaitable = this._repo.GetTagById(id).ConfigureAwait(false);
				taxonomy1 = await configuredTaskAwaitable;
				if (taxonomy1 != null && this._cache != null)
				{
					this._cache.Set<Taxonomy>(taxonomy1.Id.ToString(), taxonomy1);
				}
			}
			return taxonomy1;
		}

		public async Task<Taxonomy> GetTagBySlugAsync(Guid blogId, string slug)
		{
			Guid value;
			Guid? nullable;
			ICache cache = this._cache;
			if (cache != null)
			{
				nullable = cache.Get<Guid?>(String.Format("Tag_{0}_{1}", blogId, slug));
			}
			else
			{
				nullable = null;
			}
			Guid? nullable1 = nullable;
			Taxonomy taxonomy = null;
			if (nullable1.HasValue)
			{
				ICache cache1 = this._cache;
				value = nullable1.Value;
				taxonomy = cache1.Get<Taxonomy>(value.ToString());
			}
			if (taxonomy == null)
			{
				ConfiguredTaskAwaitable<Taxonomy> configuredTaskAwaitable = this._repo.GetTagBySlug(blogId, slug).ConfigureAwait(false);
				taxonomy = await configuredTaskAwaitable;
				if (taxonomy != null && this._cache != null)
				{
					ICache cache2 = this._cache;
					value = taxonomy.Id;
					cache2.Set<Taxonomy>(value.ToString(), taxonomy);
					this._cache.Set<Guid>(String.Format("Tag_{0}_{1}", blogId, slug), taxonomy.Id);
				}
			}
			return taxonomy;
		}

		private bool IsPublished(PostBase model)
		{
			if (model == null || !model.Published.HasValue)
			{
				return false;
			}
			return model.Published.Value <= DateTime.Now;
		}

		private async Task OnLoadAsync(PostBase model, PageInfo blog, bool isDraft = false)
		{
			Guid id;
			if (model != null)
			{
				model.Permalink = String.Concat("/", blog.Slug, "/", model.Slug);
				IDynamicContent dynamicContent = model as IDynamicContent;
				if (dynamicContent == null)
				{
					await this._factory.InitAsync<PostBase>(model, App.PostTypes.GetById(model.TypeId));
				}
				else
				{
					await this._factory.InitDynamicAsync<IDynamicContent>(dynamicContent, App.PostTypes.GetById(model.TypeId));
				}
				if (model.PrimaryImage == null)
				{
					model.PrimaryImage = new ImageField();
				}
				Guid? nullable = model.PrimaryImage.Id;
				if (nullable.HasValue)
				{
					ImageField primaryImage = model.PrimaryImage;
					IMediaService mediaService = this._mediaService;
					nullable = model.PrimaryImage.Id;
					ConfiguredTaskAwaitable<Media> configuredTaskAwaitable = mediaService.GetByIdAsync(nullable.Value).ConfigureAwait(false);
					primaryImage.Media = await configuredTaskAwaitable;
					primaryImage = null;
					if (model.PrimaryImage.Media == null)
					{
						nullable = null;
						model.PrimaryImage.Id = nullable;
					}
				}
				App.Hooks.OnLoad<PostBase>(model);
				if (!isDraft && this._cache != null && !(model is DynamicPost))
				{
					if (!(model is PostInfo))
					{
						ICache cache = this._cache;
						id = model.Id;
						cache.Set<PostBase>(id.ToString(), model);
					}
					else
					{
						ICache cache1 = this._cache;
						id = model.Id;
						cache1.Set<PostBase>(String.Concat("PostInfo_", id.ToString()), model);
					}
					this._cache.Set<Guid>(String.Format("PostId_{0}_{1}", model.BlogId, model.Slug), model.Id);
				}
			}
		}

		private void RemoveFromCache(PostBase post)
		{
			if (this._cache != null)
			{
				this._cache.Remove(post.Id.ToString());
				this._cache.Remove(String.Format("PostId_{0}_{1}", post.BlogId, post.Slug));
				ICache cache = this._cache;
				Guid id = post.Id;
				cache.Remove(String.Concat("PostInfo_", id.ToString()));
			}
		}

		public Task SaveAsync<T>(T model)
		where T : PostBase
		{
			return this.SaveAsync<T>(model, false);
		}

		private async Task SaveAsync<T>(T model, bool isDraft)
		where T : PostBase
		{
			ConfiguredTaskAwaitable configuredTaskAwaitable;
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
				model.Slug = Utils.GenerateSlug(model.Slug, false);
			}
			else
			{
				model.Slug = Utils.GenerateSlug(model.Title, false);
			}
			if (String.IsNullOrWhiteSpace(model.Slug))
			{
				throw new ValidationException("The generated slug is empty as the title only contains special characters, please specify a slug to save the post.");
			}
			if (model.Category == null || String.IsNullOrWhiteSpace(model.Category.Title) && String.IsNullOrWhiteSpace(model.Category.Slug))
			{
				throw new ValidationException("The Category field is required");
			}
			App.Hooks.OnBeforeSave<PostBase>(model);
			ConfiguredTaskAwaitable<PostInfo> configuredTaskAwaitable1 = this._repo.GetById<PostInfo>(model.Id).ConfigureAwait(false);
			PostInfo postInfo = await configuredTaskAwaitable1;
			if (!(this.IsPublished(postInfo) & isDraft))
			{
				if (postInfo == null & isDraft)
				{
					model.Published = null;
				}
				else if (postInfo != null && !isDraft)
				{
					using (Config config = new Config(this._paramService))
					{
						configuredTaskAwaitable = this._repo.DeleteDraft(model.Id).ConfigureAwait(false);
						await configuredTaskAwaitable;
						configuredTaskAwaitable = this._repo.CreateRevision(model.Id, config.PostRevisions).ConfigureAwait(false);
						await configuredTaskAwaitable;
					}
					config = null;
				}
				configuredTaskAwaitable = this._repo.Save<T>(model).ConfigureAwait(false);
				await configuredTaskAwaitable;
			}
			else
			{
				configuredTaskAwaitable = this._repo.SaveDraft<T>(model).ConfigureAwait(false);
				await configuredTaskAwaitable;
			}
			App.Hooks.OnAfterSave<PostBase>(model);
			if (!isDraft && this._search != null)
			{
				await this._search.SavePostAsync(model);
			}
			this.RemoveFromCache(model);
			if (!isDraft && this._cache != null)
			{
				ConfiguredTaskAwaitable<IEnumerable<Taxonomy>> configuredTaskAwaitable2 = this._repo.GetAllCategories(model.BlogId).ConfigureAwait(false);
				foreach (Taxonomy taxonomy in await configuredTaskAwaitable2)
				{
					this._cache.Remove(taxonomy.Id.ToString());
					this._cache.Remove(String.Format("Category_{0}_{1}", model.BlogId, taxonomy.Slug));
				}
				configuredTaskAwaitable2 = this._repo.GetAllTags(model.BlogId).ConfigureAwait(false);
				foreach (Taxonomy taxonomy1 in await configuredTaskAwaitable2)
				{
					this._cache.Remove(taxonomy1.Id.ToString());
					this._cache.Remove(String.Format("Tag_{0}_{1}", model.BlogId, taxonomy1.Slug));
				}
			}
		}

		public Task SaveCommentAndVerifyAsync(Guid postId, Comment model)
		{
			return this.SaveCommentAsync(postId, model, true);
		}

		public Task SaveCommentAsync(Guid postId, Comment model)
		{
			return this.SaveCommentAsync(postId, model, false);
		}

		private async Task SaveCommentAsync(Guid postId, Comment model, bool verify)
		{
			ConfiguredTaskAwaitable<PostInfo> configuredTaskAwaitable = this.GetByIdAsync<PostInfo>(postId).ConfigureAwait(false);
			PostInfo postInfo = await configuredTaskAwaitable;
			if (postInfo == null)
			{
				throw new ArgumentException(String.Concat("Could not find post with id ", postId.ToString()));
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
			ConfiguredTaskAwaitable configuredTaskAwaitable1 = this._repo.SaveComment(postId, model).ConfigureAwait(false);
			await configuredTaskAwaitable1;
			App.Hooks.OnAfterSave<Comment>(model);
			this.RemoveFromCache(postInfo);
		}

		public Task SaveDraftAsync<T>(T model)
		where T : PostBase
		{
			return this.SaveAsync<T>(model, true);
		}
	}
}