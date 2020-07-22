using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixAttributeFields;
using Mix.Cms.Lib.ViewModels.MixAttributeSetValues;
using Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas;
using Mix.Domain.Core.Models;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixAttributeSetDatas
{
	public class UpdateViewModel : ViewModelBase<MixCmsContext, MixAttributeSetData, Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel>
	{
		[JsonProperty("attributeSetId")]
		public int AttributeSetId
		{
			get;
			set;
		}

		[JsonProperty("attributeSetName")]
		public string AttributeSetName
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

		[JsonProperty("cultures")]
		public List<SupportedCulture> Cultures
		{
			get;
			set;
		}

		[JsonProperty("data")]
		public JObject Data
		{
			get;
			set;
		}

		[JsonProperty("dataNavs")]
		public List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel> DataNavs
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

		[JsonProperty("id")]
		public string Id
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

		[JsonProperty("parentId")]
		public string ParentId
		{
			get;
			set;
		}

		[JsonProperty("parentType")]
		public MixEnums.MixAttributeSetDataType ParentType
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

		[JsonProperty("relatedData")]
		public List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel> RelatedData
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

		[JsonProperty("values")]
		public List<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel> Values
		{
			get;
			set;
		}

		public UpdateViewModel()
		{
			this.u003cRelatedDatau003ek__BackingField = new List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel>();
			base();
			return;
		}

		public UpdateViewModel(MixAttributeSetData model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.u003cRelatedDatau003ek__BackingField = new List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel>();
			base(model, _context, _transaction);
			return;
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			stackVariable1 = ViewModelBase<MixCmsContext, MixRelatedAttributeData, Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel>.Repository;
			V_0 = Expression.Parameter(Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel::ExpandView(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Void ExpandView(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		private void ParseData()
		{
			stackVariable1 = new JObject();
			stackVariable1.Add(new JProperty("id", this.get_Id()));
			stackVariable1.Add(new JProperty("specificulture", this.get_Specificulture()));
			stackVariable1.Add(new JProperty("createdDateTime", (object)this.get_CreatedDateTime()));
			this.set_Data(stackVariable1);
			stackVariable16 = this.get_Values();
			stackVariable17 = Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel.u003cu003ec.u003cu003e9__81_0;
			if (stackVariable17 == null)
			{
				dummyVar0 = stackVariable17;
				stackVariable17 = new Func<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel, int>(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel.u003cu003ec.u003cu003e9, Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel.u003cu003ec.u003cParseDatau003eb__81_0);
				Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel.u003cu003ec.u003cu003e9__81_0 = stackVariable17;
			}
			V_0 = Enumerable.OrderBy<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel, int>(stackVariable16, stackVariable17).GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					this.get_Data().Add(this.ParseValue(V_1));
				}
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
			return;
		}

		public override MixAttributeSetData ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (string.IsNullOrEmpty(this.get_Id()))
			{
				this.set_Id(Guid.NewGuid().ToString());
				this.set_CreatedDateTime(DateTime.get_UtcNow());
				if (this.get_AttributeSetId() != 0 || string.IsNullOrEmpty(this.get_AttributeSetName()))
				{
					if (this.get_AttributeSetId() > 0 && string.IsNullOrEmpty(this.get_AttributeSetName()))
					{
						stackVariable22 = ViewModelBase<MixCmsContext, MixAttributeSet, Mix.Cms.Lib.ViewModels.MixAttributeSets.ReadViewModel>.Repository;
						V_1 = Expression.Parameter(Type.GetTypeFromHandle(// 
						// Current member / type: Mix.Cms.Lib.Models.Cms.MixAttributeSetData Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel::ParseModel(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
						// Exception in: Mix.Cms.Lib.Models.Cms.MixAttributeSetData ParseModel(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
						// Specified method is not supported.
						// 
						// mailto: JustDecompilePublicFeedback@telerik.com


		private JProperty ParseValue(Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel item)
		{
			switch (item.get_DataType())
			{
				case 0:
				case 4:
				case 5:
				case 7:
				case 8:
				case 9:
				case 10:
				case 11:
				case 12:
				case 13:
				case 14:
				case 15:
				case 16:
				case 17:
				case 19:
				case 20:
				case 21:
				{
				Label0:
					return new JProperty(item.get_AttributeFieldName(), item.get_StringValue());
				}
				case 1:
				{
					return new JProperty(item.get_AttributeFieldName(), (object)item.get_DateTimeValue());
				}
				case 2:
				{
					return new JProperty(item.get_AttributeFieldName(), (object)item.get_DateTimeValue());
				}
				case 3:
				{
					return new JProperty(item.get_AttributeFieldName(), (object)item.get_DateTimeValue());
				}
				case 6:
				{
					return new JProperty(item.get_AttributeFieldName(), (object)item.get_DoubleValue());
				}
				case 18:
				{
					return new JProperty(item.get_AttributeFieldName(), (object)item.get_BooleanValue());
				}
				case 22:
				{
					return new JProperty(item.get_AttributeFieldName(), (object)item.get_IntegerValue());
				}
				case 23:
				{
					return new JProperty(item.get_AttributeFieldName(), new JArray());
				}
				default:
				{
					goto Label0;
				}
			}
		}

		public override async Task<RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel>> SaveModelAsync(bool isSaveSubModels = false, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0.u003cu003e4__this = this;
			V_0.isSaveSubModels = isSaveSubModels;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel.u003cSaveModelAsyncu003ed__76>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private async Task<RepositoryResponse<bool>> SaveRelatedDataAsync(MixAttributeSetData parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.parent = parent;
			V_0.context = context;
			V_0.transaction = transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel.u003cSaveRelatedDataAsyncu003ed__79>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public override RepositoryResponse<bool> SaveSubModels(MixAttributeSetData parent, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			stackVariable0 = new RepositoryResponse<bool>();
			stackVariable0.set_IsSucceed(true);
			V_0 = stackVariable0;
			if (V_0.get_IsSucceed())
			{
				V_1 = this.get_Values().GetEnumerator();
				try
				{
					while (V_1.MoveNext())
					{
						V_2 = new Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel.u003cu003ec__DisplayClass78_0();
						V_2.item = V_1.get_Current();
						if (!V_0.get_IsSucceed())
						{
							break;
						}
						V_2.item.set_Field(this.get_Fields().Find(new Predicate<Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel>(V_2, Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel.u003cu003ec__DisplayClass78_0.u003cSaveSubModelsu003eb__0)));
						stackVariable25 = V_2.item;
						stackVariable28 = V_2.item.get_Field();
						if (stackVariable28 != null)
						{
							stackVariable29 = stackVariable28.get_Priority();
						}
						else
						{
							dummyVar0 = stackVariable28;
							stackVariable29 = V_2.item.get_Priority();
						}
						stackVariable25.set_Priority(stackVariable29);
						V_2.item.set_DataId(parent.get_Id());
						V_2.item.set_Specificulture(parent.get_Specificulture());
						ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel>(V_2.item.SaveModel(false, _context, _transaction), ref V_0);
					}
				}
				finally
				{
					V_1.Dispose();
				}
			}
			return V_0;
		}

		public override async Task<RepositoryResponse<bool>> SaveSubModelsAsync(MixAttributeSetData parent, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.parent = parent;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel.u003cSaveSubModelsAsyncu003ed__77>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}