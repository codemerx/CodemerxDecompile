using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Piranha.Services
{
	public interface ISiteTypeService
	{
		Task DeleteAsync(string id);

		Task DeleteAsync(SiteType model);

		Task<IEnumerable<SiteType>> GetAllAsync();

		Task<SiteType> GetByIdAsync(string id);

		Task SaveAsync(SiteType model);
	}
}