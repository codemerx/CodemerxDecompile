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
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixAttributeSetDatas
{
	public class ExportViewModel : ViewModelBase<MixCmsContext, MixAttributeSetData, ExportViewModel>
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

		[JsonIgnore]
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

		[JsonProperty("priority")]
		public int Priority
		{
			get;
			set;
		}

		public List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel> RefData
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

		[JsonIgnore]
		public List<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel> Values
		{
			get;
			set;
		}

		public ExportViewModel()
		{
			this.u003cRefDatau003ek__BackingField = new List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel>();
			this.u003cRelatedDatau003ek__BackingField = new List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel>();
			base();
			return;
		}

		public ExportViewModel(MixAttributeSetData model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.u003cRefDatau003ek__BackingField = new List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel>();
			this.u003cRelatedDatau003ek__BackingField = new List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel>();
			base(model, _context, _transaction);
			return;
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			stackVariable0 = ViewModelBase<MixCmsContext, MixAttributeSetValue, Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel>.Repository;
			V_1 = Expression.Parameter(Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ExportViewModel::ExpandView(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Void ExpandView(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public static Task<RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel>>> FilterByValueAsync(string culture, string attributeSetName, Dictionary<string, StringValues> queryDictionary, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			V_0 = new ExportViewModel.u003cu003ec__DisplayClass70_0();
			V_0.culture = culture;
			V_0.attributeSetName = attributeSetName;
			UnitOfWorkHelper<MixCmsContext>.InitTransaction(_context, _transaction, ref V_1, ref V_2, ref V_3);
			try
			{
				try
				{
					V_4 = new ExportViewModel.u003cu003ec__DisplayClass70_1();
					V_4.CSu0024u003cu003e8__locals1 = V_0;
					V_7 = Expression.Parameter(Type.GetTypeFromHandle(// 
					// Current member / type: System.Threading.Tasks.Task`1<Mix.Domain.Core.ViewModels.RepositoryResponse`1<System.Collections.Generic.List`1<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel>>> Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ExportViewModel::FilterByValueAsync(System.String,System.String,System.Collections.Generic.Dictionary`2<System.String,Microsoft.Extensions.Primitives.StringValues>,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
					// Exception in: System.Threading.Tasks.Task<Mix.Domain.Core.ViewModels.RepositoryResponse<System.Collections.Generic.List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel>>> FilterByValueAsync(System.String,System.String,System.Collections.Generic.Dictionary<System.String,Microsoft.Extensions.Primitives.StringValues>,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
					// Specified method is not supported.
					// 
					// mailto: JustDecompilePublicFeedback@telerik.com


		private void ParseData()
		{
			this.set_Data(new JObject());
			stackVariable3 = this.get_Values();
			stackVariable4 = ExportViewModel.u003cu003ec.u003cu003e9__71_0;
			if (stackVariable4 == null)
			{
				dummyVar0 = stackVariable4;
				stackVariable4 = new Func<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel, int>(ExportViewModel.u003cu003ec.u003cu003e9.u003cParseDatau003eb__71_0);
				ExportViewModel.u003cu003ec.u003cu003e9__71_0 = stackVariable4;
			}
			V_0 = stackVariable3.OrderBy<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel, int>(stackVariable4).GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					V_1.set_AttributeFieldName(V_1.get_Field().get_Name());
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
			this.get_Data().Add(new JProperty("createdDateTime", (object)this.get_CreatedDateTime()));
			return;
		}

		public override MixAttributeSetData ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (string.IsNullOrEmpty(this.get_Id()))
			{
				this.set_Id(Guid.NewGuid().ToString());
				this.set_CreatedDateTime(DateTime.get_UtcNow());
				stackVariable221 = ViewModelBase<MixCmsContext, MixAttributeSetData, ExportViewModel>.Repository;
				V_1 = Expression.Parameter(Type.GetTypeFromHandle(// 
				// Current member / type: Mix.Cms.Lib.Models.Cms.MixAttributeSetData Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ExportViewModel::ParseModel(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Exception in: Mix.Cms.Lib.Models.Cms.MixAttributeSetData ParseModel(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com


		private void ParseModelValue(JToken property, Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel item)
		{
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
					V_0 = Newtonsoft.Json.Linq.Extensions.Value<string>(property);
					if (!StringExtension.IsBase64(V_0))
					{
						item.set_StringValue(V_0);
						return;
					}
					stackVariable37 = new Mix.Cms.Lib.ViewModels.MixMedias.UpdateViewModel();
					stackVariable37.set_Specificulture(this.get_Specificulture());
					stackVariable37.set_Status(2);
					stackVariable41 = new FileViewModel();
					stackVariable41.set_FileStream(V_0);
					stackVariable41.set_Extension(".png");
					stackVariable41.set_Filename(Guid.NewGuid().ToString());
					stackVariable41.set_FileFolder("Attributes");
					stackVariable37.set_MediaFile(stackVariable41);
					V_2 = ((ViewModelBase<MixCmsContext, MixMedia, Mix.Cms.Lib.ViewModels.MixMedias.UpdateViewModel>)stackVariable37).SaveModel(true, null, null);
					if (!V_2.get_IsSucceed())
					{
						break;
					}
					item.set_StringValue(V_2.get_Data().get_FullPath());
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
	}
}