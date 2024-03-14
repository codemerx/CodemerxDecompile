using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Piranha.Repositories
{
	public interface IMediaRepository
	{
		Task<int> CountAll(Guid? folderId);

		Task Delete(Guid id);

		Task DeleteFolder(Guid id);

		Task<IEnumerable<Guid>> GetAll(Guid? folderId = null);

		Task<IEnumerable<Guid>> GetAllFolders(Guid? folderId = null);

		Task<IEnumerable<Media>> GetById(params Guid[] ids);

		Task<Media> GetById(Guid id);

		Task<MediaFolder> GetFolderById(Guid id);

		Task<MediaStructure> GetStructure();

		Task Move(Media model, Guid? folderId);

		Task Save(Media model);

		Task SaveFolder(MediaFolder model);
	}
}