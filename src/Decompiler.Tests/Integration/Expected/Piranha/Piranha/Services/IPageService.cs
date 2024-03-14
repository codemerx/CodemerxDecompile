using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Piranha.Services
{
	public interface IPageService
	{
		Task<T> CopyAsync<T>(T originalPage)
		where T : PageBase;

		Task<T> CreateAsync<T>(string typeId = null)
		where T : PageBase;

		Task DeleteAsync(Guid id);

		Task DeleteAsync<T>(T model)
		where T : PageBase;

		Task DeleteCommentAsync(Guid id);

		Task DeleteCommentAsync(Comment model);

		Task DetachAsync<T>(T model)
		where T : PageBase;

		Task<IEnumerable<DynamicPage>> GetAllAsync(Guid? siteId = null);

		Task<IEnumerable<T>> GetAllAsync<T>(Guid? siteId = null)
		where T : PageBase;

		Task<IEnumerable<DynamicPage>> GetAllBlogsAsync(Guid? siteId = null);

		Task<IEnumerable<T>> GetAllBlogsAsync<T>(Guid? siteId = null)
		where T : PageBase;

		Task<IEnumerable<Comment>> GetAllCommentsAsync(Guid? pageId = null, bool onlyApproved = true, int? page = null, int? pageSize = null);

		Task<IEnumerable<Guid>> GetAllDraftsAsync(Guid? siteId = null);

		Task<IEnumerable<Comment>> GetAllPendingCommentsAsync(Guid? pageId = null, int? page = null, int? pageSize = null);

		Task<DynamicPage> GetByIdAsync(Guid id);

		Task<T> GetByIdAsync<T>(Guid id)
		where T : PageBase;

		Task<DynamicPage> GetBySlugAsync(string slug, Guid? siteId = null);

		Task<T> GetBySlugAsync<T>(string slug, Guid? siteId = null)
		where T : PageBase;

		Task<Comment> GetCommentByIdAsync(Guid id);

		Task<DynamicPage> GetDraftByIdAsync(Guid id);

		Task<T> GetDraftByIdAsync<T>(Guid id)
		where T : PageBase;

		Task<Guid?> GetIdBySlugAsync(string slug, Guid? siteId = null);

		Task<DynamicPage> GetStartpageAsync(Guid? siteId = null);

		Task<T> GetStartpageAsync<T>(Guid? siteId = null)
		where T : PageBase;

		Task MoveAsync<T>(T model, Guid? parentId, int sortOrder)
		where T : PageBase;

		Task SaveAsync<T>(T model)
		where T : PageBase;

		Task SaveCommentAndVerifyAsync(Guid pageId, Comment model);

		Task SaveCommentAsync(Guid pageId, Comment model);

		Task SaveDraftAsync<T>(T model)
		where T : PageBase;
	}
}