using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Piranha.Repositories
{
	public interface IAliasRepository
	{
		Task Delete(Guid id);

		Task<IEnumerable<Alias>> GetAll(Guid siteId);

		Task<Alias> GetByAliasUrl(string url, Guid siteId);

		Task<Alias> GetById(Guid id);

		Task<IEnumerable<Alias>> GetByRedirectUrl(string url, Guid siteId);

		Task Save(Alias model);
	}
}