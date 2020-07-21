using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixAttributeSetValues;
using Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas;
using Mix.Domain.Core.Models;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels.MixAttributeSetDatas
{
	public class ReadMvcViewModel : ViewModelBase<MixCmsContext, MixAttributeSetData, Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ReadMvcViewModel>
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

		public JObject Data
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

		public ReadMvcViewModel()
		{
			base();
			return;
		}

		public ReadMvcViewModel(MixAttributeSetData model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			base(model, _context, _transaction);
			return;
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.set_Data(new JObject());
			stackVariable2 = ViewModelBase<MixCmsContext, MixAttributeSetValue, Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ReadMvcViewModel>.Repository;
			V_0 = Expression.Parameter(Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ReadMvcViewModel::ExpandView(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Exception in: System.Void ExpandView(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		private JProperty ParseValue(Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ReadMvcViewModel item)
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
					V_2 = item.get_DataNavs().GetEnumerator();
					try
					{
						while (V_2.MoveNext())
						{
							V_3 = V_2.get_Current();
							V_0.Add(V_3.get_Data().get_Data());
						}
					}
					finally
					{
						V_2.Dispose();
					}
					return new JProperty(item.get_AttributeFieldName(), V_0);
				}
				default:
				{
					goto Label0;
				}
			}
		}
	}
}