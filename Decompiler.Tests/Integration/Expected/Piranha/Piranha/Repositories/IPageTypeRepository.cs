using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Piranha.Repositories
{
	public interface IPageTypeRepository
	{
		Task Delete(string id);

		Task<IEnumerable<PageType>> GetAll();

		Task<PageType> GetById(string id);

		Task Save(PageType model);
	}
}