using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Primitives;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Helpers;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels;
using Mix.Cms.Lib.ViewModels.MixAttributeFields;
using Mix.Cms.Lib.ViewModels.MixAttributeSetValues;
using Mix.Cms.Lib.ViewModels.MixMedias;
using Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas;
using Mix.Common.Helper;
using Mix.Domain.Core.Models;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Mix.Heart.Extensions;
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

		public List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel> RefData { get; set; } = new List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel>();

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
		public List<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel> Values
		{
			get;
			set;
		}

		public ExportViewModel()
		{
		}

		public ExportViewModel(MixAttributeSetData model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel>> modelListBy = ViewModelBase<MixCmsContext, MixAttributeSetValue, Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel>.Repository.GetModelListBy((MixAttributeSetValue a) => a.DataId == this.Id && a.Specificulture == this.Specificulture, _context, _transaction);
			if (modelListBy.get_IsSucceed())
			{
				this.Values = (
					from a in modelListBy.get_Data()
					orderby a.Priority
					select a).ToList<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel>();
				this.ParseData();
			}
		}

		public static Task<RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel>>> FilterByValueAsync(string culture, string attributeSetName, Dictionary<string, StringValues> queryDictionary, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			ExportViewModel.u003cu003ec__DisplayClass70_0 variable = null;
			MixCmsContext mixCmsContext = null;
			IDbContextTransaction dbContextTransaction = null;
			bool flag = false;
			ExportViewModel.u003cu003ec__DisplayClass70_1 variable1 = null;
			Task<RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel>>> task;
			UnitOfWorkHelper<MixCmsContext>.InitTransaction(_context, _transaction, ref mixCmsContext, ref dbContextTransaction, ref flag);
			try
			{
				try
				{
					Expression<Func<MixAttributeSetValue, bool>> specificulture = (MixAttributeSetValue m) => m.Specificulture == variable.culture;
					List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel> updateViewModels = new List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel>();
					foreach (KeyValuePair<string, StringValues> keyValuePair in queryDictionary)
					{
						Expression<Func<MixAttributeSetValue, bool>> expression = (MixAttributeSetValue m) => m.Specificulture == variable1.CSu0024u003cu003e8__locals1.culture && m.AttributeSetName == variable1.CSu0024u003cu003e8__locals1.attributeSetName && m.AttributeFieldName == keyValuePair.Key && m.StringValue.Contains((string)keyValuePair.Value);
						specificulture = ODataHelper<MixAttributeSetValue>.CombineExpression<MixAttributeSetValue>(specificulture, expression, 1, "model");
					}
					IQueryable<MixAttributeSetValue> mixAttributeSetValues = mixCmsContext.MixAttributeSetValue.Where<MixAttributeSetValue>(specificulture);
					foreach (MixAttributeSetData mixAttributeSetDatum in 
						from m in mixCmsContext.MixAttributeSetData
						where mixAttributeSetValues.Any<MixAttributeSetValue>((MixAttributeSetValue q) => q.DataId == m.Id) && m.Specificulture == variable.culture
						select m)
					{
						updateViewModels.Add(new Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel(mixAttributeSetDatum, mixCmsContext, dbContextTransaction));
					}
					RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel>> repositoryResponse = new RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel>>();
					repositoryResponse.set_IsSucceed(true);
					repositoryResponse.set_Data(updateViewModels);
					task = Task.FromResult<RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel>>>(repositoryResponse);
				}
				catch (Exception exception)
				{
					task = Task.FromResult<RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel>>>(UnitOfWorkHelper<MixCmsContext>.HandleException<List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel>>(exception, flag, dbContextTransaction));
				}
			}
			finally
			{
				if (flag)
				{
					RelationalDatabaseFacadeExtensions.CloseConnection(mixCmsContext.get_Database());
					dbContextTransaction.Dispose();
					mixCmsContext.Dispose();
				}
			}
			return task;
		}

		private void ParseData()
		{
			this.Data = new JObject();
			foreach (Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel name in 
				from v in this.Values
				orderby v.Priority
				select v)
			{
				name.AttributeFieldName = name.Field.Name;
				this.Data.Add(this.ParseValue(name));
			}
			this.Data.Add(new JProperty("createdDateTime", (object)this.CreatedDateTime));
		}

		public override MixAttributeSetData ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			string str;
			string name;
			if (string.IsNullOrEmpty(this.Id))
			{
				this.Id = Guid.NewGuid().ToString();
				this.CreatedDateTime = DateTime.UtcNow;
				this.Priority = ViewModelBase<MixCmsContext, MixAttributeSetData, ExportViewModel>.Repository.Count((MixAttributeSetData m) => m.AttributeSetName == this.AttributeSetName && m.Specificulture == this.Specificulture, _context, _transaction).get_Data() + 1;
			}
			this.Values = this.Values ?? (
				from a in ViewModelBase<MixCmsContext, MixAttributeSetValue, Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel>.Repository.GetModelListBy((MixAttributeSetValue a) => a.DataId == this.Id && a.Specificulture == this.Specificulture, _context, _transaction).get_Data()
				orderby a.Priority
				select a).ToList<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel>();
			this.Fields = ViewModelBase<MixCmsContext, MixAttributeField, Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel>.Repository.GetModelListBy((MixAttributeField f) => f.AttributeSetId == this.AttributeSetId, _context, _transaction).get_Data();
			if (string.IsNullOrEmpty(this.AttributeSetName))
			{
				MixAttributeSet mixAttributeSet = _context.MixAttributeSet.First<MixAttributeSet>((MixAttributeSet m) => m.Id == this.AttributeSetId);
				if (mixAttributeSet != null)
				{
					name = mixAttributeSet.Name;
				}
				else
				{
					name = null;
				}
				this.AttributeSetName = name;
			}
			foreach (Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel updateViewModel in 
				from f in this.Fields
				orderby f.Priority
				select f)
			{
				Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel priority = this.Values.FirstOrDefault<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel>((Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel v) => v.AttributeFieldId == updateViewModel.Id);
				if (priority == null)
				{
					priority = new Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel(new MixAttributeSetValue()
					{
						AttributeFieldId = updateViewModel.Id,
						AttributeFieldName = updateViewModel.Name
					}, _context, _transaction)
					{
						StringValue = updateViewModel.DefaultValue,
						Priority = updateViewModel.Priority,
						Field = updateViewModel
					};
					this.Values.Add(priority);
				}
				priority.Priority = updateViewModel.Priority;
				priority.AttributeSetName = this.AttributeSetName;
				if (this.Data.get_Item(priority.AttributeFieldName) == null)
				{
					this.Data.Add(this.ParseValue(priority));
				}
				else if (priority.Field.DataType != MixEnums.MixDataType.Reference)
				{
					this.ParseModelValue(this.Data.get_Item(priority.AttributeFieldName), priority);
				}
				else
				{
					foreach (JObject jObject in Newtonsoft.Json.Linq.Extensions.Value<JArray>(this.Data.get_Item(priority.AttributeFieldName)))
					{
						JToken item = jObject.get_Item("id");
						if (item != null)
						{
							str = Newtonsoft.Json.Linq.Extensions.Value<string>(item);
						}
						else
						{
							str = null;
						}
						if (!string.IsNullOrEmpty(str))
						{
							continue;
						}
						this.RefData.Add(new Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.UpdateViewModel()
						{
							Specificulture = this.Specificulture,
							AttributeSetId = updateViewModel.ReferenceId.Value,
							Data = jObject
						});
					}
				}
			}
			return base.ParseModel(_context, _transaction);
		}

		private void ParseModelValue(JToken property, Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel item)
		{
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
					return;
				}
				case MixEnums.MixDataType.Boolean:
				{
					item.BooleanValue = Newtonsoft.Json.Linq.Extensions.Value<bool?>(property);
					item.StringValue = Newtonsoft.Json.Linq.Extensions.Value<string>(property).ToLower();
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

		private JProperty ParseValue(Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel item)
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
	}
}