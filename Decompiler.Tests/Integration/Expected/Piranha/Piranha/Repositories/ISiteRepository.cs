using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Piranha.Repositories
{
	public interface ISiteRepository
	{
		Task Delete(Guid id);

		Task<IEnumerable<Site>> GetAll();

		Task<Site> GetById(Guid id);

		Task<Site> GetByInternalId(string internalId);

		Task<DynamicSiteContent> GetContentById(Guid id);

		Task<T> GetContentById<T>(Guid id)
		where T : SiteContent<T>;

		Task<Site> GetDefault();

		Task<Sitemap> GetSitemap(Guid id, bool onlyPublished = true);

		Task Save(Site model);

		Task SaveContent<T>(Guid siteId, T content)
		where T : SiteContent<T>;
	}
}