using Piranha;
using Piranha.Models;
using Piranha.Repositories;
using System;
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
			base();
			this._repo = repo;
			this._paramService = paramService;
			this._postService = postService;
			return;
		}

		public Task<PostArchive<DynamicPost>> GetByIdAsync(Guid archiveId, int? currentPage = 1, Guid? categoryId = null, Guid? tagId = null, int? year = null, int? month = null, int? pageSize = null)
		{
			return this.GetByIdAsync<DynamicPost>(archiveId, currentPage, categoryId, tagId, year, month, pageSize);
		}

		public async Task<PostArchive<T>> GetByIdAsync<T>(Guid archiveId, int? currentPage = 1, Guid? categoryId = null, Guid? tagId = null, int? year = null, int? month = null, int? pageSize = null)
		where T : PostBase
		{
			V_0.u003cu003e4__this = this;
			V_0.archiveId = archiveId;
			V_0.currentPage = currentPage;
			V_0.categoryId = categoryId;
			V_0.tagId = tagId;
			V_0.year = year;
			V_0.month = month;
			V_0.pageSize = pageSize;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<PostArchive<T>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ArchiveService.u003cGetByIdAsyncu003ed__5<T>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}