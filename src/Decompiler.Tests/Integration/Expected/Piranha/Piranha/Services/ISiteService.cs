using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Piranha.Services
{
	public interface ISiteService
	{
		Task<T> CreateContentAsync<T>(string typeId = null)
		where T : SiteContentBase;

		Task DeleteAsync(Guid id);

		Task DeleteAsync(Site model);

		Task<IEnumerable<Site>> GetAllAsync();

		Task<Site> GetByHostnameAsync(string hostname);

		Task<Site> GetByIdAsync(Guid id);

		Task<Site> GetByInternalIdAsync(string internalId);

		Task<DynamicSiteContent> GetContentByIdAsync(Guid id);

		Task<T> GetContentByIdAsync<T>(Guid id)
		where T : SiteContent<T>;

		Task<Site> GetDefaultAsync();

		Task<Sitemap> GetSitemapAsync(Guid? id = null, bool onlyPublished = true);

		Task InvalidateSitemapAsync(Guid id, bool updateLastModified = true);

		Task RemoveSitemapFromCacheAsync(Guid id);

		Task SaveAsync(Site model);

		Task SaveContentAsync<T>(Guid siteId, T model)
		where T : SiteContent<T>;
	}
}