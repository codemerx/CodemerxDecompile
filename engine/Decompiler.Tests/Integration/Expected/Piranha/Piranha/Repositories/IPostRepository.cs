using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Piranha.Repositories
{
	public interface IPostRepository
	{
		Task CreateRevision(Guid id, int revisions);

		Task Delete(Guid id);

		Task DeleteComment(Guid id);

		Task DeleteDraft(Guid id);

		Task<IEnumerable<Guid>> GetAll(Guid blogId, int? index = null, int? pageSize = null);

		Task<IEnumerable<Guid>> GetAllBySiteId(Guid siteId);

		Task<IEnumerable<Taxonomy>> GetAllCategories(Guid blogId);

		Task<IEnumerable<Comment>> GetAllComments(Guid? postId, bool onlyApproved, int page, int pageSize);

		Task<IEnumerable<Guid>> GetAllDrafts(Guid blogId);

		Task<IEnumerable<Comment>> GetAllPendingComments(Guid? postId, int page, int pageSize);

		Task<IEnumerable<Taxonomy>> GetAllTags(Guid blogId);

		Task<T> GetById<T>(Guid id)
		where T : PostBase;

		Task<T> GetBySlug<T>(Guid blogId, string slug)
		where T : PostBase;

		Task<Taxonomy> GetCategoryById(Guid id);

		Task<Taxonomy> GetCategoryBySlug(Guid blogId, string slug);

		Task<Comment> GetCommentById(Guid id);

		Task<int> GetCount(Guid archiveId);

		Task<T> GetDraftById<T>(Guid id)
		where T : PostBase;

		Task<Taxonomy> GetTagById(Guid id);

		Task<Taxonomy> GetTagBySlug(Guid blogId, string slug);

		Task Save<T>(T model)
		where T : PostBase;

		Task SaveComment(Guid postId, Comment model);

		Task SaveDraft<T>(T model)
		where T : PostBase;
	}
}