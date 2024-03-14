using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Piranha.Repositories
{
	public interface IPostTypeRepository
	{
		Task Delete(string id);

		Task<IEnumerable<PostType>> GetAll();

		Task<PostType> GetById(string id);

		Task Save(PostType model);
	}
}