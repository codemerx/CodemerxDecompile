using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels.MixAttributeFields;
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
using System.Diagnostics;
using System.Linq;
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
				if (!(this.AttributeSetName == "sys_navigation") || this.Data == null)
				{
					return null;
				}
				return this.Data.ToObject<Navigation>();
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
		}

		public NavigationViewModel(MixAttributeSetData model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			JObject jObject = new JObject();
			jObject.Add(new JProperty("id", this.Id));
			this.Data = jObject;
			this.Values = (
				from a in ViewModelBase<MixCmsContext, MixAttributeSetValue, Mix.Cms.Lib.ViewModels.MixAttributeSetValues.NavigationViewModel>.Repository.GetModelListBy((MixAttributeSetValue a) => a.DataId == this.Id && a.Specificulture == this.Specificulture, _context, _transaction).get_Data()
				orderby a.Priority
				select a).ToList<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.NavigationViewModel>();
			foreach (Mix.Cms.Lib.ViewModels.MixAttributeSetValues.NavigationViewModel navigationViewModel in 
				from v in this.Values
				orderby v.Priority
				select v)
			{
				this.Data.Add(this.ParseValue(navigationViewModel));
			}
		}

		private void ParseModelValue(JToken property, Mix.Cms.Lib.ViewModels.MixAttributeSetValues.NavigationViewModel item)
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
					item.StringValue = Newtonsoft.Json.Linq.Extensions.Value<string>(property);
					item.StringValue = Newtonsoft.Json.Linq.Extensions.Value<string>(property);
					return;
				}
				case MixEnums.MixDataType.DateTime:
				{
					item.DateTimeValue = Newtonsoft.Json.Linq.Extensions.Value<DateTime?>(property);
					item.StringValue = Newtonsoft.Json.Linq.Extensions.Value<string>(property);
					return;
				}
				case MixEnums.MixDataType.Date:
				{
					item.DateTimeValue = Newtonsoft.Json.Linq.Extensions.Value<DateTime?>(property);
					item.StringValue = Newtonsoft.Json.Linq.Extensions.Value<string>(property);
					return;
				}
				case MixEnums.MixDataType.Time:
				{
					item.DateTimeValue = Newtonsoft.Json.Linq.Extensions.Value<DateTime?>(property);
					item.StringValue = Newtonsoft.Json.Linq.Extensions.Value<string>(property);
					return;
				}
				case MixEnums.MixDataType.Double:
				{
					item.DoubleValue = Newtonsoft.Json.Linq.Extensions.Value<double?>(property);
					item.StringValue = Newtonsoft.Json.Linq.Extensions.Value<string>(property);
					return;
				}
				case MixEnums.MixDataType.Boolean:
				{
					item.BooleanValue = Newtonsoft.Json.Linq.Extensions.Value<bool?>(property);
					item.StringValue = Newtonsoft.Json.Linq.Extensions.Value<string>(property);
					return;
				}
				case MixEnums.MixDataType.Integer:
				{
					item.IntegerValue = Newtonsoft.Json.Linq.Extensions.Value<int?>(property);
					item.StringValue = Newtonsoft.Json.Linq.Extensions.Value<string>(property);
					return;
				}
				case MixEnums.MixDataType.Reference:
				{
					item.StringValue = Newtonsoft.Json.Linq.Extensions.Value<string>(property);
					return;
				}
				default:
				{
					item.StringValue = Newtonsoft.Json.Linq.Extensions.Value<string>(property);
					item.StringValue = Newtonsoft.Json.Linq.Extensions.Value<string>(property);
					return;
				}
			}
		}

		private JProperty ParseValue(Mix.Cms.Lib.ViewModels.MixAttributeSetValues.NavigationViewModel item)
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
					foreach (Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.NavigationViewModel navigationViewModel in 
						from d in item.DataNavs
						orderby d.Priority
						select d)
					{
						navigationViewModel.Data.Data.Add(new JProperty("data", navigationViewModel.Data.Data));
						jArray.Add(navigationViewModel.Data.Data);
					}
					return new JProperty(item.AttributeFieldName, jArray);
				}
				default:
				{
					return new JProperty(item.AttributeFieldName, item.StringValue);
				}
			}
		}

		public override async Task<RepositoryResponse<bool>> SaveSubModelsAsync(MixAttributeSetData parent, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.NavigationViewModel.u003cSaveSubModelsAsyncu003ed__61 variable = new Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.NavigationViewModel.u003cSaveSubModelsAsyncu003ed__61();
			variable.u003cu003e4__this = this;
			variable.parent = parent;
			variable._context = _context;
			variable._transaction = _transaction;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.NavigationViewModel.u003cSaveSubModelsAsyncu003ed__61>(ref variable);
			return variable.u003cu003et__builder.Task;
		}
	}
}