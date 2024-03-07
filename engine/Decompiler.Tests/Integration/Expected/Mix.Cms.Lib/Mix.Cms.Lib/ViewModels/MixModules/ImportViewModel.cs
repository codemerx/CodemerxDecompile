using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixModuleDatas;
using Mix.Domain.Core.Models;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixModules
{
	public class ImportViewModel : ViewModelBase<MixCmsContext, MixModule, Mix.Cms.Lib.ViewModels.MixModules.ImportViewModel>
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

		[JsonProperty("cultures")]
		public List<SupportedCulture> Cultures
		{
			get;
			set;
		}

		[JsonProperty("data")]
		public PaginationModel<ReadViewModel> Data { get; set; } = new PaginationModel<ReadViewModel>();

		[JsonProperty("description")]
		public string Description
		{
			get;
			set;
		}

		[JsonProperty("edmTemplate")]
		public string EdmTemplate
		{
			get;
			set;
		}

		[JsonProperty("fields")]
		public string Fields
		{
			get;
			set;
		}

		[JsonProperty("formTemplate")]
		public string FormTemplate
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

		[JsonProperty("isExportData")]
		public bool IsExportData
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

		[JsonProperty("name")]
		[Required]
		public string Name
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

		[JsonProperty("pageSize")]
		public int? PageSize
		{
			get;
			set;
		}

		[JsonProperty("postId")]
		public string PostId
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

		[JsonProperty("template")]
		public string Template
		{
			get;
			set;
		}

		[JsonProperty("thumbnail")]
		public string Thumbnail
		{
			get;
			set;
		}

		[JsonProperty("title")]
		[Required]
		public string Title
		{
			get;
			set;
		}

		[JsonProperty("type")]
		public MixEnums.MixModuleType Type
		{
			get;
			set;
		}

		public ImportViewModel()
		{
		}

		public ImportViewModel(MixModule model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
		}

		public void LoadData(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<PaginationModel<ReadViewModel>> repositoryResponse = new RepositoryResponse<PaginationModel<ReadViewModel>>();
			DefaultRepository<!0, !1, !2> repository = ViewModelBase<MixCmsContext, MixModuleData, ReadViewModel>.Repository;
			int? nullable = null;
			int? nullable1 = nullable;
			nullable = null;
			repository.GetModelListBy((MixModuleData m) => m.ModuleId == this.Id && m.Specificulture == this.Specificulture, "Priority", 0, nullable1, nullable, _context, _transaction);
		}

		public override MixModule ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.Id == 0)
			{
				this.Id = ViewModelBase<MixCmsContext, MixModule, ReadListItemViewModel>.Repository.Max((MixModule m) => m.Id, _context, _transaction).get_Data() + 1;
				this.LastModified = new DateTime?(DateTime.UtcNow);
				this.CreatedDateTime = DateTime.UtcNow;
			}
			return base.ParseModel(_context, _transaction);
		}

		public override Task<RepositoryResponse<MixModule>> RemoveModelAsync(bool isRemoveRelatedModels = false, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			return base.RemoveModelAsync(isRemoveRelatedModels, _context, _transaction);
		}

		public override async Task<RepositoryResponse<bool>> SaveSubModelsAsync(MixModule parent, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			foreach (ReadViewModel item in this.Data.get_Items())
			{
				if (!repositoryResponse1.get_IsSucceed())
				{
					continue;
				}
				item.Specificulture = parent.Specificulture;
				item.ModuleId = parent.Id;
				item.CreatedDateTime = DateTime.UtcNow;
				ViewModelHelper.HandleResult<ReadViewModel>(await item.SaveModelAsync(false, _context, _transaction), ref repositoryResponse1);
			}
			return repositoryResponse1;
		}

		public override void Validate(MixCmsContext _context, IDbContextTransaction _transaction)
		{
			base.Validate(_context, _transaction);
			if (base.get_IsValid() && this.Id == 0)
			{
				base.set_IsValid(!ViewModelBase<MixCmsContext, MixModule, Mix.Cms.Lib.ViewModels.MixModules.ImportViewModel>.Repository.CheckIsExists((MixModule m) => {
					if (m.Name != this.Name)
					{
						return false;
					}
					return m.Specificulture == this.Specificulture;
				}, _context, _transaction));
				if (!base.get_IsValid())
				{
					base.get_Errors().Add("Module Name Existed");
				}
			}
		}
	}
}