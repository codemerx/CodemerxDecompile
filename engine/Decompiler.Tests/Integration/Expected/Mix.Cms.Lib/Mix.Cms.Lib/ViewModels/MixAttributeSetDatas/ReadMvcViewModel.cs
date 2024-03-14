using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixAttributeSetValues;
using Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas;
using Mix.Domain.Core.Models;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
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
		}

		public ReadMvcViewModel(MixAttributeSetData model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.Data = new JObject();
			List<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ReadMvcViewModel> list = (
				from a in ViewModelBase<MixCmsContext, MixAttributeSetValue, Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ReadMvcViewModel>.Repository.GetModelListBy((MixAttributeSetValue a) => a.DataId == this.Id && a.Specificulture == this.Specificulture, _context, _transaction).get_Data()
				orderby a.Priority
				select a).ToList<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ReadMvcViewModel>();
			this.Data.Add(new JProperty("id", this.Id));
			foreach (Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ReadMvcViewModel readMvcViewModel in 
				from v in list
				orderby v.Priority
				select v)
			{
				this.Data.Add(this.ParseValue(readMvcViewModel));
			}
		}

		private JProperty ParseValue(Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ReadMvcViewModel item)
		{
			switch (item.DataType)
			{
				case MixEnums.MixDataType.Custom:
				case MixEnums.MixDataType.Duration:
				case MixEnums.MixDataType.PhoneNumber:
				case MixEnums.MixDataType.Text:
				case MixEnums.MixDataType.Html:
				case MixEnums.MixDataType.MultilineText:
				case MixEnums.MixDataType.EmailAddress:
				case MixEnums.MixDataType.Password:
				case MixEnums.MixDataType.Url:
				case MixEnums.MixDataType.ImageUrl:
				case MixEnums.MixDataType.CreditCard:
				case MixEnums.MixDataType.PostalCode:
				case MixEnums.MixDataType.Upload:
				case MixEnums.MixDataType.Color:
				case MixEnums.MixDataType.Icon:
				case MixEnums.MixDataType.VideoYoutube:
				case MixEnums.MixDataType.TuiEditor:
				{
					return new JProperty(item.AttributeFieldName, item.StringValue);
				}
				case MixEnums.MixDataType.DateTime:
				{
					return new JProperty(item.AttributeFieldName, (object)item.DateTimeValue);
				}
				case MixEnums.MixDataType.Date:
				{
					return new JProperty(item.AttributeFieldName, (object)item.DateTimeValue);
				}
				case MixEnums.MixDataType.Time:
				{
					return new JProperty(item.AttributeFieldName, (object)item.DateTimeValue);
				}
				case MixEnums.MixDataType.Double:
				{
					return new JProperty(item.AttributeFieldName, (object)item.DoubleValue);
				}
				case MixEnums.MixDataType.Boolean:
				{
					return new JProperty(item.AttributeFieldName, (object)item.BooleanValue);
				}
				case MixEnums.MixDataType.Integer:
				{
					return new JProperty(item.AttributeFieldName, (object)item.IntegerValue);
				}
				case MixEnums.MixDataType.Reference:
				{
					JArray jArray = new JArray();
					foreach (Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.ReadMvcViewModel dataNav in item.DataNavs)
					{
						jArray.Add(dataNav.Data.Data);
					}
					return new JProperty(item.AttributeFieldName, jArray);
				}
				default:
				{
					return new JProperty(item.AttributeFieldName, item.StringValue);
				}
			}
		}
	}
}