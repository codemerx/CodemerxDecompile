using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixAttributeFields;
using Mix.Cms.Lib.ViewModels.MixTemplates;
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
	public class UpdateViewModel : ViewModelBase<MixCmsContext, MixAttributeSet, Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel>
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

		[JsonProperty("edmView")]
		public Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel EdmView
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

		[JsonProperty("formView")]
		public Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel FormView
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

		[JsonProperty("removeAttributes")]
		public List<Mix.Cms.Lib.ViewModels.MixAttributeFields.DeleteViewModel> RemoveAttributes { get; set; } = new List<Mix.Cms.Lib.ViewModels.MixAttributeFields.DeleteViewModel>();

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

		[JsonProperty("title")]
		public string Title
		{
			get;
			set;
		}

		[JsonProperty("type")]
		public MixEnums.MixAttributeSetDataType? Type
		{
			get;
			set;
		}

		public UpdateViewModel()
		{
		}

		public UpdateViewModel(MixAttributeSet model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			List<Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel> list;
			if (this.Id <= 0)
			{
				this.Fields = new List<Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel>();
				return;
			}
			List<Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel> data = ViewModelBase<MixCmsContext, MixAttributeField, Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel>.Repository.GetModelListBy((MixAttributeField a) => a.AttributeSetId == this.Id, _context, _transaction).get_Data();
			if (data != null)
			{
				list = (
					from a in data
					orderby a.Priority
					select a).ToList<Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel>();
			}
			else
			{
				list = null;
			}
			this.Fields = list;
		}

		public override MixAttributeSet ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.Id == 0)
			{
				this.Id = ViewModelBase<MixCmsContext, MixAttributeSet, Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel>.Repository.Max((MixAttributeSet s) => s.Id, _context, _transaction).get_Data() + 1;
				this.CreatedDateTime = DateTime.UtcNow;
			}
			this.FormTemplate = (this.FormView != null ? string.Format("{0}/{1}{2}", this.FormView.FolderType, this.FormView.FileName, this.FormView.Extension) : this.FormTemplate);
			this.EdmTemplate = (this.EdmView != null ? string.Format("{0}/{1}{2}", this.EdmView.FolderType, this.EdmView.FileName, this.EdmView.Extension) : this.EdmTemplate);
			return base.ParseModel(_context, _transaction);
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
			if (repositoryResponse1.get_IsSucceed())
			{
				foreach (Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel field in this.Fields)
				{
					if (!repositoryResponse1.get_IsSucceed())
					{
						break;
					}
					field.AttributeSetId = parent.Id;
					field.AttributeSetName = parent.Name;
					ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel>(await field.SaveModelAsync(false, _context, _transaction), ref repositoryResponse1);
				}
			}
			if (repositoryResponse1.get_IsSucceed())
			{
				foreach (Mix.Cms.Lib.ViewModels.MixAttributeFields.DeleteViewModel removeAttribute in this.RemoveAttributes)
				{
					if (!repositoryResponse1.get_IsSucceed())
					{
						break;
					}
					ViewModelHelper.HandleResult<MixAttributeField>(await removeAttribute.RemoveModelAsync(false, _context, _transaction), ref repositoryResponse1);
				}
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