using Piranha;
using Piranha.Cache;
using Piranha.Models;
using Piranha.Repositories;
using Piranha.Runtime;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
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
			this._repo = repo;
			if (App.CacheLevel > CacheLevel.Minimal)
			{
				this._cache = cache;
			}
		}

		public async Task DeleteAsync(string id)
		{
			ConfiguredTaskAwaitable<PageType> configuredTaskAwaitable = this._repo.GetById(id).ConfigureAwait(false);
			PageType pageType = await configuredTaskAwaitable;
			if (pageType != null)
			{
				await this.DeleteAsync(pageType).ConfigureAwait(false);
			}
		}

		public async Task DeleteAsync(PageType model)
		{
			App.Hooks.OnBeforeDelete<PageType>(model);
			ConfiguredTaskAwaitable configuredTaskAwaitable = this._repo.Delete(model.Id).ConfigureAwait(false);
			await configuredTaskAwaitable;
			App.Hooks.OnAfterDelete<PageType>(model);
			ICache cache = this._cache;
			if (cache != null)
			{
				cache.Remove("Piranha_PageTypes");
			}
			else
			{
			}
		}

		public Task<IEnumerable<PageType>> GetAllAsync()
		{
			return this.GetTypes();
		}

		public async Task<PageType> GetByIdAsync(string id)
		{
			ConfiguredTaskAwaitable<IEnumerable<PageType>> configuredTaskAwaitable = this.GetTypes().ConfigureAwait(false);
			PageType pageType = await configuredTaskAwaitable.FirstOrDefault<PageType>((PageType t) => t.Id == id);
			return pageType;
		}

		private async Task<IEnumerable<PageType>> GetTypes()
		{
			IEnumerable<PageType> pageTypes;
			ICache cache = this._cache;
			if (cache != null)
			{
				pageTypes = cache.Get<IEnumerable<PageType>>("Piranha_PageTypes");
			}
			else
			{
				pageTypes = null;
			}
			IEnumerable<PageType> pageTypes1 = pageTypes;
			if (pageTypes1 == null)
			{
				ConfiguredTaskAwaitable<IEnumerable<PageType>> configuredTaskAwaitable = this._repo.GetAll().ConfigureAwait(false);
				pageTypes1 = await configuredTaskAwaitable;
				ICache cache1 = this._cache;
				if (cache1 != null)
				{
					cache1.Set<IEnumerable<PageType>>("Piranha_PageTypes", pageTypes1);
				}
				else
				{
				}
			}
			return pageTypes1;
		}

		public async Task SaveAsync(PageType model)
		{
			Validator.ValidateObject(model, new ValidationContext(model), true);
			App.Hooks.OnBeforeSave<PageType>(model);
			await this._repo.Save(model).ConfigureAwait(false);
			App.Hooks.OnAfterSave<PageType>(model);
			ICache cache = this._cache;
			if (cache != null)
			{
				cache.Remove("Piranha_PageTypes");
			}
			else
			{
			}
		}
	}
}