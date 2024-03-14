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
	public class ReadViewModel : ViewModelBase<MixCmsContext, MixPortalPageNavigation, Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.ReadViewModel>
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
		public ReadRolePermissionViewModel Page
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

		public ReadViewModel(MixPortalPageNavigation model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public ReadViewModel()
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<ReadRolePermissionViewModel> singleModel = ViewModelBase<MixCmsContext, MixPortalPage, ReadRolePermissionViewModel>.Repository.GetSingleModel((MixPortalPage p) => p.Id == this.Id, null, null);
			if (singleModel.get_IsSucceed())
			{
				this.Page = singleModel.get_Data();
			}
		}

		public static async Task<RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.ReadViewModel>>> UpdateInfosAsync(List<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.ReadViewModel> cates)
		{
			Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.ReadViewModel.u003cUpdateInfosAsyncu003ed__59 variable = new Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.ReadViewModel.u003cUpdateInfosAsyncu003ed__59();
			variable.cates = cates;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.ReadViewModel>>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.ReadViewModel.u003cUpdateInfosAsyncu003ed__59>(ref variable);
			return variable.u003cu003et__builder.Task;
		}
	}
}