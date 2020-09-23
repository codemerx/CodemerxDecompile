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
	public class ImportViewModel : ViewModelBase<MixCmsContext, MixAttributeSetData, Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel>
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

		[JsonIgnore]
		public List<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ImportViewModel> Values
		{
			get;
			set;
		}

		public ImportViewModel()
		{
			this.u003cRelatedDatau003ek__BackingField = new List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel>();
			base();
			return;
		}

		public ImportViewModel(MixAttributeSetData model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.u003cRelatedDatau003ek__BackingField = new List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel>();
			base(model, _context, _transaction);
			return;
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			stackVariable1 = ViewModelBase<MixCmsContext, MixAttributeSetValue, Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ImportViewModel>.Repository;
			V_0 = Expression.Parameter(Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel::ExpandView(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Void ExpandView(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		public override void GenerateCache(MixAttributeSetData model, Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.ParseData();
			this.GenerateCache(model, view, _context, _transaction);
			return;
		}

		private void ParseData()
		{
			stackVariable1 = new JObject();
			stackVariable1.Add(new JProperty("id", this.get_Id()));
			stackVariable1.Add(new JProperty("createdDateTime", (object)this.get_CreatedDateTime()));
			this.set_Data(stackVariable1);
			stackVariable12 = this.get_Values();
			stackVariable13 = Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel.u003cu003ec.u003cu003e9__69_0;
			if (stackVariable13 == null)
			{
				dummyVar0 = stackVariable13;
				stackVariable13 = new Func<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ImportViewModel, int>(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel.u003cu003ec.u003cu003e9.u003cParseDatau003eb__69_0);
				Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel.u003cu003ec.u003cu003e9__69_0 = stackVariable13;
			}
			V_0 = stackVariable12.OrderBy<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ImportViewModel, int>(stackVariable13).GetEnumerator();
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
			return;
		}

		public override MixAttributeSetData ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (string.IsNullOrEmpty(this.get_Id()))
			{
				this.set_Id(Guid.NewGuid().ToString());
				this.set_CreatedDateTime(DateTime.get_UtcNow());
			}
			stackVariable5 = this.get_Fields();
			if (stackVariable5 == null)
			{
				dummyVar0 = stackVariable5;
				stackVariable5 = new List<Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel>();
			}
			this.set_Fields(stackVariable5);
			this.set_Values(new List<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ImportViewModel>());
			V_1 = this.get_Fields().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					stackVariable15 = new Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ImportViewModel();
					stackVariable15.set_AttributeFieldId(V_2.get_Id());
					stackVariable15.set_AttributeFieldName(V_2.get_Name());
					stackVariable15.set_StringValue(V_2.get_DefaultValue());
					stackVariable15.set_Priority(V_2.get_Priority());
					stackVariable15.set_Field(V_2);
					V_3 = stackVariable15;
					V_3.set_Priority(V_2.get_Priority());
					V_3.set_DataType(V_2.get_DataType());
					V_3.set_AttributeSetName(V_2.get_AttributeSetName());
					if (this.get_Data().get_Item(V_3.get_AttributeFieldName()) != null && V_3.get_Field().get_DataType() != 23)
					{
						this.ParseModelValue(this.get_Data().get_Item(V_3.get_AttributeFieldName()), V_3);
					}
					this.get_Values().Add(V_3);
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			return this.ParseModel(_context, _transaction);
		}

		private void ParseModelValue(JToken property, Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ImportViewModel item)
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
					this.get_Data().set_Item(item.get_AttributeFieldName(), JToken.op_Implicit(item.get_StringValue()));
					return;
				}
				case 18:
				{
					item.set_BooleanValue(Newtonsoft.Json.Linq.Extensions.Value<bool?>(property));
					stackVariable68 = item;
					stackVariable70 = Newtonsoft.Json.Linq.Extensions.Value<string>(property);
					if (stackVariable70 != null)
					{
						stackVariable71 = stackVariable70.ToLower();
					}
					else
					{
						dummyVar0 = stackVariable70;
						stackVariable71 = null;
					}
					stackVariable68.set_StringValue(stackVariable71);
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

		private JProperty ParseValue(Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ImportViewModel item)
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

		public override async Task<RepositoryResponse<bool>> SaveSubModelsAsync(MixAttributeSetData parent, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			V_0.u003cu003e4__this = this;
			V_0.parent = parent;
			V_0._context = _context;
			V_0._transaction = _transaction;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel.u003cSaveSubModelsAsyncu003ed__65>(ref V_0);
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
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel.u003cSaveValuesu003ed__66>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}