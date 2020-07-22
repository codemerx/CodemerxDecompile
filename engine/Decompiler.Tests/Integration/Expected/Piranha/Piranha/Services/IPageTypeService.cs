using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Piranha.Services
{
	public interface IPageTypeService
	{
		Task DeleteAsync(string id);

		Task DeleteAsync(PageType model);

		Task<IEnumerable<PageType>> GetAllAsync();

		Task<PageType> GetByIdAsync(string id);

		Task SaveAsync(PageType model);
	}
}