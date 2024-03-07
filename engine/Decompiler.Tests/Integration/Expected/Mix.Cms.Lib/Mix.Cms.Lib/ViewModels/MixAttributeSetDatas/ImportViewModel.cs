using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels;
using Mix.Cms.Lib.ViewModels.MixAttributeFields;
using Mix.Cms.Lib.ViewModels.MixAttributeSetValues;
using Mix.Cms.Lib.ViewModels.MixMedias;
using Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas;
using Mix.Domain.Core.Models;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Mix.Heart.Extensions;
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
		public List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel> RelatedData { get; set; } = new List<Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas.UpdateViewModel>();

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
		}

		public ImportViewModel(MixAttributeSetData model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.Values = (
				from a in ViewModelBase<MixCmsContext, MixAttributeSetValue, Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ImportViewModel>.Repository.GetModelListBy((MixAttributeSetValue a) => a.DataId == this.Id && a.Specificulture == this.Specificulture, _context, _transaction).get_Data()
				orderby a.Priority
				select a).ToList<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ImportViewModel>();
			this.ParseData();
		}

		public override void GenerateCache(MixAttributeSetData model, Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.ImportViewModel view, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.ParseData();
			base.GenerateCache(model, view, _context, _transaction);
		}

		private void ParseData()
		{
			JObject jObject = new JObject();
			jObject.Add(new JProperty("id", this.Id));
			jObject.Add(new JProperty("createdDateTime", (object)this.CreatedDateTime));
			this.Data = jObject;
			foreach (Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ImportViewModel name in 
				from v in this.Values
				orderby v.Priority
				select v)
			{
				name.AttributeFieldName = name.Field.Name;
				this.Data.Add(this.ParseValue(name));
			}
		}

		public override MixAttributeSetData ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (string.IsNullOrEmpty(this.Id))
			{
				this.Id = Guid.NewGuid().ToString();
				this.CreatedDateTime = DateTime.UtcNow;
			}
			this.Fields = this.Fields ?? new List<Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel>();
			this.Values = new List<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ImportViewModel>();
			foreach (Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel field in this.Fields)
			{
				Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ImportViewModel importViewModel = new Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ImportViewModel()
				{
					AttributeFieldId = field.Id,
					AttributeFieldName = field.Name,
					StringValue = field.DefaultValue,
					Priority = field.Priority,
					Field = field
				};
				importViewModel.Priority = field.Priority;
				importViewModel.DataType = field.DataType;
				importViewModel.AttributeSetName = field.AttributeSetName;
				if (this.Data.get_Item(importViewModel.AttributeFieldName) != null && importViewModel.Field.DataType != MixEnums.MixDataType.Reference)
				{
					this.ParseModelValue(this.Data.get_Item(importViewModel.AttributeFieldName), importViewModel);
				}
				this.Values.Add(importViewModel);
			}
			return base.ParseModel(_context, _transaction);
		}

		private void ParseModelValue(JToken property, Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ImportViewModel item)
		{
			string lower;
			switch (item.Field.DataType)
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
				case MixEnums.MixDataType.Color:
				case MixEnums.MixDataType.Icon:
				case MixEnums.MixDataType.VideoYoutube:
				case MixEnums.MixDataType.TuiEditor:
				{
					item.StringValue = Newtonsoft.Json.Linq.Extensions.Value<string>(property);
					break;
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
				case MixEnums.MixDataType.Upload:
				{
					string str = Newtonsoft.Json.Linq.Extensions.Value<string>(property);
					if (!StringExtension.IsBase64(str))
					{
						item.StringValue = str;
						return;
					}
					RepositoryResponse<Mix.Cms.Lib.ViewModels.MixMedias.UpdateViewModel> repositoryResponse = ((ViewModelBase<MixCmsContext, MixMedia, Mix.Cms.Lib.ViewModels.MixMedias.UpdateViewModel>)(new Mix.Cms.Lib.ViewModels.MixMedias.UpdateViewModel()
					{
						Specificulture = this.Specificulture,
						Status = MixEnums.MixContentStatus.Published,
						MediaFile = new FileViewModel()
						{
							FileStream = str,
							Extension = ".png",
							Filename = Guid.NewGuid().ToString(),
							FileFolder = "Attributes"
						}
					})).SaveModel(true, null, null);
					if (!repositoryResponse.get_IsSucceed())
					{
						break;
					}
					item.StringValue = repositoryResponse.get_Data().FullPath;
					this.Data.set_Item(item.AttributeFieldName, item.StringValue);
					return;
				}
				case MixEnums.MixDataType.Boolean:
				{
					item.BooleanValue = Newtonsoft.Json.Linq.Extensions.Value<bool?>(property);
					Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ImportViewModel importViewModel = item;
					string str1 = Newtonsoft.Json.Linq.Extensions.Value<string>(property);
					if (str1 != null)
					{
						lower = str1.ToLower();
					}
					else
					{
						lower = null;
					}
					importViewModel.StringValue = lower;
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
					goto case MixEnums.MixDataType.TuiEditor;
				}
			}
		}

		private JProperty ParseValue(Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ImportViewModel item)
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
					return new JProperty(item.AttributeFieldName, new JArray());
				}
				default:
				{
					return new JProperty(item.AttributeFieldName, item.StringValue);
				}
			}
		}

		public override async Task<RepositoryResponse<bool>> SaveSubModelsAsync(MixAttributeSetData parent, MixCmsContext _context, IDbContextTransaction _transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			if (repositoryResponse1.get_IsSucceed())
			{
				ViewModelHelper.HandleResult<bool>(await this.SaveValues(parent, _context, _transaction), ref repositoryResponse1);
			}
			return repositoryResponse1;
		}

		private async Task<RepositoryResponse<bool>> SaveValues(MixAttributeSetData parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			foreach (Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ImportViewModel value in this.Values)
			{
				if (!repositoryResponse1.get_IsSucceed())
				{
					break;
				}
				if (!this.Fields.Any<Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel>((Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel f) => f.Id == value.AttributeFieldId))
				{
					ViewModelHelper.HandleResult<MixAttributeSetValue>(await value.RemoveModelAsync(false, context, transaction), ref repositoryResponse1);
				}
				else
				{
					value.Priority = value.Field.Priority;
					value.DataId = parent.Id;
					value.Specificulture = parent.Specificulture;
					ViewModelHelper.HandleResult<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.ImportViewModel>(await value.SaveModelAsync(false, context, transaction), ref repositoryResponse1);
				}
			}
			return repositoryResponse1;
		}
	}
}