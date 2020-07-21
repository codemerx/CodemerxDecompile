using Piranha;
using Piranha.Extend;
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
			base();
			this._repo = repo;
			this._factory = factory;
			this._siteService = siteService;
			this._paramService = paramService;
			this._mediaService = mediaService;
			this._search = search;
			if (App.get_CacheLevel() > 2)
			{
				this._cache = cache;
			}
			return;
		}

		public async Task<T> CopyAsync<T>(T originalPage)
		where T : PageBase
		{
			V_0.u003cu003e4__this = this;
			V_0.originalPage = originalPage;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<T>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PageService.u003cCopyAsyncu003ed__9<T>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<T> CreateAsync<T>(string typeId = null)
		where T : PageBase
		{
			V_0.u003cu003e4__this = this;
			V_0.typeId = typeId;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<T>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PageService.u003cCreateAsyncu003ed__8<T>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task DeleteAsync(Guid id)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PageService.u003cDeleteAsyncu003ed__36>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task DeleteAsync<T>(T model)
		where T : PageBase
		{
			V_0.u003cu003e4__this = this;
			V_0.model = model;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PageService.u003cDeleteAsyncu003ed__37<T>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task DeleteCommentAsync(Guid id)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PageService.u003cDeleteCommentAsyncu003ed__38>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task DeleteCommentAsync(Comment model)
		{
			V_0.u003cu003e4__this = this;
			V_0.model = model;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PageService.u003cDeleteCommentAsyncu003ed__39>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task DetachAsync<T>(T model)
		where T : PageBase
		{
			V_0.u003cu003e4__this = this;
			V_0.model = model;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PageService.u003cDetachAsyncu003ed__10<T>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private async Task<Guid?> EnsureSiteIdAsync(Guid? siteId)
		{
			V_0.u003cu003e4__this = this;
			V_0.siteId = siteId;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<Guid?>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PageService.u003cEnsureSiteIdAsyncu003ed__41>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public Task<IEnumerable<DynamicPage>> GetAllAsync(Guid? siteId = null)
		{
			return this.GetAllAsync<DynamicPage>(siteId);
		}

		public async Task<IEnumerable<T>> GetAllAsync<T>(Guid? siteId = null)
		where T : PageBase
		{
			V_0.u003cu003e4__this = this;
			V_0.siteId = siteId;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<IEnumerable<T>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PageService.u003cGetAllAsyncu003ed__12<T>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public Task<IEnumerable<DynamicPage>> GetAllBlogsAsync(Guid? siteId = null)
		{
			return this.GetAllBlogsAsync<DynamicPage>(siteId);
		}

		public async Task<IEnumerable<T>> GetAllBlogsAsync<T>(Guid? siteId = null)
		where T : PageBase
		{
			V_0.u003cu003e4__this = this;
			V_0.siteId = siteId;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<IEnumerable<T>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PageService.u003cGetAllBlogsAsyncu003ed__14<T>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public Task<IEnumerable<Comment>> GetAllCommentsAsync(Guid? pageId = null, bool onlyApproved = true, int? page = null, int? pageSize = null)
		{
			return this.GetAllCommentsAsync(pageId, onlyApproved, false, page, pageSize);
		}

		private async Task<IEnumerable<Comment>> GetAllCommentsAsync(Guid? pageId = null, bool onlyApproved = true, bool onlyPending = false, int? page = null, int? pageSize = null)
		{
			V_0.u003cu003e4__this = this;
			V_0.pageId = pageId;
			V_0.onlyApproved = onlyApproved;
			V_0.onlyPending = onlyPending;
			V_0.page = page;
			V_0.pageSize = pageSize;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<IEnumerable<Comment>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PageService.u003cGetAllCommentsAsyncu003ed__31>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<IEnumerable<Guid>> GetAllDraftsAsync(Guid? siteId = null)
		{
			V_0.u003cu003e4__this = this;
			V_0.siteId = siteId;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<IEnumerable<Guid>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PageService.u003cGetAllDraftsAsyncu003ed__15>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
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
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<T>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PageService.u003cGetByIdAsyncu003ed__21<T>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public Task<DynamicPage> GetBySlugAsync(string slug, Guid? siteId = null)
		{
			return this.GetBySlugAsync<DynamicPage>(slug, siteId);
		}

		public async Task<T> GetBySlugAsync<T>(string slug, Guid? siteId = null)
		where T : PageBase
		{
			V_0.u003cu003e4__this = this;
			V_0.slug = slug;
			V_0.siteId = siteId;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<T>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PageService.u003cGetBySlugAsyncu003ed__23<T>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
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
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<T>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PageService.u003cGetDraftByIdAsyncu003ed__26<T>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<Guid?> GetIdBySlugAsync(string slug, Guid? siteId = null)
		{
			V_0.u003cu003e4__this = this;
			V_0.slug = slug;
			V_0.siteId = siteId;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<Guid?>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PageService.u003cGetIdBySlugAsyncu003ed__24>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public Task<DynamicPage> GetStartpageAsync(Guid? siteId = null)
		{
			return this.GetStartpageAsync<DynamicPage>(siteId);
		}

		public async Task<T> GetStartpageAsync<T>(Guid? siteId = null)
		where T : PageBase
		{
			V_0.u003cu003e4__this = this;
			V_0.siteId = siteId;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<T>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PageService.u003cGetStartpageAsyncu003ed__19<T>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private bool IsPublished(PageBase model)
		{
			if (model == null || !model.get_Published().get_HasValue())
			{
				return false;
			}
			V_0 = model.get_Published();
			return DateTime.op_LessThanOrEqual(V_0.get_Value(), DateTime.get_Now());
		}

		private async Task<T> MapOriginalAsync<T>(T model)
		where T : PageBase
		{
			V_0.u003cu003e4__this = this;
			V_0.model = model;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<T>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PageService.u003cMapOriginalAsyncu003ed__40<T>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task MoveAsync<T>(T model, Guid? parentId, int sortOrder)
		where T : PageBase
		{
			V_0.u003cu003e4__this = this;
			V_0.model = model;
			V_0.parentId = parentId;
			V_0.sortOrder = sortOrder;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PageService.u003cMoveAsyncu003ed__27<T>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private async Task OnLoadAsync(PageBase model, bool isDraft = false)
		{
			V_0.u003cu003e4__this = this;
			V_0.model = model;
			V_0.isDraft = isDraft;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PageService.u003cOnLoadAsyncu003ed__42>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private async Task RemoveFromCache(PageBase model)
		{
			V_0.u003cu003e4__this = this;
			V_0.model = model;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PageService.u003cRemoveFromCacheu003ed__43>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public Task SaveAsync<T>(T model)
		where T : PageBase
		{
			return this.SaveAsync<T>(model, false);
		}

		private async Task SaveAsync<T>(T model, bool isDraft)
		where T : PageBase
		{
			V_0.u003cu003e4__this = this;
			V_0.model = model;
			V_0.isDraft = isDraft;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PageService.u003cSaveAsyncu003ed__32<T>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
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
			V_0.u003cu003e4__this = this;
			V_0.pageId = pageId;
			V_0.model = model;
			V_0.verify = verify;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PageService.u003cSaveCommentAsyncu003ed__35>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public Task SaveDraftAsync<T>(T model)
		where T : PageBase
		{
			return this.SaveAsync<T>(model, true);
		}
	}
}