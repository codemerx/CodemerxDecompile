using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Piranha.Services
{
	public interface IPostService
	{
		Task<T> CreateAsync<T>(string typeId = null)
		where T : PostBase;

		Task DeleteAsync(Guid id);

		Task DeleteAsync<T>(T model)
		where T : PostBase;

		Task DeleteCommentAsync(Guid id);

		Task DeleteCommentAsync(Comment model);

		Task<IEnumerable<DynamicPost>> GetAllAsync(Guid blogId, int? index = null, int? pageSize = null);

		Task<IEnumerable<T>> GetAllAsync<T>(Guid blogId, int? index = null, int? pageSize = null)
		where T : PostBase;

		Task<IEnumerable<DynamicPost>> GetAllAsync(string slug, Guid? siteId = null);

		Task<IEnumerable<T>> GetAllAsync<T>(string slug, Guid? siteId = null)
		where T : PostBase;

		Task<IEnumerable<DynamicPost>> GetAllBySiteIdAsync(Guid? siteId = null);

		Task<IEnumerable<T>> GetAllBySiteIdAsync<T>(Guid? siteId = null)
		where T : PostBase;

		Task<IEnumerable<Taxonomy>> GetAllCategoriesAsync(Guid blogId);

		Task<IEnumerable<Comment>> GetAllCommentsAsync(Guid? postId = null, bool onlyApproved = true, int? page = null, int? pageSize = null);

		Task<IEnumerable<Guid>> GetAllDraftsAsync(Guid blogId);

		Task<IEnumerable<Comment>> GetAllPendingCommentsAsync(Guid? postId = null, int? page = null, int? pageSize = null);

		Task<IEnumerable<Taxonomy>> GetAllTagsAsync(Guid blogId);

		Task<DynamicPost> GetByIdAsync(Guid id);

		Task<T> GetByIdAsync<T>(Guid id)
		where T : PostBase;

		Task<DynamicPost> GetBySlugAsync(string blog, string slug, Guid? siteId = null);

		Task<T> GetBySlugAsync<T>(string blog, string slug, Guid? siteId = null)
		where T : PostBase;

		Task<DynamicPost> GetBySlugAsync(Guid blogId, string slug);

		Task<T> GetBySlugAsync<T>(Guid blogId, string slug)
		where T : PostBase;

		Task<Taxonomy> GetCategoryByIdAsync(Guid id);

		Task<Taxonomy> GetCategoryBySlugAsync(Guid blogId, string slug);

		Task<Comment> GetCommentByIdAsync(Guid id);

		Task<int> GetCountAsync(Guid archiveId);

		Task<DynamicPost> GetDraftByIdAsync(Guid id);

		Task<T> GetDraftByIdAsync<T>(Guid id)
		where T : PostBase;

		Task<Taxonomy> GetTagByIdAsync(Guid id);

		Task<Taxonomy> GetTagBySlugAsync(Guid blogId, string slug);

		Task SaveAsync<T>(T model)
		where T : PostBase;

		Task SaveCommentAndVerifyAsync(Guid postId, Comment model);

		Task SaveCommentAsync(Guid postId, Comment model);

		Task SaveDraftAsync<T>(T model)
		where T : PostBase;
	}
}