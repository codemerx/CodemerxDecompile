using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Piranha.Services
{
	public interface IMediaService
	{
		Task<int> CountFolderItemsAsync(Guid? folderId = null);

		Task DeleteAsync(Guid id);

		Task DeleteAsync(Media model);

		Task DeleteFolderAsync(Guid id);

		Task DeleteFolderAsync(MediaFolder model);

		string EnsureVersion(Guid id, int width, int? height = null);

		Task<string> EnsureVersionAsync(Guid id, int width, int? height = null);

		Task<string> EnsureVersionAsync(Media media, int width, int? height = null);

		Task<IEnumerable<Media>> GetAllByFolderIdAsync(Guid? folderId = null);

		Task<IEnumerable<MediaFolder>> GetAllFoldersAsync(Guid? folderId = null);

		Task<Media> GetByIdAsync(Guid id);

		Task<IEnumerable<Media>> GetByIdAsync(params Guid[] ids);

		Task<MediaFolder> GetFolderByIdAsync(Guid id);

		Task<MediaStructure> GetStructureAsync();

		Task MoveAsync(Media model, Guid? folderId);

		Task SaveAsync(Media model);

		Task SaveAsync(MediaContent content);

		Task SaveFolderAsync(MediaFolder model);
	}
}