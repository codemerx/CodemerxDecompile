using Piranha;
using Piranha.Cache;
using Piranha.Extend.Fields;
using Piranha.Models;
using Piranha.Repositories;
using Piranha.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
			this._repo = repo;
			this._factory = factory;
			if (App.CacheLevel > CacheLevel.None)
			{
				this._cache = cache;
			}
		}

		public Task<T> CreateContentAsync<T>(string typeId = null)
		where T : SiteContentBase
		{
			if (String.IsNullOrEmpty(typeId))
			{
				typeId = typeof(T).Name;
			}
			SiteType byId = App.SiteTypes.GetById(typeId);
			if (byId == null)
			{
				return null;
			}
			return this._factory.CreateAsync<T>(byId);
		}

		public async Task DeleteAsync(Guid id)
		{
			ConfiguredTaskAwaitable<Site> configuredTaskAwaitable = this.GetByIdAsync(id).ConfigureAwait(false);
			Site site = await configuredTaskAwaitable;
			if (site != null)
			{
				await this.DeleteAsync(site).ConfigureAwait(false);
			}
		}

		public async Task DeleteAsync(Site model)
		{
			App.Hooks.OnBeforeDelete<Site>(model);
			ConfiguredTaskAwaitable configuredTaskAwaitable = this._repo.Delete(model.Id).ConfigureAwait(false);
			await configuredTaskAwaitable;
			App.Hooks.OnAfterDelete<Site>(model);
			this.RemoveFromCache(model);
		}

		public async Task<IEnumerable<Site>> GetAllAsync()
		{
			IEnumerable<Site> all = await this._repo.GetAll();
			if (all.Count<Site>() > 0)
			{
				foreach (Site site in all)
				{
					if (!(site.Logo != null) || !site.Logo.Id.HasValue)
					{
						continue;
					}
					await this._factory.InitFieldAsync(site.Logo, false);
				}
			}
			return all;
		}

		public async Task<Site> GetByHostnameAsync(string hostname)
		{
			Site site;
			IList<SiteService.SiteMapping> list;
			ConfiguredTaskAwaitable<IEnumerable<Site>> configuredTaskAwaitable;
			if (this._cache == null)
			{
				configuredTaskAwaitable = this.GetAllAsync().ConfigureAwait(false);
				IEnumerable<Site> sites = await configuredTaskAwaitable;
				IEnumerable<Site> hostnames = 
					from s in sites
					where (object)s.Hostnames != (object)null
					select s;
				list = (
					from s in hostnames
					select new SiteService.SiteMapping()
					{
						Id = s.Id,
						Hostnames = s.Hostnames
					}).ToList<SiteService.SiteMapping>();
			}
			else
			{
				list = this._cache.Get<IList<SiteService.SiteMapping>>("Site_Mappings");
				if (list == null)
				{
					configuredTaskAwaitable = this.GetAllAsync().ConfigureAwait(false);
					IEnumerable<Site> sites1 = await configuredTaskAwaitable;
					IEnumerable<Site> hostnames1 = 
						from s in sites1
						where (object)s.Hostnames != (object)null
						select s;
					list = (
						from s in hostnames1
						select new SiteService.SiteMapping()
						{
							Id = s.Id,
							Hostnames = s.Hostnames
						}).ToList<SiteService.SiteMapping>();
					this._cache.Set<IList<SiteService.SiteMapping>>("Site_Mappings", list);
				}
			}
			foreach (SiteService.SiteMapping siteMapping in list)
			{
				string str = siteMapping.Hostnames;
				string[] strArray = str.Split(new Char[] { ',' });
				int num = 0;
				while (num < (int)strArray.Length)
				{
					if (strArray[num].Trim().ToLower() != hostname)
					{
						num++;
					}
					else
					{
						ConfiguredTaskAwaitable<Site> configuredTaskAwaitable1 = this.GetByIdAsync(siteMapping.Id).ConfigureAwait(false);
						site = await configuredTaskAwaitable1;
						return site;
					}
				}
			}
			site = null;
			return site;
		}

		public async Task<Site> GetByIdAsync(Guid id)
		{
			Site site;
			ICache cache = this._cache;
			if (cache != null)
			{
				site = cache.Get<Site>(id.ToString());
			}
			else
			{
				site = null;
			}
			Site site1 = site;
			if (site1 == null)
			{
				ConfiguredTaskAwaitable<Site> configuredTaskAwaitable = this._repo.GetById(id).ConfigureAwait(false);
				site1 = await configuredTaskAwaitable;
				this.OnLoad(site1);
			}
			if (site1 != null && site1.Logo != null && site1.Logo.Id.HasValue)
			{
				await this._factory.InitFieldAsync(site1.Logo, false);
			}
			return site1;
		}

		public async Task<Site> GetByInternalIdAsync(string internalId)
		{
			Guid? id;
			ConfiguredTaskAwaitable<Site> configuredTaskAwaitable;
			Guid? nullable;
			ICache cache = this._cache;
			if (cache != null)
			{
				nullable = cache.Get<Guid?>(String.Concat("SiteId_", internalId));
			}
			else
			{
				id = null;
				nullable = id;
			}
			Guid? nullable1 = nullable;
			Site site = null;
			if (!nullable1.HasValue)
			{
				configuredTaskAwaitable = this._repo.GetByInternalId(internalId).ConfigureAwait(false);
				site = await configuredTaskAwaitable;
				this.OnLoad(site);
			}
			else
			{
				configuredTaskAwaitable = this.GetByIdAsync(nullable1.Value).ConfigureAwait(false);
				site = await configuredTaskAwaitable;
			}
			if (site != null && site.Logo != null)
			{
				id = site.Logo.Id;
				if (id.HasValue)
				{
					await this._factory.InitFieldAsync(site.Logo, false);
				}
			}
			return site;
		}

		public Task<DynamicSiteContent> GetContentByIdAsync(Guid id)
		{
			return this.GetContentByIdAsync<DynamicSiteContent>(id);
		}

		public async Task<T> GetContentByIdAsync<T>(Guid id)
		where T : SiteContent<T>
		{
			T t;
			SiteContentBase siteContentBase;
			SiteContentBase siteContentBase1 = null;
			if (!typeof(DynamicSiteContent).IsAssignableFrom(typeof(T)))
			{
				ICache cache = this._cache;
				if (cache != null)
				{
					siteContentBase = cache.Get<SiteContentBase>(String.Format("SiteContent_{0}", id));
				}
				else
				{
					siteContentBase = null;
				}
				siteContentBase1 = siteContentBase;
				if (siteContentBase1 != null)
				{
					await this._factory.InitAsync<SiteContentBase>(siteContentBase1, App.SiteTypes.GetById(siteContentBase1.TypeId));
				}
			}
			if (siteContentBase1 == null)
			{
				ConfiguredTaskAwaitable<T> configuredTaskAwaitable = this._repo.GetContentById<T>(id).ConfigureAwait(false);
				siteContentBase1 = await configuredTaskAwaitable;
				await this.OnLoadContentAsync(siteContentBase1).ConfigureAwait(false);
			}
			t = (siteContentBase1 == null || !(siteContentBase1 is T) ? default(T) : (T)siteContentBase1);
			return t;
		}

		public async Task<Site> GetDefaultAsync()
		{
			Site site;
			ICache cache = this._cache;
			if (cache != null)
			{
				site = cache.Get<Site>(String.Format("Site_{0}", Guid.Empty));
			}
			else
			{
				site = null;
			}
			Site site1 = site;
			if (site1 == null)
			{
				ConfiguredTaskAwaitable<Site> configuredTaskAwaitable = this._repo.GetDefault().ConfigureAwait(false);
				site1 = await configuredTaskAwaitable;
				this.OnLoad(site1);
			}
			if (site1 != null && site1.Logo != null && site1.Logo.Id.HasValue)
			{
				await this._factory.InitFieldAsync(site1.Logo, false);
			}
			return site1;
		}

		public async Task<Sitemap> GetSitemapAsync(Guid? id = null, bool onlyPublished = true)
		{
			Sitemap sitemap;
			Sitemap sitemap1;
			if (!id.HasValue)
			{
				Site site = await this.GetDefaultAsync().ConfigureAwait(false);
				if (site != null)
				{
					id = new Guid?(site.Id);
				}
			}
			if (!id.HasValue)
			{
				sitemap = null;
			}
			else
			{
				if (onlyPublished)
				{
					ICache cache = this._cache;
					if (cache != null)
					{
						sitemap1 = cache.Get<Sitemap>(String.Format("Sitemap_{0}", id));
					}
					else
					{
						sitemap1 = null;
					}
				}
				else
				{
					sitemap1 = null;
				}
				Sitemap sitemap2 = sitemap1;
				if (sitemap2 == null)
				{
					ConfiguredTaskAwaitable<Sitemap> configuredTaskAwaitable = this._repo.GetSitemap(id.Value, onlyPublished).ConfigureAwait(false);
					sitemap2 = await configuredTaskAwaitable;
					App.Hooks.OnLoad<Sitemap>(sitemap2);
					if (onlyPublished)
					{
						ICache cache1 = this._cache;
						if (cache1 != null)
						{
							cache1.Set<Sitemap>(String.Format("Sitemap_{0}", id), sitemap2);
						}
						else
						{
						}
					}
				}
				sitemap = sitemap2;
			}
			return sitemap;
		}

		public async Task InvalidateSitemapAsync(Guid id, bool updateLastModified = true)
		{
			Sitemap sitemap;
			if (updateLastModified)
			{
				ConfiguredTaskAwaitable<Site> configuredTaskAwaitable = this.GetByIdAsync(id).ConfigureAwait(false);
				Site nullable = await configuredTaskAwaitable;
				if (nullable != null)
				{
					nullable.ContentLastModified = new DateTime?(DateTime.Now);
					await this.SaveAsync(nullable).ConfigureAwait(false);
				}
			}
			ICache cache = this._cache;
			if (cache != null)
			{
				sitemap = cache.Get<Sitemap>(String.Format("Sitemap_{0}", id));
			}
			else
			{
				sitemap = null;
			}
			Sitemap sitemap1 = sitemap;
			if (sitemap1 != null)
			{
				App.Hooks.OnBeforeDelete<Sitemap>(sitemap1);
			}
			ICache cache1 = this._cache;
			if (cache1 != null)
			{
				cache1.Remove(String.Format("Sitemap_{0}", id));
			}
			else
			{
			}
		}

		private void OnLoad(Site model)
		{
			if (model != null)
			{
				App.Hooks.OnLoad<Site>(model);
				if (this._cache != null)
				{
					this._cache.Set<Site>(model.Id.ToString(), model);
					this._cache.Set<Guid>(String.Concat("SiteId_", model.InternalId), model.Id);
					if (model.IsDefault)
					{
						this._cache.Set<Site>(String.Format("Site_{0}", Guid.Empty), model);
					}
				}
			}
		}

		private async Task OnLoadContentAsync(SiteContentBase model)
		{
			if (model != null)
			{
				IDynamicContent dynamicContent = model as IDynamicContent;
				if (dynamicContent == null)
				{
					await this._factory.InitAsync<SiteContentBase>(model, App.SiteTypes.GetById(model.TypeId));
				}
				else
				{
					await this._factory.InitDynamicAsync<IDynamicContent>(dynamicContent, App.SiteTypes.GetById(model.TypeId));
				}
				App.Hooks.OnLoad<SiteContentBase>(model);
				if (this._cache != null && !(model is DynamicSiteContent))
				{
					this._cache.Set<SiteContentBase>(String.Format("SiteContent_{0}", model.Id), model);
				}
			}
		}

		private void RemoveContentFromCache<T>(T model)
		where T : SiteContentBase
		{
			ICache cache = this._cache;
			if (cache == null)
			{
				return;
			}
			cache.Remove(String.Format("SiteContent_{0}", model.Id));
		}

		private void RemoveFromCache(Site model)
		{
			if (this._cache != null)
			{
				this._cache.Remove(model.Id.ToString());
				this._cache.Remove(String.Concat("SiteId_", model.InternalId));
				if (model.IsDefault)
				{
					this._cache.Remove(String.Format("Site_{0}", Guid.Empty));
				}
				this._cache.Remove("Site_Mappings");
			}
		}

		public async Task RemoveSitemapFromCacheAsync(Guid id)
		{
			if (this._cache != null)
			{
				ConfiguredTaskAwaitable<Site> configuredTaskAwaitable = this.GetByIdAsync(id).ConfigureAwait(false);
				if (await configuredTaskAwaitable != null)
				{
					this._cache.Remove(String.Format("Sitemap_{0}", id));
				}
			}
		}

		public async Task SaveAsync(Site model)
		{
			ConfiguredTaskAwaitable configuredTaskAwaitable;
			if (model.Id == Guid.Empty)
			{
				model.Id = Guid.NewGuid();
			}
			Validator.ValidateObject(model, new ValidationContext(model), true);
			if (String.IsNullOrWhiteSpace(model.InternalId))
			{
				model.InternalId = Utils.GenerateInteralId(model.Title);
			}
			ConfiguredTaskAwaitable<Site> configuredTaskAwaitable1 = this._repo.GetByInternalId(model.InternalId).ConfigureAwait(false);
			Site site = await configuredTaskAwaitable1;
			if (site != null && site.Id != model.Id)
			{
				throw new ValidationException("The InternalId field must be unique");
			}
			if (!model.IsDefault)
			{
				configuredTaskAwaitable1 = this._repo.GetDefault().ConfigureAwait(false);
				Site site1 = await configuredTaskAwaitable1;
				if (site1 == null || site1.Id == model.Id)
				{
					model.IsDefault = true;
				}
			}
			else
			{
				configuredTaskAwaitable1 = this._repo.GetDefault().ConfigureAwait(false);
				Site site2 = await configuredTaskAwaitable1;
				if (site2 != null && site2.Id != model.Id)
				{
					site2.IsDefault = false;
					configuredTaskAwaitable = this._repo.Save(site2).ConfigureAwait(false);
					await configuredTaskAwaitable;
					this.RemoveFromCache(site2);
				}
				site2 = null;
			}
			App.Hooks.OnBeforeSave<Site>(model);
			configuredTaskAwaitable = this._repo.Save(model).ConfigureAwait(false);
			await configuredTaskAwaitable;
			App.Hooks.OnAfterSave<Site>(model);
			this.RemoveFromCache(model);
		}

		public async Task SaveContentAsync<T>(Guid siteId, T model)
		where T : SiteContent<T>
		{
			if (model.Id != siteId)
			{
				model.Id = siteId;
			}
			if (model.Id == Guid.Empty)
			{
				throw new ValidationException("The Id field is required for this operation");
			}
			ValidationContext validationContext = new ValidationContext((object)model);
			Validator.ValidateObject(model, validationContext, true);
			App.Hooks.OnBeforeSave<SiteContentBase>(model);
			ConfiguredTaskAwaitable configuredTaskAwaitable = this._repo.SaveContent<T>(siteId, model).ConfigureAwait(false);
			await configuredTaskAwaitable;
			App.Hooks.OnAfterSave<SiteContentBase>(model);
			this.RemoveContentFromCache<T>(model);
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
			}
		}
	}
}