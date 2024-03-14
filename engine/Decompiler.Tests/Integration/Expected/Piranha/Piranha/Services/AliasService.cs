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
	public class AliasService : IAliasService
	{
		private readonly IAliasRepository _repo;

		private readonly ISiteService _siteService;

		private readonly ICache _cache;

		public AliasService(IAliasRepository repo, ISiteService siteService, ICache cache = null)
		{
			this._repo = repo;
			this._siteService = siteService;
			if (App.CacheLevel > CacheLevel.Minimal)
			{
				this._cache = cache;
			}
		}

		public async Task DeleteAsync(Guid id)
		{
			ConfiguredTaskAwaitable<Alias> configuredTaskAwaitable = this.GetByIdAsync(id).ConfigureAwait(false);
			Alias alia = await configuredTaskAwaitable;
			if (alia != null)
			{
				await this.DeleteAsync(alia).ConfigureAwait(false);
			}
		}

		public async Task DeleteAsync(Alias model)
		{
			App.Hooks.OnBeforeDelete<Alias>(model);
			ConfiguredTaskAwaitable configuredTaskAwaitable = this._repo.Delete(model.Id).ConfigureAwait(false);
			await configuredTaskAwaitable;
			App.Hooks.OnAfterDelete<Alias>(model);
			this.RemoveFromCache(model);
		}

		public async Task<IEnumerable<Alias>> GetAllAsync(Guid? siteId = null)
		{
			IEnumerable<Alias> aliases;
			if (!siteId.HasValue)
			{
				ConfiguredTaskAwaitable<Site> configuredTaskAwaitable = this._siteService.GetDefaultAsync().ConfigureAwait(false);
				Site site = await configuredTaskAwaitable;
				if (site != null)
				{
					siteId = new Guid?(site.Id);
				}
			}
			if (!siteId.HasValue)
			{
				aliases = null;
			}
			else
			{
				ConfiguredTaskAwaitable<IEnumerable<Alias>> configuredTaskAwaitable1 = this._repo.GetAll(siteId.Value).ConfigureAwait(false);
				aliases = await configuredTaskAwaitable1;
			}
			return aliases;
		}

		public async Task<Alias> GetByAliasUrlAsync(string url, Guid? siteId = null)
		{
			ConfiguredTaskAwaitable<Alias> configuredTaskAwaitable;
			Guid? nullable;
			if (!siteId.HasValue)
			{
				ConfiguredTaskAwaitable<Site> configuredTaskAwaitable1 = this._siteService.GetDefaultAsync().ConfigureAwait(false);
				Site site = await configuredTaskAwaitable1;
				if (site != null)
				{
					siteId = new Guid?(site.Id);
				}
			}
			ICache cache = this._cache;
			if (cache != null)
			{
				nullable = cache.Get<Guid?>(String.Format("AliasId_{0}_{1}", siteId, url));
			}
			else
			{
				nullable = null;
			}
			Guid? nullable1 = nullable;
			Alias alia = null;
			if (!nullable1.HasValue)
			{
				configuredTaskAwaitable = this._repo.GetByAliasUrl(url, siteId.Value).ConfigureAwait(false);
				alia = await configuredTaskAwaitable;
				if (alia == null)
				{
					ICache cache1 = this._cache;
					if (cache1 != null)
					{
						cache1.Set<Guid>(String.Format("AliasId_{0}_{1}", siteId, url), Guid.Empty);
					}
					else
					{
					}
				}
				else
				{
					this.OnLoad(alia);
				}
			}
			else if (nullable1.Value != Guid.Empty)
			{
				configuredTaskAwaitable = this.GetByIdAsync(nullable1.Value).ConfigureAwait(false);
				alia = await configuredTaskAwaitable;
			}
			return alia;
		}

		public async Task<Alias> GetByIdAsync(Guid id)
		{
			Alias alia;
			ICache cache = this._cache;
			if (cache != null)
			{
				alia = cache.Get<Alias>(id.ToString());
			}
			else
			{
				alia = null;
			}
			Alias alia1 = alia;
			if (alia1 == null)
			{
				ConfiguredTaskAwaitable<Alias> configuredTaskAwaitable = this._repo.GetById(id).ConfigureAwait(false);
				alia1 = await configuredTaskAwaitable;
				this.OnLoad(alia1);
			}
			return alia1;
		}

		public async Task<IEnumerable<Alias>> GetByRedirectUrlAsync(string url, Guid? siteId = null)
		{
			if (!siteId.HasValue)
			{
				ConfiguredTaskAwaitable<Site> configuredTaskAwaitable = this._siteService.GetDefaultAsync().ConfigureAwait(false);
				Site site = await configuredTaskAwaitable;
				if (site != null)
				{
					siteId = new Guid?(site.Id);
				}
			}
			ConfiguredTaskAwaitable<IEnumerable<Alias>> configuredTaskAwaitable1 = this._repo.GetByRedirectUrl(url, siteId.Value).ConfigureAwait(false);
			return await configuredTaskAwaitable1;
		}

		private void OnLoad(Alias model)
		{
			if (model != null)
			{
				App.Hooks.OnLoad<Alias>(model);
				if (this._cache != null)
				{
					this._cache.Set<Alias>(model.Id.ToString(), model);
					this._cache.Set<Guid>(String.Format("AliasId_{0}_{1}", model.SiteId, model.AliasUrl), model.Id);
				}
			}
		}

		private void RemoveFromCache(Alias model)
		{
			if (this._cache != null)
			{
				this._cache.Remove(model.Id.ToString());
				this._cache.Remove(String.Format("AliasId_{0}_{1}", model.SiteId, model.AliasUrl));
			}
		}

		public async Task SaveAsync(Alias model)
		{
			if (model.Id == Guid.Empty)
			{
				model.Id = Guid.NewGuid();
			}
			Validator.ValidateObject(model, new ValidationContext(model), true);
			if (!model.AliasUrl.StartsWith("/"))
			{
				model.AliasUrl = String.Concat("/", model.AliasUrl);
			}
			if (!model.RedirectUrl.StartsWith("/") && !model.RedirectUrl.StartsWith("http://") && !model.RedirectUrl.StartsWith("https://"))
			{
				model.RedirectUrl = String.Concat("/", model.RedirectUrl);
			}
			ConfiguredTaskAwaitable<Alias> configuredTaskAwaitable = this._repo.GetByAliasUrl(model.AliasUrl, model.SiteId).ConfigureAwait(false);
			Alias alia = await configuredTaskAwaitable;
			if (alia != null && alia.Id != model.Id)
			{
				throw new ValidationException("The AliasUrl field must be unique");
			}
			App.Hooks.OnBeforeSave<Alias>(model);
			await this._repo.Save(model).ConfigureAwait(false);
			App.Hooks.OnAfterSave<Alias>(model);
			this.RemoveFromCache(model);
		}
	}
}