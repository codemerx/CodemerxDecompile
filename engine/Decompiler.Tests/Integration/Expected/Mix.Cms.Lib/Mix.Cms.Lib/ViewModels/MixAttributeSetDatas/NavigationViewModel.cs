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
	public class NavigationViewModel : ViewModelBase<MixCmsContext, MixAttributeSetData, Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.NavigationViewModel>
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
		public List<Mix.Cms.Lib.ViewModels.MixAttributeFields.ReadViewModel> Fields
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

		[JsonProperty("nav")]
		public Navigation Nav
		{
			get
			{
				if (!string.op_Equality(this.get_AttributeSetName(), "sys_navigation") || this.get_Data() == null)
				{
					return null;
				}
				return this.get_Data().ToObject<Navigation>();
			}
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

		[JsonIgnore]
		public List<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.NavigationViewModel> Values
		{
			get;
			set;
		}

		public NavigationViewModel()
		{
			base();
			return;
		}

		public NavigationViewModel(MixAttributeSetData model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			base(model, _context, _transaction);
			return;
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			stackVariable1 = new JObject();
			stackVariable1.Add(new JProperty("id", this.get_Id()));
			this.set_Data(stackVariable1);
			stackVariable7 = ViewModelBase<MixCmsContext, MixAttributeSetValue, Mix.Cms.Lib.ViewModels.MixAttributeSetValues.NavigationViewModel>.Repository;
			V_0 = Expression.Parameter(Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.NavigationViewModel::ExpandView(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Void ExpandView(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		private void ParseModelValue(JToken property, Mix.Cms.Lib.ViewModels.MixAttributeSetValues.NavigationViewModel item)
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
				Label1:
					item.set_StringValue(Newtonsoft.Json.Linq.Extensions.Value<string>(property));
					goto Label0;
				}
				case 1:
				{
					item.set_DateTimeValue(Newtonsoft.Json.Linq.Extensions.Value<DateTime?>(property));
					goto Label0;
				}
				case 2:
				{
					item.set_DateTimeValue(Newtonsoft.Json.Linq.Extensions.Value<DateTime?>(property));
					goto Label0;
				}
				case 3:
				{
					item.set_DateTimeValue(Newtonsoft.Json.Linq.Extensions.Value<DateTime?>(property));
					goto Label0;
				}
				case 6:
				{
					item.set_DoubleValue(Newtonsoft.Json.Linq.Extensions.Value<double?>(property));
					goto Label0;
				}
				case 18:
				{
					item.set_BooleanValue(Newtonsoft.Json.Linq.Extensions.Value<bool?>(property));
					goto Label0;
				}
				case 22:
				{
					item.set_IntegerValue(Newtonsoft.Json.Linq.Extensions.Value<int?>(property));
					goto Label0;
				}
				case 23:
				{
				Label0:
					item.set_StringValue(Newtonsoft.Json.Linq.Extensions.Value<string>(property));
					return;
				}
				default:
				{
					goto Label1;
				}
			}
		}

		private JProperty ParseValue(Mix.Cms.Lib.ViewModels.MixAttributeSetValues.NavigationViewModel item)
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
					V_0 = new JArray();
					stackVariable46 = item.get_DataNavs();
					stackVariable47 = Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.NavigationViewModel.u003cu003ec.u003cu003e9__62_0;
					if (stackVariable47 == null)
					{
						dummyVar0 = stackVariable47;
						stackVariable47 = new Func<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.NavigationViewModel, int>(Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.NavigationViewModel.u003cu003ec.u003cu003e9.u003cParseValueu003eb__62_0);
						Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.NavigationViewModel.u003cu003ec.u003cu003e9__62_0 = stackVariable47;
					}
					V_2 = stackVariable46.OrderBy<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.NavigationViewModel, int>(stackVariable47).GetEnumerator();
					try
					{
						while (V_2.MoveNext())
						{
							V_3 = V_2.get_Current();
							V_3.get_Data().get_Data().Add(new JProperty("data", V_3.get_Data().get_Data()));
							V_0.Add(V_3.get_Data().get_Data());
						}
					}
					finally
					{
						if (V_2 != null)
						{
							V_2.Dispose();
						}
					}
					return new JProperty(item.get_AttributeFieldName(), V_0);
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
			V_0.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.NavigationViewModel.u003cSaveSubModelsAsyncu003ed__61>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}