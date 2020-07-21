using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixPortalPages;
using Mix.Domain.Core.ViewModels;
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

		public ReadViewModel(MixPortalPageNavigation model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			base(model, _context, _transaction);
			return;
		}

		public ReadViewModel()
		{
			base();
			return;
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			stackVariable0 = ViewModelBase<MixCmsContext, MixPortalPage, ReadRolePermissionViewModel>.Repository;
			V_1 = Expression.Parameter(Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.ReadViewModel::ExpandView(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Void ExpandView(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public static async Task<RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.ReadViewModel>>> UpdateInfosAsync(List<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.ReadViewModel> cates)
		{
			V_0.cates = cates;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.ReadViewModel>>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.ReadViewModel.u003cUpdateInfosAsyncu003ed__59>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}