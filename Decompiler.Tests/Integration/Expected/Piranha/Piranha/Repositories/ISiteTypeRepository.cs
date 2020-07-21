using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Piranha.Repositories
{
	public interface ISiteTypeRepository
	{
		Task Delete(string id);

		Task<IEnumerable<SiteType>> GetAll();

		Task<SiteType> GetById(string id);

		Task Save(SiteType model);
	}
}