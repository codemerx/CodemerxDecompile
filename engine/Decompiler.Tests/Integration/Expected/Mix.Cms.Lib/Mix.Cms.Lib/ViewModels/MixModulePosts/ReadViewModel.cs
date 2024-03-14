using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixModules;
using Mix.Cms.Lib.ViewModels.MixPosts;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels.MixModulePosts
{
	public class ReadViewModel : ViewModelBase<MixCmsContext, MixModulePost, Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel>
	{
		[JsonProperty("createdBy")]
		public string CreatedBy
		{
			get;
			set;
		}

		[JsonProperty("createdDateTime")]
		public DateTime CreatedDateTime
		{
			get;
			set;
		}

		[JsonProperty("description")]
		public string Description
		{
			get;
			set;
		}

		[JsonProperty("id")]
		public int Id
		{
			get;
			set;
		}

		[JsonProperty("image")]
		public string Image
		{
			get;
			set;
		}

		[JsonProperty("isActived")]
		public bool IsActived
		{
			get;
			set;
		}

		[JsonProperty("lastModified")]
		public DateTime? LastModified
		{
			get;
			set;
		}

		[JsonProperty("modifiedBy")]
		public string ModifiedBy
		{
			get;
			set;
		}

		[JsonProperty("module")]
		public Mix.Cms.Lib.ViewModels.MixModules.ReadListItemViewModel Module
		{
			get;
			set;
		}

		[JsonProperty("moduleId")]
		public int ModuleId
		{
			get;
			set;
		}

		[JsonProperty("post")]
		public Mix.Cms.Lib.ViewModels.MixPosts.ReadListItemViewModel Post
		{
			get;
			set;
		}

		[JsonProperty("postId")]
		public int PostId
		{
			get;
			set;
		}

		[JsonProperty("priority")]
		public int Priority
		{
			get;
			set;
		}

		[JsonProperty("specificulture")]
		public string Specificulture
		{
			get;
			set;
		}

		[JsonProperty("status")]
		public MixEnums.MixContentStatus Status
		{
			get;
			set;
		}

		public ReadViewModel(MixModulePost model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public ReadViewModel()
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<Mix.Cms.Lib.ViewModels.MixPosts.ReadListItemViewModel> singleModel = ViewModelBase<MixCmsContext, MixPost, Mix.Cms.Lib.ViewModels.MixPosts.ReadListItemViewModel>.Repository.GetSingleModel((MixPost p) => p.Id == this.PostId && p.Specificulture == this.Specificulture, _context, _transaction);
			if (singleModel.get_IsSucceed())
			{
				this.Post = singleModel.get_Data();
			}
			RepositoryResponse<Mix.Cms.Lib.ViewModels.MixModules.ReadListItemViewModel> repositoryResponse = ViewModelBase<MixCmsContext, MixModule, Mix.Cms.Lib.ViewModels.MixModules.ReadListItemViewModel>.Repository.GetSingleModel((MixModule p) => p.Id == this.ModuleId && p.Specificulture == this.Specificulture, _context, _transaction);
			if (repositoryResponse.get_IsSucceed())
			{
				this.Module = repositoryResponse.get_Data();
			}
		}

		public static RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel>> GetModulePostNavAsync(int postId, string specificulture, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel>> repositoryResponse;
			Func<MixModulePost, bool> func1 = null;
			MixCmsContext mixCmsContext = _context ?? new MixCmsContext();
			IDbContextTransaction dbContextTransaction = _transaction ?? mixCmsContext.get_Database().BeginTransaction();
			try
			{
				try
				{
					IEnumerable<Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel> readViewModels = (
						from a in EntityFrameworkQueryableExtensions.Include<MixModule, ICollection<MixModulePost>>(mixCmsContext.MixModule, (MixModule cp) => cp.MixModulePost)
						where a.Specificulture == specificulture && (a.Type == 2 || a.Type == 6)
						select a).AsEnumerable<MixModule>().Select<MixModule, Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel>((MixModule p) => {
						Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel readViewModel = new Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel(new MixModulePost()
						{
							PostId = postId,
							ModuleId = p.Id,
							Specificulture = specificulture
						}, _context, _transaction);
						ICollection<MixModulePost> mixModulePost = p.MixModulePost;
						Func<MixModulePost, bool> u003cu003e9_3 = func1;
						if (u003cu003e9_3 == null)
						{
							Func<MixModulePost, bool> func2 = (MixModulePost cp) => {
								if (cp.PostId != postId)
								{
									return false;
								}
								return cp.Specificulture == specificulture;
							};
							Func<MixModulePost, bool> func = func2;
							func1 = func2;
							u003cu003e9_3 = func;
						}
						readViewModel.IsActived = mixModulePost.Any<MixModulePost>(u003cu003e9_3);
						readViewModel.Description = p.Title;
						return readViewModel;
					});
					RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel>> repositoryResponse1 = new RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel>>();
					repositoryResponse1.set_IsSucceed(true);
					repositoryResponse1.set_Data(readViewModels.ToList<Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel>());
					repositoryResponse = repositoryResponse1;
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					if (_transaction == null)
					{
						dbContextTransaction.Rollback();
					}
					RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel>> repositoryResponse2 = new RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel>>();
					repositoryResponse2.set_IsSucceed(true);
					repositoryResponse2.set_Data(null);
					repositoryResponse2.set_Exception(exception);
					repositoryResponse = repositoryResponse2;
				}
			}
			finally
			{
				if (_context == null)
				{
					dbContextTransaction.Dispose();
					RelationalDatabaseFacadeExtensions.CloseConnection(mixCmsContext.get_Database());
					dbContextTransaction.Dispose();
					mixCmsContext.Dispose();
				}
			}
			return repositoryResponse;
		}

		public override MixModulePost ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.Id == 0)
			{
				this.Id = ViewModelBase<MixCmsContext, MixModulePost, Mix.Cms.Lib.ViewModels.MixModulePosts.ReadViewModel>.Repository.Max((MixModulePost c) => c.Id, _context, _transaction).get_Data() + 1;
				this.CreatedDateTime = DateTime.UtcNow;
			}
			return base.ParseModel(_context, _transaction);
		}
	}
}