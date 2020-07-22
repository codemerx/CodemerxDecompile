using Piranha;
using Piranha.Extend.Fields;
using Piranha.Models;
using Piranha.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
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
			base();
			this._repo = repo;
			this._factory = factory;
			this._siteService = siteService;
			this._pageService = pageService;
			this._paramService = paramService;
			this._mediaService = mediaService;
			this._search = search;
			if (App.get_CacheLevel() > 2)
			{
				this._cache = cache;
			}
			return;
		}

		public async Task<T> CreateAsync<T>(string typeId = null)
		where T : PostBase
		{
			V_0.u003cu003e4__this = this;
			V_0.typeId = typeId;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<T>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PostService.u003cCreateAsyncu003ed__9<T>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task DeleteAsync(Guid id)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PostService.u003cDeleteAsyncu003ed__42>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task DeleteAsync<T>(T model)
		where T : PostBase
		{
			V_0.u003cu003e4__this = this;
			V_0.model = model;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PostService.u003cDeleteAsyncu003ed__43<T>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task DeleteCommentAsync(Guid id)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PostService.u003cDeleteCommentAsyncu003ed__44>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task DeleteCommentAsync(Comment model)
		{
			V_0.u003cu003e4__this = this;
			V_0.model = model;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PostService.u003cDeleteCommentAsyncu003ed__45>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private async Task<Guid?> EnsureSiteIdAsync(Guid? siteId)
		{
			V_0.u003cu003e4__this = this;
			V_0.siteId = siteId;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<Guid?>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PostService.u003cEnsureSiteIdAsyncu003ed__47>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public Task<IEnumerable<DynamicPost>> GetAllAsync(Guid blogId, int? index = null, int? pageSize = null)
		{
			return this.GetAllAsync<DynamicPost>(blogId, index, pageSize);
		}

		public async Task<IEnumerable<T>> GetAllAsync<T>(Guid blogId, int? index = null, int? pageSize = null)
		where T : PostBase
		{
			V_0.u003cu003e4__this = this;
			V_0.blogId = blogId;
			V_0.index = index;
			V_0.pageSize = pageSize;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<IEnumerable<T>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PostService.u003cGetAllAsyncu003ed__11<T>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public Task<IEnumerable<DynamicPost>> GetAllAsync(string slug, Guid? siteId = null)
		{
			return this.GetAllAsync<DynamicPost>(slug, siteId);
		}

		public async Task<IEnumerable<T>> GetAllAsync<T>(string slug, Guid? siteId = null)
		where T : PostBase
		{
			V_0.u003cu003e4__this = this;
			V_0.slug = slug;
			V_0.siteId = siteId;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<IEnumerable<T>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PostService.u003cGetAllAsyncu003ed__15<T>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public Task<IEnumerable<DynamicPost>> GetAllBySiteIdAsync(Guid? siteId = null)
		{
			return this.GetAllBySiteIdAsync<DynamicPost>(siteId);
		}

		public async Task<IEnumerable<T>> GetAllBySiteIdAsync<T>(Guid? siteId = null)
		where T : PostBase
		{
			V_0.u003cu003e4__this = this;
			V_0.siteId = siteId;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<IEnumerable<T>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PostService.u003cGetAllBySiteIdAsyncu003ed__13<T>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
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
			V_0.u003cu003e4__this = this;
			V_0.postId = postId;
			V_0.onlyApproved = onlyApproved;
			V_0.onlyPending = onlyPending;
			V_0.page = page;
			V_0.pageSize = pageSize;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<IEnumerable<Comment>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PostService.u003cGetAllCommentsAsyncu003ed__39>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
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
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.blogPages = blogPages;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<T>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PostService.u003cGetByIdAsyncu003ed__46<T>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public Task<DynamicPost> GetBySlugAsync(string blog, string slug, Guid? siteId = null)
		{
			return this.GetBySlugAsync<DynamicPost>(blog, slug, siteId);
		}

		public async Task<T> GetBySlugAsync<T>(string blog, string slug, Guid? siteId = null)
		where T : PostBase
		{
			V_0.u003cu003e4__this = this;
			V_0.blog = blog;
			V_0.slug = slug;
			V_0.siteId = siteId;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<T>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PostService.u003cGetBySlugAsyncu003ed__25<T>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public Task<DynamicPost> GetBySlugAsync(Guid blogId, string slug)
		{
			return this.GetBySlugAsync<DynamicPost>(blogId, slug);
		}

		public async Task<T> GetBySlugAsync<T>(Guid blogId, string slug)
		where T : PostBase
		{
			V_0.u003cu003e4__this = this;
			V_0.blogId = blogId;
			V_0.slug = slug;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<T>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PostService.u003cGetBySlugAsyncu003ed__27<T>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<Taxonomy> GetCategoryByIdAsync(Guid id)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<Taxonomy>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PostService.u003cGetCategoryByIdAsyncu003ed__31>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<Taxonomy> GetCategoryBySlugAsync(Guid blogId, string slug)
		{
			V_0.u003cu003e4__this = this;
			V_0.blogId = blogId;
			V_0.slug = slug;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<Taxonomy>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PostService.u003cGetCategoryBySlugAsyncu003ed__30>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
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
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<T>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PostService.u003cGetDraftByIdAsyncu003ed__29<T>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<Taxonomy> GetTagByIdAsync(Guid id)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<Taxonomy>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PostService.u003cGetTagByIdAsyncu003ed__33>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<Taxonomy> GetTagBySlugAsync(Guid blogId, string slug)
		{
			V_0.u003cu003e4__this = this;
			V_0.blogId = blogId;
			V_0.slug = slug;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<Taxonomy>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PostService.u003cGetTagBySlugAsyncu003ed__32>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private bool IsPublished(PostBase model)
		{
			if (model == null || !model.get_Published().get_HasValue())
			{
				return false;
			}
			V_0 = model.get_Published();
			return DateTime.op_LessThanOrEqual(V_0.get_Value(), DateTime.get_Now());
		}

		private async Task OnLoadAsync(PostBase model, PageInfo blog, bool isDraft = false)
		{
			V_0.u003cu003e4__this = this;
			V_0.model = model;
			V_0.blog = blog;
			V_0.isDraft = isDraft;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PostService.u003cOnLoadAsyncu003ed__48>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private void RemoveFromCache(PostBase post)
		{
			if (this._cache != null)
			{
				stackVariable3 = this._cache;
				stackVariable3.Remove(post.get_Id().ToString());
				this._cache.Remove(String.Format("PostId_{0}_{1}", post.get_BlogId(), post.get_Slug()));
				stackVariable18 = this._cache;
				V_0 = post.get_Id();
				stackVariable18.Remove(String.Concat("PostInfo_", V_0.ToString()));
			}
			return;
		}

		public Task SaveAsync<T>(T model)
		where T : PostBase
		{
			return this.SaveAsync<T>(model, false);
		}

		private async Task SaveAsync<T>(T model, bool isDraft)
		where T : PostBase
		{
			V_0.u003cu003e4__this = this;
			V_0.model = model;
			V_0.isDraft = isDraft;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PostService.u003cSaveAsyncu003ed__41<T>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
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
			V_0.u003cu003e4__this = this;
			V_0.postId = postId;
			V_0.model = model;
			V_0.verify = verify;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PostService.u003cSaveCommentAsyncu003ed__40>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public Task SaveDraftAsync<T>(T model)
		where T : PostBase
		{
			return this.SaveAsync<T>(model, true);
		}
	}
}