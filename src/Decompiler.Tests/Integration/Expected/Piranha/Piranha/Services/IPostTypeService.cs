using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Piranha.Services
{
	public interface IPostTypeService
	{
		Task DeleteAsync(string id);

		Task DeleteAsync(PostType model);

		Task<IEnumerable<PostType>> GetAllAsync();

		Task<PostType> GetByIdAsync(string id);

		Task SaveAsync(PostType model);
	}
}