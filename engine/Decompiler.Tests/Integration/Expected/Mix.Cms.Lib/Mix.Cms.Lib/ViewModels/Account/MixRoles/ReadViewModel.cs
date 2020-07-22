using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib.Models.Account;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages;
using Mix.Cms.Lib.ViewModels.MixPortalPageRoles;
using Mix.Cms.Lib.ViewModels.MixPortalPages;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.Account.MixRoles
{
	public class ReadViewModel : ViewModelBase<MixCmsAccountContext, AspNetRoles, Mix.Cms.Lib.ViewModels.Account.MixRoles.ReadViewModel>
	{
		[JsonProperty("concurrencyStamp")]
		public string ConcurrencyStamp
		{
			get;
			set;
		}

		[JsonProperty("id")]
		public string Id
		{
			get;
			set;
		}

		[JsonProperty("name")]
		[Required]
		public string Name
		{
			get;
			set;
		}

		[JsonProperty("normalizedName")]
		public string NormalizedName
		{
			get;
			set;
		}

		[JsonProperty("permissions")]
		public List<ReadRolePermissionViewModel> Permissions
		{
			get;
			set;
		}

		public ReadViewModel()
		{
			base();
			return;
		}

		public ReadViewModel(AspNetRoles model, MixCmsAccountContext _context = null, IDbContextTransaction _transaction = null)
		{
			base(model, _context, _transaction);
			return;
		}

		public override void ExpandView(MixCmsAccountContext _context = null, IDbContextTransaction _transaction = null)
		{
			stackVariable1 = ViewModelBase<MixCmsContext, MixPortalPage, ReadRolePermissionViewModel>.Repository;
			V_0 = Expression.Parameter(Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Mix.Cms.Lib.ViewModels.Account.MixRoles.ReadViewModel::ExpandView(Mix.Cms.Lib.Models.Account.MixCmsAccountContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Void ExpandView(Mix.Cms.Lib.Models.Account.MixCmsAccountContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		private List<Mix.Cms.Lib.ViewModels.MixPortalPageRoles.ReadViewModel> GetPermission()
		{
			V_0 = new Mix.Cms.Lib.ViewModels.Account.MixRoles.ReadViewModel.u003cu003ec__DisplayClass25_0();
			V_0.u003cu003e4__this = this;
			V_0.context = new MixCmsContext();
			try
			{
				V_1 = new Mix.Cms.Lib.ViewModels.Account.MixRoles.ReadViewModel.u003cu003ec__DisplayClass25_1();
				V_1.CSu0024u003cu003e8__locals1 = V_0;
				V_1.transaction = V_1.CSu0024u003cu003e8__locals1.context.get_Database().BeginTransaction();
				stackVariable17 = V_1.CSu0024u003cu003e8__locals1.context.get_MixPortalPage();
				V_2 = Expression.Parameter(Type.GetTypeFromHandle(// 
				// Current member / type: System.Collections.Generic.List`1<Mix.Cms.Lib.ViewModels.MixPortalPageRoles.ReadViewModel> Mix.Cms.Lib.ViewModels.Account.MixRoles.ReadViewModel::GetPermission()
				// Exception in: System.Collections.Generic.List<Mix.Cms.Lib.ViewModels.MixPortalPageRoles.ReadViewModel> GetPermission()
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com


		public override AspNetRoles ParseModel(MixCmsAccountContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (string.IsNullOrEmpty(this.get_Id()))
			{
				this.set_Id(Guid.NewGuid().ToString());
			}
			return this.ParseModel(_context, _transaction);
		}

		public override async Task<RepositoryResponse<bool>> RemoveRelatedModelsAsync(Mix.Cms.Lib.ViewModels.Account.MixRoles.ReadViewModel view, MixCmsAccountContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0.u003cu003e4__this = this;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.Account.MixRoles.ReadViewModel.u003cRemoveRelatedModelsAsyncu003ed__23>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}