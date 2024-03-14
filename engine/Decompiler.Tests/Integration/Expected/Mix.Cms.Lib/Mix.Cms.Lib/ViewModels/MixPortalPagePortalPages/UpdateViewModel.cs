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
			Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel.u003cUpdateInfosAsyncu003ed__64 variable = new Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel.u003cUpdateInfosAsyncu003ed__64();
			variable.cates = cates;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel>>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel.u003cUpdateInfosAsyncu003ed__64>(ref variable);
			return variable.u003cu003et__builder.Task;
		}
	}
}