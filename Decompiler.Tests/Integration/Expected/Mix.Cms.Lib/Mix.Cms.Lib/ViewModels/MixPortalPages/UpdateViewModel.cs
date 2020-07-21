using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixPortalPages
{
	public class UpdateViewModel : ViewModelBase<MixCmsContext, MixPortalPage, Mix.Cms.Lib.ViewModels.MixPortalPages.UpdateViewModel>
	{
		[JsonProperty("childNavs")]
		public List<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel> ChildNavs
		{
			get;
			set;
		}

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
		public string Descriotion
		{
			get;
			set;
		}

		[JsonProperty("icon")]
		public string Icon
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

		[JsonProperty("parentNavs")]
		public List<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel> ParentNavs
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

		[JsonProperty("textDefault")]
		public string TextDefault
		{
			get;
			set;
		}

		[JsonProperty("textKeyword")]
		public string TextKeyword
		{
			get;
			set;
		}

		[JsonProperty("title")]
		public string Title
		{
			get;
			set;
		}

		[JsonProperty("url")]
		public string Url
		{
			get;
			set;
		}

		public UpdateViewModel()
		{
			base();
			return;
		}

		public UpdateViewModel(MixPortalPage model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			base(model, _context, _transaction);
			return;
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.set_ParentNavs(this.GetParentNavs(_context, _transaction));
			this.set_ChildNavs(this.GetChildNavs(_context, _transaction));
			return;
		}

		public List<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel> GetChildNavs(MixCmsContext context, IDbContextTransaction transaction)
		{
			V_0 = new Mix.Cms.Lib.ViewModels.MixPortalPages.UpdateViewModel.u003cu003ec__DisplayClass75_0();
			V_0.u003cu003e4__this = this;
			V_0.context = context;
			V_0.transaction = transaction;
			stackVariable9 = V_0.context.get_MixPortalPage();
			V_1 = Expression.Parameter(Type.GetTypeFromHandle(// 
			// Current member / type: System.Collections.Generic.List`1<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel> Mix.Cms.Lib.ViewModels.MixPortalPages.UpdateViewModel::GetChildNavs(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Collections.Generic.List<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel> GetChildNavs(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public List<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel> GetParentNavs(MixCmsContext context, IDbContextTransaction transaction)
		{
			V_0 = new Mix.Cms.Lib.ViewModels.MixPortalPages.UpdateViewModel.u003cu003ec__DisplayClass74_0();
			V_0.u003cu003e4__this = this;
			V_0.context = context;
			stackVariable7 = V_0.context.get_MixPortalPage();
			V_1 = Expression.Parameter(Type.GetTypeFromHandle(// 
			// Current member / type: System.Collections.Generic.List`1<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel> Mix.Cms.Lib.ViewModels.MixPortalPages.UpdateViewModel::GetParentNavs(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Collections.Generic.List<Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages.UpdateViewModel> GetParentNavs(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public override MixPortalPage ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.get_Id() == 0)
			{
				stackVariable33 = ViewModelBase<MixCmsContext, MixPortalPage, Mix.Cms.Lib.ViewModels.MixPortalPages.UpdateViewModel>.Repository;
				V_1 = Expression.Parameter(Type.GetTypeFromHandle(// 
				// Current member / type: Mix.Cms.Lib.Models.Cms.MixPortalPage Mix.Cms.Lib.ViewModels.MixPortalPages.UpdateViewModel::ParseModel(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Exception in: Mix.Cms.Lib.Models.Cms.MixPortalPage ParseModel(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com


		public override async Task<RepositoryResponse<bool>> RemoveRelatedModelsAsync(Mix.Cms.Lib.ViewModels.MixPortalPages.UpdateViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0.u003cu003e4__this = this;
			V_0._context = _context;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixPortalPages.UpdateViewModel.u003cRemoveRelatedModelsAsyncu003ed__73>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public override async Task<RepositoryResponse<bool>> SaveSubModelsAsync(MixPortalPage parent, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.parent = parent;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixPortalPages.UpdateViewModel.u003cSaveSubModelsAsyncu003ed__72>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}