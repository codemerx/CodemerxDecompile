using Piranha;
using Piranha.Cache;
using Piranha.Models;
using Piranha.Repositories;
using Piranha.Runtime;
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
			this._repo = repo;
			if (App.CacheLevel > CacheLevel.None)
			{
				this._cache = cache;
			}
		}

		public async Task DeleteAsync(Guid id)
		{
			ConfiguredTaskAwaitable<Param> configuredTaskAwaitable = this.GetByIdAsync(id).ConfigureAwait(false);
			Param param = await configuredTaskAwaitable;
			if (param != null)
			{
				await this.DeleteAsync(param).ConfigureAwait(false);
			}
		}

		public async Task DeleteAsync(Param model)
		{
			App.Hooks.OnBeforeDelete<Param>(model);
			ConfiguredTaskAwaitable configuredTaskAwaitable = this._repo.Delete(model.Id).ConfigureAwait(false);
			await configuredTaskAwaitable;
			App.Hooks.OnAfterDelete<Param>(model);
			this.RemoveFromCache(model);
		}

		public Task<IEnumerable<Param>> GetAllAsync()
		{
			return this._repo.GetAll();
		}

		public async Task<Param> GetByIdAsync(Guid id)
		{
			Param param;
			ICache cache = this._cache;
			if (cache != null)
			{
				param = cache.Get<Param>(id.ToString());
			}
			else
			{
				param = null;
			}
			Param param1 = param;
			if (param1 == null)
			{
				ConfiguredTaskAwaitable<Param> configuredTaskAwaitable = this._repo.GetById(id).ConfigureAwait(false);
				param1 = await configuredTaskAwaitable;
				this.OnLoad(param1);
			}
			return param1;
		}

		public async Task<Param> GetByKeyAsync(string key)
		{
			ConfiguredTaskAwaitable<Param> configuredTaskAwaitable;
			Guid? nullable;
			ICache cache = this._cache;
			if (cache != null)
			{
				nullable = cache.Get<Guid?>(String.Concat("ParamKey_", key));
			}
			else
			{
				nullable = null;
			}
			Guid? nullable1 = nullable;
			Param param = null;
			if (!nullable1.HasValue)
			{
				configuredTaskAwaitable = this._repo.GetByKey(key).ConfigureAwait(false);
				param = await configuredTaskAwaitable;
				this.OnLoad(param);
			}
			else
			{
				configuredTaskAwaitable = this.GetByIdAsync(nullable1.Value).ConfigureAwait(false);
				param = await configuredTaskAwaitable;
			}
			return param;
		}

		private void OnLoad(Param model)
		{
			if (model != null)
			{
				App.Hooks.OnLoad<Param>(model);
				if (this._cache != null)
				{
					this._cache.Set<Param>(model.Id.ToString(), model);
					this._cache.Set<Guid>(String.Concat("ParamKey_", model.Key), model.Id);
				}
			}
		}

		private void RemoveFromCache(Param model)
		{
			if (this._cache != null)
			{
				this._cache.Remove(model.Id.ToString());
				this._cache.Remove(String.Concat("ParamKey_", model.Key));
			}
		}

		public async Task SaveAsync(Param model)
		{
			if (model.Id == Guid.Empty)
			{
				model.Id = Guid.NewGuid();
			}
			Validator.ValidateObject(model, new ValidationContext(model), true);
			ConfiguredTaskAwaitable<Param> configuredTaskAwaitable = this._repo.GetByKey(model.Key).ConfigureAwait(false);
			Param param = await configuredTaskAwaitable;
			if (param != null && param.Id != model.Id)
			{
				throw new ValidationException("The Key field must be unique");
			}
			App.Hooks.OnBeforeSave<Param>(model);
			await this._repo.Save(model).ConfigureAwait(false);
			App.Hooks.OnAfterSave<Param>(model);
			this.RemoveFromCache(model);
		}
	}
}