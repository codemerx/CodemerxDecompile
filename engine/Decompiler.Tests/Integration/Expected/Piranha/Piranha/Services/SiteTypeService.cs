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
	public class SiteTypeService : ISiteTypeService
	{
		private readonly ISiteTypeRepository _repo;

		private readonly ICache _cache;

		public SiteTypeService(ISiteTypeRepository repo, ICache cache)
		{
			this._repo = repo;
			if (App.CacheLevel > CacheLevel.Minimal)
			{
				this._cache = cache;
			}
		}

		public async Task DeleteAsync(string id)
		{
			ConfiguredTaskAwaitable<SiteType> configuredTaskAwaitable = this._repo.GetById(id).ConfigureAwait(false);
			SiteType siteType = await configuredTaskAwaitable;
			if (siteType != null)
			{
				await this.DeleteAsync(siteType).ConfigureAwait(false);
			}
		}

		public async Task DeleteAsync(SiteType model)
		{
			App.Hooks.OnBeforeDelete<SiteType>(model);
			ConfiguredTaskAwaitable configuredTaskAwaitable = this._repo.Delete(model.Id).ConfigureAwait(false);
			await configuredTaskAwaitable;
			App.Hooks.OnAfterDelete<SiteType>(model);
			ICache cache = this._cache;
			if (cache != null)
			{
				cache.Remove("Piranha_SiteTypes");
			}
			else
			{
			}
		}

		public Task<IEnumerable<SiteType>> GetAllAsync()
		{
			return this.GetTypes();
		}

		public async Task<SiteType> GetByIdAsync(string id)
		{
			ConfiguredTaskAwaitable<IEnumerable<SiteType>> configuredTaskAwaitable = this.GetTypes().ConfigureAwait(false);
			SiteType siteType = await configuredTaskAwaitable.FirstOrDefault<SiteType>((SiteType t) => t.Id == id);
			return siteType;
		}

		private async Task<IEnumerable<SiteType>> GetTypes()
		{
			IEnumerable<SiteType> siteTypes;
			ICache cache = this._cache;
			if (cache != null)
			{
				siteTypes = cache.Get<IEnumerable<SiteType>>("Piranha_SiteTypes");
			}
			else
			{
				siteTypes = null;
			}
			IEnumerable<SiteType> siteTypes1 = siteTypes;
			if (siteTypes1 == null)
			{
				ConfiguredTaskAwaitable<IEnumerable<SiteType>> configuredTaskAwaitable = this._repo.GetAll().ConfigureAwait(false);
				siteTypes1 = await configuredTaskAwaitable;
				ICache cache1 = this._cache;
				if (cache1 != null)
				{
					cache1.Set<IEnumerable<SiteType>>("Piranha_SiteTypes", siteTypes1);
				}
				else
				{
				}
			}
			return siteTypes1;
		}

		public async Task SaveAsync(SiteType model)
		{
			Validator.ValidateObject(model, new ValidationContext(model), true);
			App.Hooks.OnBeforeSave<SiteType>(model);
			await this._repo.Save(model).ConfigureAwait(false);
			App.Hooks.OnAfterSave<SiteType>(model);
			ICache cache = this._cache;
			if (cache != null)
			{
				cache.Remove("Piranha_SiteTypes");
			}
			else
			{
			}
		}
	}
}