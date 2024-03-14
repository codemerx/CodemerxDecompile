using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Piranha.Repositories
{
	public interface IParamRepository
	{
		Task Delete(Guid id);

		Task<IEnumerable<Param>> GetAll();

		Task<Param> GetById(Guid id);

		Task<Param> GetByKey(string key);

		Task Save(Param model);
	}
}