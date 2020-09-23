using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixAttributeFields;
using Mix.Cms.Lib.ViewModels.MixTemplates;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		public List<Mix.Cms.Lib.ViewModels.MixAttributeFields.DeleteViewModel> RemoveAttributes
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
			this.u003cRemoveAttributesu003ek__BackingField = new List<Mix.Cms.Lib.ViewModels.MixAttributeFields.DeleteViewModel>();
			base();
			return;
		}

		public UpdateViewModel(MixAttributeSet model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.u003cRemoveAttributesu003ek__BackingField = new List<Mix.Cms.Lib.ViewModels.MixAttributeFields.DeleteViewModel>();
			base(model, _context, _transaction);
			return;
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.get_Id() <= 0)
			{
				this.set_Fields(new List<Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel>());
				return;
			}
			stackVariable6 = ViewModelBase<MixCmsContext, MixAttributeField, Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel>.Repository;
			V_0 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel::ExpandView(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Void ExpandView(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public override MixAttributeSet ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (this.get_Id() == 0)
			{
				stackVariable37 = ViewModelBase<MixCmsContext, MixAttributeSet, Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel>.Repository;
				V_0 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
				// Current member / type: Mix.Cms.Lib.Models.Cms.MixAttributeSet Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel::ParseModel(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Exception in: Mix.Cms.Lib.Models.Cms.MixAttributeSet ParseModel(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com


		public override RepositoryResponse<bool> SaveSubModels(MixAttributeSet parent, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			stackVariable0 = new RepositoryResponse<bool>();
			stackVariable0.set_IsSucceed(true);
			V_0 = stackVariable0;
			if (V_0.get_IsSucceed())
			{
				V_1 = this.get_Fields().GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = V_1.get_Current();
						if (!V_0.get_IsSucceed())
						{
							break;
						}
						V_2.set_AttributeSetName(parent.get_Name());
						V_2.set_AttributeSetId(parent.get_Id());
						ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel>(V_2.SaveModel(false, _context, _transaction), ref V_0);
					}
				}
				finally
				{
					((IDisposable)V_1).Dispose();
				}
			}
			return V_0;
		}

		public override async Task<RepositoryResponse<bool>> SaveSubModelsAsync(MixAttributeSet parent, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.parent = parent;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel.u003cSaveSubModelsAsyncu003ed__93>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public override void Validate(MixCmsContext _context, IDbContextTransaction _transaction)
		{
			this.Validate(_context, _transaction);
			if (this.get_IsValid())
			{
				stackVariable6 = _context.get_MixAttributeSet();
				V_0 = Expression.Parameter(System.Type.GetTypeFromHandle(// 
				// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixAttributeSets.UpdateViewModel::Validate(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Exception in: System.Void Validate(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com

	}
}