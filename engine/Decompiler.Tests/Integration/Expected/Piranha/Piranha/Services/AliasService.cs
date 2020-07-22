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
	public class AliasService : IAliasService
	{
		private readonly IAliasRepository _repo;

		private readonly ISiteService _siteService;

		private readonly ICache _cache;

		public AliasService(IAliasRepository repo, ISiteService siteService, ICache cache = null)
		{
			base();
			this._repo = repo;
			this._siteService = siteService;
			if (App.get_CacheLevel() > 1)
			{
				this._cache = cache;
			}
			return;
		}

		public async Task DeleteAsync(Guid id)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<AliasService.u003cDeleteAsyncu003ed__9>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task DeleteAsync(Alias model)
		{
			V_0.u003cu003e4__this = this;
			V_0.model = model;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<AliasService.u003cDeleteAsyncu003ed__10>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<IEnumerable<Alias>> GetAllAsync(Guid? siteId = null)
		{
			V_0.u003cu003e4__this = this;
			V_0.siteId = siteId;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<IEnumerable<Alias>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<AliasService.u003cGetAllAsyncu003ed__4>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<Alias> GetByAliasUrlAsync(string url, Guid? siteId = null)
		{
			V_0.u003cu003e4__this = this;
			V_0.url = url;
			V_0.siteId = siteId;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<Alias>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<AliasService.u003cGetByAliasUrlAsyncu003ed__6>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<Alias> GetByIdAsync(Guid id)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<Alias>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<AliasService.u003cGetByIdAsyncu003ed__5>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<IEnumerable<Alias>> GetByRedirectUrlAsync(string url, Guid? siteId = null)
		{
			V_0.u003cu003e4__this = this;
			V_0.url = url;
			V_0.siteId = siteId;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<IEnumerable<Alias>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<AliasService.u003cGetByRedirectUrlAsyncu003ed__7>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private void OnLoad(Alias model)
		{
			if (model != null)
			{
				App.get_Hooks().OnLoad<Alias>(model);
				if (this._cache != null)
				{
					stackVariable6 = this._cache;
					V_0 = model.get_Id();
					stackVariable6.Set<Alias>(V_0.ToString(), model);
					this._cache.Set<Guid>(String.Format("AliasId_{0}_{1}", model.get_SiteId(), model.get_AliasUrl()), model.get_Id());
				}
			}
			return;
		}

		private void RemoveFromCache(Alias model)
		{
			if (this._cache != null)
			{
				stackVariable3 = this._cache;
				stackVariable3.Remove(model.get_Id().ToString());
				this._cache.Remove(String.Format("AliasId_{0}_{1}", model.get_SiteId(), model.get_AliasUrl()));
			}
			return;
		}

		public async Task SaveAsync(Alias model)
		{
			V_0.u003cu003e4__this = this;
			V_0.model = model;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<AliasService.u003cSaveAsyncu003ed__8>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}