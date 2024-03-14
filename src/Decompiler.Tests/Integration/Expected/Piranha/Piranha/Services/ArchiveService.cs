using Piranha;
using Piranha.Models;
using Piranha.Repositories;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Piranha.Services
{
	public class ArchiveService : IArchiveService
	{
		private readonly IArchiveRepository _repo;

		private readonly IParamService _paramService;

		private readonly IPostService _postService;

		public ArchiveService(IArchiveRepository repo, IParamService paramService, IPostService postService)
		{
			this._repo = repo;
			this._paramService = paramService;
			this._postService = postService;
		}

		public Task<PostArchive<DynamicPost>> GetByIdAsync(Guid archiveId, int? currentPage = 1, Guid? categoryId = null, Guid? tagId = null, int? year = null, int? month = null, int? pageSize = null)
		{
			return this.GetByIdAsync<DynamicPost>(archiveId, currentPage, categoryId, tagId, year, month, pageSize);
		}

		public async Task<PostArchive<T>> GetByIdAsync<T>(Guid archiveId, int? currentPage = 1, Guid? categoryId = null, Guid? tagId = null, int? year = null, int? month = null, int? pageSize = null)
		where T : PostBase
		{
			Taxonomy taxonomy;
			ConfiguredTaskAwaitable<Taxonomy> configuredTaskAwaitable;
			int num;
			PostArchive<T> postArchive = new PostArchive<T>();
			if (!pageSize.HasValue)
			{
				using (Config config = new Config(this._paramService))
				{
					pageSize = new int?(config.ArchivePageSize);
					if (pageSize.HasValue)
					{
						int? nullable = pageSize;
						if (!(nullable.GetValueOrDefault() == 0 & nullable.HasValue))
						{
							goto Label0;
						}
					}
					pageSize = new int?(5);
				Label0:
				}
			}
			postArchive.Year = year;
			postArchive.Month = month;
			PostArchive<T> postArchive1 = postArchive;
			ConfiguredTaskAwaitable<int> configuredTaskAwaitable1 = this._repo.GetPostCount(archiveId, categoryId, tagId, year, month).ConfigureAwait(false);
			postArchive1.TotalPosts = await configuredTaskAwaitable1;
			postArchive1 = null;
			postArchive.TotalPages = Math.Max(Convert.ToInt32(Math.Ceiling((double)postArchive.TotalPosts / (double)pageSize.Value)), 1);
			PostArchive<T> postArchive2 = postArchive;
			num = (currentPage.HasValue ? currentPage.Value : 1);
			postArchive2.CurrentPage = Math.Min(Math.Max(1, num), postArchive.TotalPages);
			if (categoryId.HasValue)
			{
				postArchive1 = postArchive;
				configuredTaskAwaitable = this._postService.GetCategoryByIdAsync(categoryId.Value).ConfigureAwait(false);
				taxonomy = await configuredTaskAwaitable;
				postArchive1.Category = taxonomy;
				postArchive1 = null;
			}
			if (tagId.HasValue)
			{
				postArchive1 = postArchive;
				configuredTaskAwaitable = this._postService.GetTagByIdAsync(tagId.Value).ConfigureAwait(false);
				taxonomy = await configuredTaskAwaitable;
				postArchive1.Tag = taxonomy;
				postArchive1 = null;
			}
			ConfiguredTaskAwaitable<IEnumerable<Guid>> configuredTaskAwaitable2 = this._repo.GetPosts(archiveId, pageSize.Value, postArchive.CurrentPage, categoryId, tagId, year, month).ConfigureAwait(false);
			foreach (Guid guid in await configuredTaskAwaitable2)
			{
				ConfiguredTaskAwaitable<T> configuredTaskAwaitable3 = this._postService.GetByIdAsync<T>(guid).ConfigureAwait(false);
				T t = await configuredTaskAwaitable3;
				if (t == null)
				{
					continue;
				}
				postArchive.Posts.Add(t);
			}
			return postArchive;
		}
	}
}