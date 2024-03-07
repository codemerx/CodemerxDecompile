using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixPortalPages;
using Mix.Common.Helper;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages
{
	public class UpdateViewModel : ViewModelBase<MixCmsContext, MixPortalPageNavigation, Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel>
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

		[JsonProperty("level")]
		public int Level
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

		[JsonProperty("page")]
		public UpdateRolePermissionViewModel Page
		{
			get;
			set;
		}

		[JsonProperty("parentId")]
		public int ParentId
		{
			get;
			set;
		}

		[JsonProperty("parent")]
		public Mix.Cms.Lib.ViewModels.MixPortalPages.ReadViewModel ParentPage
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

		public UpdateViewModel(MixPortalPageNavigation model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public UpdateViewModel()
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<UpdateRolePermissionViewModel> singleModel = ViewModelBase<MixCmsContext, MixPortalPage, UpdateRolePermissionViewModel>.Repository.GetSingleModel((MixPortalPage p) => p.Id == this.Id, null, null);
			if (singleModel.get_IsSucceed())
			{
				this.Page = singleModel.get_Data();
			}
		}

		public override async Task<RepositoryResponse<bool>> SaveSubModelsAsync(MixPortalPageNavigation parent, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			RepositoryResponse<bool> repositoryResponse;
			if (this.Page == null)
			{
				repositoryResponse = await this.u003cu003en__0(parent, _context, _transaction);
			}
			else
			{
				RepositoryResponse<UpdateRolePermissionViewModel> repositoryResponse1 = await this.Page.SaveModelAsync(false, _context, _transaction);
				RepositoryResponse<bool> repositoryResponse2 = new RepositoryResponse<bool>();
				repositoryResponse2.set_IsSucceed(repositoryResponse1.get_IsSucceed());
				repositoryResponse2.set_Data(repositoryResponse1.get_IsSucceed());
				repositoryResponse2.set_Errors(repositoryResponse1.get_Errors());
				repositoryResponse2.set_Exception(repositoryResponse1.get_Exception());
				repositoryResponse = repositoryResponse2;
			}
			return repositoryResponse;
		}

		public static async Task<RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel>>> UpdateInfosAsync(List<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel> cates)
		{
			RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel>> repositoryResponse;
			MixCmsContext mixCmsContext = new MixCmsContext();
			IDbContextTransaction dbContextTransaction = mixCmsContext.get_Database().BeginTransaction();
			RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel>> repositoryResponse1 = new RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel>>();
			try
			{
				try
				{
					foreach (Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel cate in cates)
					{
						RepositoryResponse<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel> repositoryResponse2 = await cate.SaveModelAsync(false, mixCmsContext, dbContextTransaction);
						repositoryResponse1.set_IsSucceed(repositoryResponse2.get_IsSucceed());
						if (repositoryResponse1.get_IsSucceed())
						{
							continue;
						}
						repositoryResponse1.get_Errors().AddRange(repositoryResponse2.get_Errors());
						repositoryResponse1.set_Exception(repositoryResponse2.get_Exception());
						break;
					}
					UnitOfWorkHelper<MixCmsContext>.HandleTransaction(repositoryResponse1.get_IsSucceed(), true, dbContextTransaction);
					repositoryResponse = repositoryResponse1;
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					UnitOfWorkHelper<MixCmsContext>.HandleException<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel>(exception, true, dbContextTransaction);
					RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel>> repositoryResponse3 = new RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel>>();
					repositoryResponse3.set_IsSucceed(false);
					repositoryResponse3.set_Data(null);
					repositoryResponse3.set_Exception(exception);
					repositoryResponse = repositoryResponse3;
				}
			}
			finally
			{
				dbContextTransaction.Dispose();
				RelationalDatabaseFacadeExtensions.CloseConnection(mixCmsContext.get_Database());
				dbContextTransaction.Dispose();
				mixCmsContext.Dispose();
			}
			return repositoryResponse;
		}
	}
}