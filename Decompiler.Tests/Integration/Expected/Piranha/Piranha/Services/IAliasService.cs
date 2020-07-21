using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Piranha.Services
{
	public interface IAliasService
	{
		Task DeleteAsync(Guid id);

		Task DeleteAsync(Alias model);

		Task<IEnumerable<Alias>> GetAllAsync(Guid? siteId = null);

		Task<Alias> GetByAliasUrlAsync(string url, Guid? siteId = null);

		Task<Alias> GetByIdAsync(Guid id);

		Task<IEnumerable<Alias>> GetByRedirectUrlAsync(string url, Guid? siteId = null);

		Task SaveAsync(Alias model);
	}
}