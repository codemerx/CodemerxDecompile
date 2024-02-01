using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Primitives;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Helpers;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.Repositories;
using Mix.Cms.Lib.ViewModels;
using Mix.Cms.Lib.ViewModels.MixAttributeFields;
using Mix.Cms.Lib.ViewModels.MixAttributeSets;
using Mix.Cms.Lib.ViewModels.MixAttributeSetValues;
using Mix.Cms.Lib.ViewModels.MixMedias;
using Mix.Cms.Lib.ViewModels.MixRelatedAttributeDatas;
using Mix.Cms.Lib.ViewModels.MixTemplates;
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
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixAttributeSetDatas
{
	public class FormPortalViewModel : ViewModelBase<MixCmsContext, MixAttributeSetData, FormPortalViewModel>
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

		[JsonIgnore]
		public List<FormPortalViewModel> RefData { get; set; } = new List<FormPortalViewModel>();

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

		public FormPortalViewModel()
		{
		}

		public FormPortalViewModel(MixAttributeSetData model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.ParseData(_context, _transaction);
		}

		public static Task<RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel>>> FilterByValueAsync(string culture, string attributeSetName, Dictionary<string, StringValues> queryDictionary, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			FormPortalViewModel.u003cu003ec__DisplayClass84_0 variable = null;
			MixCmsContext mixCmsContext = null;
			IDbContextTransaction dbContextTransaction = null;
			bool flag = false;
			FormPortalViewModel.u003cu003ec__DisplayClass84_1 variable1 = null;
			Task<RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel>>> task;
			UnitOfWorkHelper<MixCmsContext>.InitTransaction(_context, _transaction, ref mixCmsContext, ref dbContextTransaction, ref flag);
			try
			{
				try
				{
					Expression<Func<MixAttributeSetValue, bool>> specificulture = (MixAttributeSetValue m) => m.Specificulture == variable.culture;
					List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel> formViewModels = new List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel>();
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
						formViewModels.Add(new Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel(mixAttributeSetDatum, mixCmsContext, dbContextTransaction));
					}
					RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel>> repositoryResponse = new RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel>>();
					repositoryResponse.set_IsSucceed(true);
					repositoryResponse.set_Data(formViewModels);
					task = Task.FromResult<RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel>>>(repositoryResponse);
				}
				catch (Exception exception)
				{
					task = Task.FromResult<RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel>>>(UnitOfWorkHelper<MixCmsContext>.HandleException<List<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel>>(exception, flag, dbContextTransaction));
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

		private void ParseData(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<List<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel>> modelListBy = ViewModelBase<MixCmsContext, MixAttributeSetValue, Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel>.Repository.GetModelListBy((MixAttributeSetValue a) => a.DataId == this.Id && a.Specificulture == this.Specificulture, _context, _transaction);
			if (modelListBy.get_IsSucceed())
			{
				this.Fields = ViewModelBase<MixCmsContext, MixAttributeField, Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel>.Repository.GetModelListBy((MixAttributeField f) => f.AttributeSetId == this.AttributeSetId, _context, _transaction).get_Data();
				this.Values = (
					from a in modelListBy.get_Data()
					orderby a.Priority
					select a).ToList<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel>();
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
						}, _context, _transaction);
						this.Values.Add(priority);
					}
					priority.Priority = updateViewModel.Priority;
					priority.DataType = updateViewModel.DataType;
					priority.Field = updateViewModel;
					priority.AttributeSetName = this.AttributeSetName;
				}
				this.Obj = new JObject(new JProperty("id", this.Id));
				foreach (Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel name in 
					from v in this.Values
					orderby v.Priority
					select v)
				{
					name.AttributeFieldName = name.Field.Name;
					this.Obj.Add(this.ParseValue(name, _context, _transaction));
				}
			}
		}

		public override MixAttributeSetData ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			string str;
			int id;
			string name;
			if (string.IsNullOrEmpty(this.Id))
			{
				this.Id = Guid.NewGuid().ToString();
				this.CreatedDateTime = DateTime.UtcNow;
				this.Priority = ViewModelBase<MixCmsContext, MixAttributeSetData, FormPortalViewModel>.Repository.Count((MixAttributeSetData m) => m.AttributeSetName == this.AttributeSetName && m.Specificulture == this.Specificulture, _context, _transaction).get_Data() + 1;
			}
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
			if (this.AttributeSetId == 0)
			{
				MixAttributeSet mixAttributeSet1 = _context.MixAttributeSet.First<MixAttributeSet>((MixAttributeSet m) => m.Name == this.AttributeSetName);
				if (mixAttributeSet1 != null)
				{
					id = mixAttributeSet1.Id;
				}
				else
				{
					id = 0;
				}
				this.AttributeSetId = id;
			}
			this.Values = this.Values ?? (
				from a in ViewModelBase<MixCmsContext, MixAttributeSetValue, Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel>.Repository.GetModelListBy((MixAttributeSetValue a) => a.DataId == this.Id && a.Specificulture == this.Specificulture, _context, _transaction).get_Data()
				orderby a.Priority
				select a).ToList<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel>();
			this.Fields = ViewModelBase<MixCmsContext, MixAttributeField, Mix.Cms.Lib.ViewModels.MixAttributeFields.UpdateViewModel>.Repository.GetModelListBy((MixAttributeField f) => f.AttributeSetId == this.AttributeSetId, _context, _transaction).get_Data();
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
				if (this.Obj.get_Item(priority.AttributeFieldName) == null)
				{
					this.Obj.Add(this.ParseValue(priority, _context, _transaction));
				}
				else if (priority.Field.DataType != MixEnums.MixDataType.Reference)
				{
					this.ParseModelValue(this.Obj.get_Item(priority.AttributeFieldName), priority);
				}
				else
				{
					JArray jArray = Newtonsoft.Json.Linq.Extensions.Value<JArray>(this.Obj.get_Item(priority.AttributeFieldName));
					if (jArray == null)
					{
						continue;
					}
					foreach (JObject jObject in jArray)
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
						string str1 = str;
						if (string.IsNullOrEmpty(str1))
						{
							this.RefData.Add(new FormPortalViewModel()
							{
								Specificulture = this.Specificulture,
								AttributeSetId = updateViewModel.ReferenceId.Value,
								Obj = Newtonsoft.Json.Linq.Extensions.Value<JObject>(jObject.get_Item("obj"))
							});
						}
						else
						{
							RepositoryResponse<FormPortalViewModel> singleModel = ViewModelBase<MixCmsContext, MixAttributeSetData, FormPortalViewModel>.Repository.GetSingleModel((MixAttributeSetData m) => m.Id == str1 && m.Specificulture == this.Specificulture, _context, _transaction);
							if (!singleModel.get_IsSucceed())
							{
								continue;
							}
							singleModel.get_Data().Obj = Newtonsoft.Json.Linq.Extensions.Value<JObject>(jObject.get_Item("obj"));
							this.RefData.Add(singleModel.get_Data());
						}
					}
				}
			}
			RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSets.ReadViewModel> repositoryResponse = ViewModelBase<MixCmsContext, MixAttributeSet, Mix.Cms.Lib.ViewModels.MixAttributeSets.ReadViewModel>.Repository.GetSingleModel((MixAttributeSet m) => m.Name == this.AttributeSetName, _context, _transaction);
			RepositoryResponse<Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel> templateByPath = Mix.Cms.Lib.ViewModels.MixTemplates.UpdateViewModel.GetTemplateByPath(repositoryResponse.get_Data().EdmTemplate, this.Specificulture, null, null);
			Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel webPath = this.Values.FirstOrDefault<Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel>((Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel f) => f.AttributeFieldName == "edm");
			if (webPath != null && templateByPath.get_IsSucceed() && !string.IsNullOrEmpty(templateByPath.get_Data().Content))
			{
				string content = templateByPath.get_Data().Content;
				foreach (JProperty jProperty in this.Obj.Properties())
				{
					content = content.Replace(string.Concat("[[", jProperty.get_Name(), "]]"), Newtonsoft.Json.Linq.Extensions.Value<string>(this.Obj.get_Item(jProperty.get_Name())));
				}
				FileViewModel fileViewModel = new FileViewModel()
				{
					Content = content,
					Extension = ".html",
					FileFolder = "edms",
					Filename = string.Concat(repositoryResponse.get_Data().EdmSubject, "-", this.Id)
				};
				if (FileRepository.Instance.SaveWebFile(fileViewModel))
				{
					this.Obj.set_Item("edm", fileViewModel.WebPath);
					webPath.StringValue = fileViewModel.WebPath;
				}
			}
			return base.ParseModel(_context, _transaction);
		}

		private void ParseModelValue(JToken property, Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel item)
		{
			string lower;
			string str;
			string str1;
			if (item.Field.IsEncrypt)
			{
				JObject jObject = Newtonsoft.Json.Linq.Extensions.Value<JObject>(property);
				item.StringValue = jObject.ToString(0, Array.Empty<JsonConverter>());
				Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel updateViewModel = item;
				JToken jToken = jObject.get_Item("data");
				if (jToken != null)
				{
					str = jToken.ToString();
				}
				else
				{
					str = null;
				}
				updateViewModel.EncryptValue = str;
				Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel updateViewModel1 = item;
				JToken jToken1 = jObject.get_Item("key");
				if (jToken1 != null)
				{
					str1 = jToken1.ToString();
				}
				else
				{
					str1 = null;
				}
				updateViewModel1.EncryptKey = str1;
				return;
			}
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
					string str2 = Newtonsoft.Json.Linq.Extensions.Value<string>(property);
					if (!StringExtension.IsBase64(str2))
					{
						item.StringValue = str2;
						return;
					}
					RepositoryResponse<Mix.Cms.Lib.ViewModels.MixMedias.UpdateViewModel> repositoryResponse = ((ViewModelBase<MixCmsContext, MixMedia, Mix.Cms.Lib.ViewModels.MixMedias.UpdateViewModel>)(new Mix.Cms.Lib.ViewModels.MixMedias.UpdateViewModel()
					{
						Specificulture = this.Specificulture,
						Status = MixEnums.MixContentStatus.Published,
						MediaFile = new FileViewModel()
						{
							FileStream = str2,
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
					this.Obj.set_Item(item.AttributeFieldName, item.StringValue);
					return;
				}
				case MixEnums.MixDataType.Boolean:
				{
					item.BooleanValue = Newtonsoft.Json.Linq.Extensions.Value<bool?>(property);
					Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel updateViewModel2 = item;
					string str3 = Newtonsoft.Json.Linq.Extensions.Value<string>(property);
					if (str3 != null)
					{
						lower = str3.ToLower();
					}
					else
					{
						lower = null;
					}
					updateViewModel2.StringValue = lower;
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

		private JProperty ParseValue(Mix.Cms.Lib.ViewModels.MixAttributeSetValues.UpdateViewModel item, MixCmsContext context, IDbContextTransaction transaction)
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
					return new JProperty(item.AttributeFieldName, jArray);
				}
				default:
				{
					return new JProperty(item.AttributeFieldName, item.StringValue);
				}
			}
		}

		public override RepositoryResponse<FormPortalViewModel> SaveModel(bool isSaveSubModels = false, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			RepositoryResponse<FormPortalViewModel> repositoryResponse = base.SaveModel(isSaveSubModels, _context, _transaction);
			if (repositoryResponse.get_IsSucceed())
			{
				this.ParseData(null, null);
			}
			return repositoryResponse;
		}

		public override async Task<RepositoryResponse<FormPortalViewModel>> SaveModelAsync(bool isSaveSubModels = false, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			FormPortalViewModel.u003cSaveModelAsyncu003ed__76 variable = new FormPortalViewModel.u003cSaveModelAsyncu003ed__76();
			variable.u003cu003e4__this = this;
			variable.isSaveSubModels = isSaveSubModels;
			variable._context = _context;
			variable._transaction = _transaction;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<FormPortalViewModel>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<FormPortalViewModel.u003cSaveModelAsyncu003ed__76>(ref variable);
			return variable.u003cu003et__builder.Task;
		}

		public static async Task<RepositoryResponse<Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel>> SaveObjectAsync(JObject data, string attributeSetName)
		{
			string str;
			string str1;
			Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel formViewModel = new Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel();
			JToken item = data.get_Item("id");
			if (item != null)
			{
				str = Newtonsoft.Json.Linq.Extensions.Value<string>(item);
			}
			else
			{
				str = null;
			}
			formViewModel.Id = str;
			JToken jToken = data.get_Item("specificulture");
			if (jToken != null)
			{
				str1 = Newtonsoft.Json.Linq.Extensions.Value<string>(jToken);
			}
			else
			{
				str1 = null;
			}
			formViewModel.Specificulture = str1;
			formViewModel.AttributeSetName = attributeSetName;
			formViewModel.Obj = data;
			return await ((ViewModelBase<MixCmsContext, MixAttributeSetData, Mix.Cms.Lib.ViewModels.MixAttributeSetDatas.FormViewModel>)formViewModel).SaveModelAsync(false, null, null);
		}

		private async Task<RepositoryResponse<bool>> SaveRefDataAsync(MixAttributeSetData parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			FormPortalViewModel.u003cSaveRefDataAsyncu003ed__80 variable = new FormPortalViewModel.u003cSaveRefDataAsyncu003ed__80();
			variable.u003cu003e4__this = this;
			variable.parent = parent;
			variable.context = context;
			variable.transaction = transaction;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<FormPortalViewModel.u003cSaveRefDataAsyncu003ed__80>(ref variable);
			return variable.u003cu003et__builder.Task;
		}

		private async Task<RepositoryResponse<bool>> SaveRelatedDataAsync(MixAttributeSetData parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			FormPortalViewModel.u003cSaveRelatedDataAsyncu003ed__81 variable = new FormPortalViewModel.u003cSaveRelatedDataAsyncu003ed__81();
			variable.u003cu003e4__this = this;
			variable.parent = parent;
			variable.context = context;
			variable.transaction = transaction;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<FormPortalViewModel.u003cSaveRelatedDataAsyncu003ed__81>(ref variable);
			return variable.u003cu003et__builder.Task;
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
			if (repositoryResponse1.get_IsSucceed())
			{
				ViewModelHelper.HandleResult<bool>(await this.SaveRefDataAsync(parent, _context, _transaction), ref repositoryResponse1);
			}
			return repositoryResponse1;
		}

		private async Task<RepositoryResponse<bool>> SaveValues(MixAttributeSetData parent, MixCmsContext context, IDbContextTransaction transaction)
		{
			FormPortalViewModel.u003cSaveValuesu003ed__79 variable = new FormPortalViewModel.u003cSaveValuesu003ed__79();
			variable.u003cu003e4__this = this;
			variable.parent = parent;
			variable.context = context;
			variable.transaction = transaction;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<bool>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<FormPortalViewModel.u003cSaveValuesu003ed__79>(ref variable);
			return variable.u003cu003et__builder.Task;
		}
	}
}