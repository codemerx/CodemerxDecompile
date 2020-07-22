using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Primitives;
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
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixAttributeSetDatas
{
	public class FormViewModel : ViewModelBase<MixCmsContext, MixAttributeSetData, Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel>
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

		[JsonProperty("obj")]
		public JObject Obj
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

		public List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel> RefData
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

		public List<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel> Values
		{
			get;
			set;
		}

		public FormViewModel()
		{
			this.u003cRelatedDatau003ek__BackingField = new List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel>();
			this.u003cRefDatau003ek__BackingField = new List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel>();
			base();
			return;
		}

		public FormViewModel(MixAttributeSetData model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.u003cRelatedDatau003ek__BackingField = new List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel>();
			this.u003cRefDatau003ek__BackingField = new List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel>();
			base(model, _context, _transaction);
			return;
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			stackVariable1 = ViewModelBase<MixCmsContext, MixAttributeField, Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel>.Repository;
			V_0 = Expression.Parameter(Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel::ExpandView(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Void ExpandView(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public static Task<RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel>>> FilterByValueAsync(string culture, string attributeSetName, Dictionary<string, StringValues> queryDictionary, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0 = new Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel.u003cu003ec__DisplayClass84_0();
			V_0.culture = culture;
			V_0.attributeSetName = attributeSetName;
			UnitOfWorkHelper<MixCmsContext>.InitTransaction(_context, _transaction, ref V_1, ref V_2, ref V_3);
			try
			{
				try
				{
					V_4 = new Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel.u003cu003ec__DisplayClass84_1();
					V_4.CSu0024u003cu003e8__locals1 = V_0;
					V_7 = Expression.Parameter(Type.GetTypeFromHandle(// 
					// Current member / type: System.Threading.Tasks.Task`1<Mix.Domain.Core.ViewModels.RepositoryResponse`1<System.Collections.Generic.List`1<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel>>> Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel::FilterByValueAsync(System.String,System.String,System.Collections.Generic.Dictionary`2<System.String,Microsoft.Extensions.Primitives.StringValues>,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
					// Exception in: System.Threading.Tasks.Task<Mix.Domain.Core.ViewModels.RepositoryResponse<System.Collections.Generic.List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel>>> FilterByValueAsync(System.String,System.String,System.Collections.Generic.Dictionary<System.String,Microsoft.Extensions.Primitives.StringValues>,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
					// Specified method is not supported.
					// 
					// mailto: JustDecompilePublicFeedback@telerik.com


		public override void GenerateCache(MixAttributeSetData model, Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.ParseData(null, null);
			this.GenerateCache(model, view, _context, _transaction);
			return;
		}

		private void ParseData(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			stackVariable0 = ViewModelBase<MixCmsContext, MixAttributeSetValue, Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel>.Repository;
			V_1 = Expression.Parameter(Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel::ParseData(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Void ParseData(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public override MixAttributeSetData ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (string.IsNullOrEmpty(this.get_Id()))
			{
				this.set_Id(Guid.NewGuid().ToString());
				this.set_CreatedDateTime(DateTime.get_UtcNow());
				if (this.get_Priority() == 0)
				{
					stackVariable315 = ViewModelBase<MixCmsContext, MixAttributeSetData, Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel>.Repository;
					V_1 = Expression.Parameter(Type.GetTypeFromHandle(// 
					// Current member / type: Mix.Cms.Lib.Models.Cms.MixAttributeSetData Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel::ParseModel(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
					// Exception in: Mix.Cms.Lib.Models.Cms.MixAttributeSetData ParseModel(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
					// Specified method is not supported.
					// 
					// mailto: JustDecompilePublicFeedback@telerik.com


		private void ParseModelValue(JToken property, Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel item)
		{
			if (item.get_Field().get_IsEncrypt())
			{
				V_0 = Newtonsoft.Json.Linq.Extensions.Value<JObject>(property);
				item.set_StringValue(V_0.ToString(0, Array.Empty<JsonConverter>()));
				stackVariable91 = item;
				stackVariable94 = V_0.get_Item("data");
				if (stackVariable94 != null)
				{
					stackVariable95 = stackVariable94.ToString();
				}
				else
				{
					dummyVar0 = stackVariable94;
					stackVariable95 = null;
				}
				stackVariable91.set_EncryptValue(stackVariable95);
				stackVariable96 = item;
				stackVariable99 = V_0.get_Item("key");
				if (stackVariable99 != null)
				{
					stackVariable100 = stackVariable99.ToString();
				}
				else
				{
					dummyVar1 = stackVariable99;
					stackVariable100 = null;
				}
				stackVariable96.set_EncryptKey(stackVariable100);
				return;
			}
			switch (item.get_Field().get_DataType())
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
				case 17:
				case 19:
				case 20:
				case 21:
				{
				Label0:
					item.set_StringValue(Newtonsoft.Json.Linq.Extensions.Value<string>(property));
					break;
				}
				case 1:
				{
					item.set_DateTimeValue(Newtonsoft.Json.Linq.Extensions.Value<DateTime?>(property));
					item.set_StringValue(Newtonsoft.Json.Linq.Extensions.Value<string>(property));
					return;
				}
				case 2:
				{
					item.set_DateTimeValue(Newtonsoft.Json.Linq.Extensions.Value<DateTime?>(property));
					item.set_StringValue(Newtonsoft.Json.Linq.Extensions.Value<string>(property));
					return;
				}
				case 3:
				{
					item.set_DateTimeValue(Newtonsoft.Json.Linq.Extensions.Value<DateTime?>(property));
					item.set_StringValue(Newtonsoft.Json.Linq.Extensions.Value<string>(property));
					return;
				}
				case 6:
				{
					item.set_DoubleValue(Newtonsoft.Json.Linq.Extensions.Value<double?>(property));
					item.set_StringValue(Newtonsoft.Json.Linq.Extensions.Value<string>(property));
					return;
				}
				case 16:
				{
					V_1 = Newtonsoft.Json.Linq.Extensions.Value<string>(property);
					if (!StringExtension.IsBase64(V_1))
					{
						item.set_StringValue(V_1);
						return;
					}
					stackVariable40 = new Mix.Cms.Lib.ViewModels.MixMedias.UpdateViewModel();
					stackVariable40.set_Specificulture(this.get_Specificulture());
					stackVariable40.set_Status(2);
					stackVariable44 = new FileViewModel();
					stackVariable44.set_FileStream(V_1);
					stackVariable44.set_Extension(".png");
					stackVariable44.set_Filename(Guid.NewGuid().ToString());
					stackVariable44.set_FileFolder("Attributes");
					stackVariable40.set_MediaFile(stackVariable44);
					V_3 = ((ViewModelBase<MixCmsContext, MixMedia, Mix.Cms.Lib.ViewModels.MixMedias.UpdateViewModel>)stackVariable40).SaveModel(true, null, null);
					if (!V_3.get_IsSucceed())
					{
						break;
					}
					item.set_StringValue(V_3.get_Data().get_FullPath());
					this.get_Obj().set_Item(item.get_AttributeFieldName(), JToken.op_Implicit(item.get_StringValue()));
					return;
				}
				case 18:
				{
					item.set_BooleanValue(Newtonsoft.Json.Linq.Extensions.Value<bool?>(property));
					item.set_StringValue(Newtonsoft.Json.Linq.Extensions.Value<string>(property).ToLower());
					return;
				}
				case 22:
				{
					item.set_IntegerValue(Newtonsoft.Json.Linq.Extensions.Value<int?>(property));
					item.set_StringValue(Newtonsoft.Json.Linq.Extensions.Value<string>(property));
					return;
				}
				case 23:
				{
					item.set_StringValue(Newtonsoft.Json.Linq.Extensions.Value<string>(property));
					return;
				}
				default:
				{
					goto Label0;
				}
			}
			return;
		}

		private JProperty ParseValue(Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel item, MixCmsContext context, IDbContextTransaction transaction)
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
					return new JProperty(item.get_AttributeFieldName(), null);
				}
				default:
				{
					goto Label0;
				}
			}
		}

		public override RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel> SaveModel(bool isSaveSubModels = false, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			stackVariable4 = this.SaveModel(isSaveSubModels, _context, _transaction);
			if (stackVariable4.get_IsSucceed())
			{
				this.ParseData(null, null);
			}
			return stackVariable4;
		}

		public override async Task<RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel>> SaveModelAsync(bool isSaveSubModels = false, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0.u003cu003e4__this = this;
			V_0.isSaveSubModels = isSaveSubModels;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel.u003cSaveModelAsyncu003ed__76>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public static async Task<RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel>> SaveObjectAsync(JObject data, string attributeSetName)
		{
			V_0.data = data;
			V_0.attributeSetName = attributeSetName;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel.u003cSaveObjectAsyncu003ed__87>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private async Task<RepositoryResponse<bool>> SaveRefDataAsync(MixAttributeSetData parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.parent = parent;
			V_0.context = context;
			V_0.transaction = transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel.u003cSaveRefDataAsyncu003ed__80>(ref V_0);
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
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel.u003cSaveRelatedDataAsyncu003ed__81>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public override async Task<RepositoryResponse<bool>> SaveSubModelsAsync(MixAttributeSetData parent, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.parent = parent;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel.u003cSaveSubModelsAsyncu003ed__78>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		private async Task<RepositoryResponse<bool>> SaveValues(MixAttributeSetData parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.parent = parent;
			V_0.context = context;
			V_0.transaction = transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel.u003cSaveValuesu003ed__79>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}