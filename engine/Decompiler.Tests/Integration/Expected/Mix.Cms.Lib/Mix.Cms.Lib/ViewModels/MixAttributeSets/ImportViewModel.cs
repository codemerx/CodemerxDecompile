using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixAttributeFields;
using Mix.Cms.Lib.ViewModels.MixAttributeSetDatas;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixAttributeSets
{
	public class ImportViewModel : ViewModelBase<MixCmsContext, MixAttributeSet, Mix.Cms.Lib.ViewModels.MixAttributeSets.ImportViewModel>
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

		[JsonIgnore]
		[JsonProperty("data")]
		public List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel> Data
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

		[JsonProperty("edmAutoSend")]
		public bool? EdmAutoSend
		{
			get;
			set;
		}

		[JsonProperty("edmFrom")]
		public string EdmFrom
		{
			get;
			set;
		}

		[JsonProperty("edmSubject")]
		public string EdmSubject
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
		public List<Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel> Fields
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
		public string Name
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

		[JsonProperty("ReferenceId")]
		public int? ReferenceId
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

		[JsonProperty("title")]
		public string Title
		{
			get;
			set;
		}

		[JsonProperty("type")]
		public int? Type
		{
			get;
			set;
		}

		public ImportViewModel()
		{
		}

		public ImportViewModel(MixAttributeSet model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
		}

		public override MixAttributeSet ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.Id == 0)
			{
				this.Id = ViewModelBase<MixCmsContext, MixAttributeSet, Mix.Cms.Lib.ViewModels.MixAttributeSets.ImportViewModel>.Repository.Max((MixAttributeSet s) => s.Id, _context, _transaction).get_Data() + 1;
				this.CreatedDateTime = DateTime.UtcNow;
			}
			return base.ParseModel(_context, _transaction);
		}

		private async Task<RepositoryResponse<bool>> SaveDataAsync(MixAttributeSet parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			Mix.Cms.Lib.ViewModels.MixAttributeSets.ImportViewModel.u003cSaveDataAsyncu003ed__86 variable = new Mix.Cms.Lib.ViewModels.MixAttributeSets.ImportViewModel.u003cSaveDataAsyncu003ed__86();
			variable.u003cu003e4__this = this;
			variable.parent = parent;
			variable.context = context;
			variable.transaction = transaction;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixAttributeSets.ImportViewModel.u003cSaveDataAsyncu003ed__86>(ref variable);
			return variable.u003cu003et__builder.Task;
		}

		private async Task<RepositoryResponse<bool>> SaveFieldsAsync(MixAttributeSet parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			Mix.Cms.Lib.ViewModels.MixAttributeSets.ImportViewModel.u003cSaveFieldsAsyncu003ed__87 variable = new Mix.Cms.Lib.ViewModels.MixAttributeSets.ImportViewModel.u003cSaveFieldsAsyncu003ed__87();
			variable.u003cu003e4__this = this;
			variable.parent = parent;
			variable.context = context;
			variable.transaction = transaction;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixAttributeSets.ImportViewModel.u003cSaveFieldsAsyncu003ed__87>(ref variable);
			return variable.u003cu003et__builder.Task;
		}

		public override RepositoryResponse<bool> SaveSubModels(MixAttributeSet parent, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			if (repositoryResponse1.get_IsSucceed())
			{
				foreach (Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel field in this.Fields)
				{
					if (!repositoryResponse1.get_IsSucceed())
					{
						break;
					}
					field.AttributeSetName = parent.Name;
					field.AttributeSetId = parent.Id;
					ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel>(field.SaveModel(false, _context, _transaction), ref repositoryResponse1);
				}
			}
			return repositoryResponse1;
		}

		public override async Task<RepositoryResponse<bool>> SaveSubModelsAsync(MixAttributeSet parent, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			if (this.Fields != null)
			{
				repositoryResponse1 = await this.SaveFieldsAsync(parent, _context, _transaction);
			}
			return repositoryResponse1;
		}

		public override void Validate(MixCmsContext _context, IDbContextTransaction _transaction)
		{
			base.Validate(_context, _transaction);
			if (base.get_IsValid())
			{
				if (_context.MixAttributeSet.Any<MixAttributeSet>((MixAttributeSet s) => s.Name == this.Name && s.Id != this.Id))
				{
					base.set_IsValid(false);
					base.get_Errors().Add(string.Concat(this.Name, " is Existed"));
				}
			}
		}
	}
}