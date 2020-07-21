using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Piranha.Repositories
{
	public interface IArchiveRepository
	{
		Task<int> GetPostCount(Guid archiveId, Guid? categoryId = null, Guid? tagId = null, int? year = null, int? month = null);

		Task<IEnumerable<Guid>> GetPosts(Guid archiveId, int pageSize, int currentPage, Guid? categoryId = null, Guid? tagId = null, int? year = null, int? month = null);
	}
}