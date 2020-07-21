using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Piranha.Repositories
{
	public interface IPageRepository
	{
		Task CreateRevision(Guid id, int revisions);

		Task<IEnumerable<Guid>> Delete(Guid id);

		Task DeleteComment(Guid id);

		Task DeleteDraft(Guid id);

		Task<IEnumerable<Guid>> GetAll(Guid siteId);

		Task<IEnumerable<Guid>> GetAllBlogs(Guid siteId);

		Task<IEnumerable<Comment>> GetAllComments(Guid? pageId, bool onlyApproved, int page, int pageSize);

		Task<IEnumerable<Guid>> GetAllDrafts(Guid siteId);

		Task<IEnumerable<Comment>> GetAllPendingComments(Guid? pageId, int page, int pageSize);

		Task<T> GetById<T>(Guid id)
		where T : PageBase;

		Task<T> GetBySlug<T>(string slug, Guid siteId)
		where T : PageBase;

		Task<Comment> GetCommentById(Guid id);

		Task<T> GetDraftById<T>(Guid id)
		where T : PageBase;

		Task<T> GetStartpage<T>(Guid siteId)
		where T : PageBase;

		Task<IEnumerable<Guid>> Move<T>(T model, Guid? parentId, int sortOrder)
		where T : PageBase;

		Task<IEnumerable<Guid>> Save<T>(T model)
		where T : PageBase;

		Task SaveComment(Guid pageId, Comment model);

		Task SaveDraft<T>(T model)
		where T : PageBase;
	}
}