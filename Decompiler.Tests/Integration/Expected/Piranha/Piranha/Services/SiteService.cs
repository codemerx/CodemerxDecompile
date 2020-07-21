using Piranha;
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
	public class SiteService : ISiteService
	{
		private readonly ISiteRepository _repo;

		private readonly IContentFactory _factory;

		private readonly ICache _cache;

		private const string SITE_MAPPINGS = "Site_Mappings";

		public SiteService(ISiteRepository repo, IContentFactory factory, ICache cache = null)
		{
			base();
			this._repo = repo;
			this._factory = factory;
			if (App.get_CacheLevel() > 0)
			{
				this._cache = cache;
			}
			return;
		}

		public Task<T> CreateContentAsync<T>(string typeId = null)
		where T : SiteContentBase
		{
			if (String.IsNullOrEmpty(typeId))
			{
				typeId = Type.GetTypeFromHandle(// 
				// Current member / type: System.Threading.Tasks.Task`1<T> Piranha.Services.SiteService::CreateContentAsync(System.String)
				// Exception in: System.Threading.Tasks.Task<T> CreateContentAsync(System.String)
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com


		public async Task DeleteAsync(Guid id)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<SiteService.u003cDeleteAsyncu003ed__18>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task DeleteAsync(Site model)
		{
			V_0.u003cu003e4__this = this;
			V_0.model = model;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<SiteService.u003cDeleteAsyncu003ed__19>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<IEnumerable<Site>> GetAllAsync()
		{
			V_0.u003cu003e4__this = this;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<IEnumerable<Site>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<SiteService.u003cGetAllAsyncu003ed__6>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<Site> GetByHostnameAsync(string hostname)
		{
			V_0.u003cu003e4__this = this;
			V_0.hostname = hostname;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<Site>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<SiteService.u003cGetByHostnameAsyncu003ed__9>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<Site> GetByIdAsync(Guid id)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<Site>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<SiteService.u003cGetByIdAsyncu003ed__7>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<Site> GetByInternalIdAsync(string internalId)
		{
			V_0.u003cu003e4__this = this;
			V_0.internalId = internalId;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<Site>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<SiteService.u003cGetByInternalIdAsyncu003ed__8>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public Task<DynamicSiteContent> GetContentByIdAsync(Guid id)
		{
			return this.GetContentByIdAsync<DynamicSiteContent>(id);
		}

		public async Task<T> GetContentByIdAsync<T>(Guid id)
		where T : SiteContent<T>
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<T>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<SiteService.u003cGetContentByIdAsyncu003ed__12<T>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<Site> GetDefaultAsync()
		{
			V_0.u003cu003e4__this = this;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<Site>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<SiteService.u003cGetDefaultAsyncu003ed__10>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<Sitemap> GetSitemapAsync(Guid? id = null, bool onlyPublished = true)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.onlyPublished = onlyPublished;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<Sitemap>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<SiteService.u003cGetSitemapAsyncu003ed__13>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task InvalidateSitemapAsync(Guid id, bool updateLastModified = true)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.updateLastModified = updateLastModified;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<SiteService.u003cInvalidateSitemapAsyncu003ed__17>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private void OnLoad(Site model)
		{
			if (model != null)
			{
				App.get_Hooks().OnLoad<Site>(model);
				if (this._cache != null)
				{
					stackVariable6 = this._cache;
					V_0 = model.get_Id();
					stackVariable6.Set<Site>(V_0.ToString(), model);
					this._cache.Set<Guid>(String.Concat("SiteId_", model.get_InternalId()), model.get_Id());
					if (model.get_IsDefault())
					{
						this._cache.Set<Site>(String.Format("Site_{0}", Guid.Empty), model);
					}
				}
			}
			return;
		}

		private async Task OnLoadContentAsync(SiteContentBase model)
		{
			V_0.u003cu003e4__this = this;
			V_0.model = model;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<SiteService.u003cOnLoadContentAsyncu003ed__22>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private void RemoveContentFromCache<T>(T model)
		where T : SiteContentBase
		{
			stackVariable1 = this._cache;
			if (stackVariable1 == null)
			{
				dummyVar0 = stackVariable1;
				return;
			}
			stackVariable1.Remove(String.Format("SiteContent_{0}", model.get_Id()));
			return;
		}

		private void RemoveFromCache(Site model)
		{
			if (this._cache != null)
			{
				stackVariable3 = this._cache;
				stackVariable3.Remove(model.get_Id().ToString());
				this._cache.Remove(String.Concat("SiteId_", model.get_InternalId()));
				if (model.get_IsDefault())
				{
					this._cache.Remove(String.Format("Site_{0}", Guid.Empty));
				}
				this._cache.Remove("Site_Mappings");
			}
			return;
		}

		public async Task RemoveSitemapFromCacheAsync(Guid id)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<SiteService.u003cRemoveSitemapFromCacheAsyncu003ed__20>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task SaveAsync(Site model)
		{
			V_0.u003cu003e4__this = this;
			V_0.model = model;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<SiteService.u003cSaveAsyncu003ed__14>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task SaveContentAsync<T>(Guid siteId, T model)
		where T : SiteContent<T>
		{
			V_0.u003cu003e4__this = this;
			V_0.siteId = siteId;
			V_0.model = model;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<SiteService.u003cSaveContentAsyncu003ed__15<T>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		[Serializable]
		public class SiteMapping
		{
			public string Hostnames
			{
				get;
				set;
			}

			public Guid Id
			{
				get;
				set;
			}

			public SiteMapping()
			{
				base();
				return;
			}
		}
	}
}