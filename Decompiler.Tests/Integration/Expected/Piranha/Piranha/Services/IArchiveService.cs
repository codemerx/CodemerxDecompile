using Piranha.Models;
using System;
using System.Threading.Tasks;

namespace Piranha.Services
{
	public interface IArchiveService
	{
		Task<PostArchive<DynamicPost>> GetByIdAsync(Guid archiveId, int? currentPage = 1, Guid? categoryId = null, Guid? tagId = null, int? year = null, int? month = null, int? pageSize = null);

		Task<PostArchive<T>> GetByIdAsync<T>(Guid archiveId, int? currentPage = 1, Guid? categoryId = null, Guid? tagId = null, int? year = null, int? month = null, int? pageSize = null)
		where T : PostBase;
	}
}