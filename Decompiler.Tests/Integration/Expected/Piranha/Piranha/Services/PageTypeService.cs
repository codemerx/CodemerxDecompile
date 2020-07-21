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
	public class PageTypeService : IPageTypeService
	{
		private readonly IPageTypeRepository _repo;

		private readonly ICache _cache;

		public PageTypeService(IPageTypeRepository repo, ICache cache)
		{
			base();
			this._repo = repo;
			if (App.get_CacheLevel() > 1)
			{
				this._cache = cache;
			}
			return;
		}

		public async Task DeleteAsync(string id)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PageTypeService.u003cDeleteAsyncu003ed__6>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task DeleteAsync(PageType model)
		{
			V_0.u003cu003e4__this = this;
			V_0.model = model;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PageTypeService.u003cDeleteAsyncu003ed__7>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public Task<IEnumerable<PageType>> GetAllAsync()
		{
			return this.GetTypes();
		}

		public async Task<PageType> GetByIdAsync(string id)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<PageType>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PageTypeService.u003cGetByIdAsyncu003ed__4>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private async Task<IEnumerable<PageType>> GetTypes()
		{
			V_0.u003cu003e4__this = this;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<IEnumerable<PageType>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PageTypeService.u003cGetTypesu003ed__8>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public async Task SaveAsync(PageType model)
		{
			V_0.u003cu003e4__this = this;
			V_0.model = model;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<PageTypeService.u003cSaveAsyncu003ed__5>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}