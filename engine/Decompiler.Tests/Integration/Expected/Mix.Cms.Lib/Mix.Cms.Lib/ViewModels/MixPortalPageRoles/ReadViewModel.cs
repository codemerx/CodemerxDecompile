using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixPortalPages;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels.MixPortalPageRoles
{
	public class ReadViewModel : ViewModelBase<MixCmsContext, MixPortalPageRole, Mix.Cms.Lib.ViewModels.MixPortalPageRoles.ReadViewModel>
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

		[JsonProperty("id")]
		public int Id
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

		[JsonProperty("page")]
		public Mix.Cms.Lib.ViewModels.MixPortalPages.ReadViewModel Page
		{
			get;
			set;
		}

		[JsonProperty("pageId")]
		public int PageId
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

		[JsonProperty("roleId")]
		public string RoleId
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

		public ReadViewModel(MixPortalPageRole model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public ReadViewModel()
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<Mix.Cms.Lib.ViewModels.MixPortalPages.ReadViewModel> singleModel = ViewModelBase<MixCmsContext, MixPortalPage, Mix.Cms.Lib.ViewModels.MixPortalPages.ReadViewModel>.Repository.GetSingleModel((MixPortalPage p) => p.Id == this.Id, _context, _transaction);
			if (singleModel.get_IsSucceed())
			{
				this.Page = singleModel.get_Data();
			}
		}

		public override MixPortalPageRole ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.CreatedDateTime == new DateTime())
			{
				this.CreatedDateTime = DateTime.UtcNow;
			}
			return base.ParseModel(_context, _transaction);
		}
	}
}