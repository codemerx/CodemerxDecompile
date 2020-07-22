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
	public class ParamService : IParamService
	{
		private readonly IParamRepository _repo;

		private readonly ICache _cache;

		public ParamService(IParamRepository repo, ICache cache = null)
		{
			base();
			this._repo = repo;
			if (App.get_CacheLevel() > 0)
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
			V_0.u003cu003et__builder.Start<ParamService.u003cDeleteAsyncu003ed__7>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task DeleteAsync(Param model)
		{
			V_0.u003cu003e4__this = this;
			V_0.model = model;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ParamService.u003cDeleteAsyncu003ed__8>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public Task<IEnumerable<Param>> GetAllAsync()
		{
			return this._repo.GetAll();
		}

		public async Task<Param> GetByIdAsync(Guid id)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<Param>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ParamService.u003cGetByIdAsyncu003ed__4>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task<Param> GetByKeyAsync(string key)
		{
			V_0.u003cu003e4__this = this;
			V_0.key = key;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<Param>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ParamService.u003cGetByKeyAsyncu003ed__5>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private void OnLoad(Param model)
		{
			if (model != null)
			{
				App.get_Hooks().OnLoad<Param>(model);
				if (this._cache != null)
				{
					stackVariable6 = this._cache;
					V_0 = model.get_Id();
					stackVariable6.Set<Param>(V_0.ToString(), model);
					this._cache.Set<Guid>(String.Concat("ParamKey_", model.get_Key()), model.get_Id());
				}
			}
			return;
		}

		private void RemoveFromCache(Param model)
		{
			if (this._cache != null)
			{
				stackVariable3 = this._cache;
				stackVariable3.Remove(model.get_Id().ToString());
				this._cache.Remove(String.Concat("ParamKey_", model.get_Key()));
			}
			return;
		}

		public async Task SaveAsync(Param model)
		{
			V_0.u003cu003e4__this = this;
			V_0.model = model;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ParamService.u003cSaveAsyncu003ed__6>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}