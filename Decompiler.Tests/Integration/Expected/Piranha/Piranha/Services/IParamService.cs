using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Piranha.Services
{
	public interface IParamService
	{
		Task DeleteAsync(Guid id);

		Task DeleteAsync(Param model);

		Task<IEnumerable<Param>> GetAllAsync();

		Task<Param> GetByIdAsync(Guid id);

		Task<Param> GetByKeyAsync(string key);

		Task SaveAsync(Param model);
	}
}