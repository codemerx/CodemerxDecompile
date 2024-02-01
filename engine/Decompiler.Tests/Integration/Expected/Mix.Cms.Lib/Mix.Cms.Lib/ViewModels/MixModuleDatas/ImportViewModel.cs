using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.Services;
using Mix.Cms.Lib.ViewModels;
using Mix.Common.Helper;
using Mix.Domain.Core.Models;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixModuleDatas
{
	public class ImportViewModel : ViewModelBase<MixCmsContext, MixModuleData, ImportViewModel>
	{
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

		[JsonProperty("dataProperties")]
		public List<ApiModuleDataValueViewModel> DataProperties
		{
			get;
			set;
		}

		[JsonIgnore]
		[JsonProperty("fields")]
		public string Fields { get; set; } = "[]";

		[JsonProperty("id")]
		public string Id
		{
			get;
			set;
		}

		[JsonProperty("jItem")]
		public JObject JItem
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

		[JsonProperty("moduleId")]
		public int ModuleId
		{
			get;
			set;
		}

		[JsonProperty("pageId")]
		public int? PageId
		{
			get;
			set;
		}

		[JsonProperty("postId")]
		public int? PostId
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

		[JsonProperty("productId")]
		public int? ProductId
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
		[JsonProperty("value")]
		public string Value
		{
			get;
			set;
		}

		public ImportViewModel()
		{
		}

		public ImportViewModel(MixModuleData model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			JToken jToken = null;
			string fields;
			List<ApiModuleDataValueViewModel> apiModuleDataValueViewModels;
			string str;
			string str1;
			MixModule mixModule = _context.MixModule.First<MixModule>((MixModule m) => m.Id == this.ModuleId && m.Specificulture == this.Specificulture);
			if (mixModule != null)
			{
				fields = mixModule.Fields;
			}
			else
			{
				fields = null;
			}
			this.Fields = fields;
			if (this.Fields == null)
			{
				apiModuleDataValueViewModels = null;
			}
			else
			{
				apiModuleDataValueViewModels = JsonConvert.DeserializeObject<List<ApiModuleDataValueViewModel>>(this.Fields);
			}
			this.DataProperties = apiModuleDataValueViewModels;
			this.JItem = (this.Value == null ? this.InitValue() : JsonConvert.DeserializeObject<JObject>(this.Value));
			foreach (ApiModuleDataValueViewModel dataProperty in this.DataProperties)
			{
				if (this.JItem.TryGetValue(dataProperty.Name, ref jToken))
				{
					continue;
				}
				string empty = string.Empty;
				if (dataProperty.DataType != MixEnums.MixDataType.Upload)
				{
					JToken item = this.JItem.get_Item(dataProperty.Name);
					if (item != null)
					{
						str = Newtonsoft.Json.Linq.Extensions.Value<JObject>(item).Value<string>("value");
					}
					else
					{
						str = null;
					}
					empty = str;
				}
				else
				{
					string config = MixService.GetConfig<string>("Domain");
					JToken item1 = this.JItem.get_Item(dataProperty.Name);
					if (item1 != null)
					{
						str1 = Newtonsoft.Json.Linq.Extensions.Value<JObject>(item1).Value<string>("value");
					}
					else
					{
						str1 = null;
					}
					empty = Path.Combine(config, str1);
				}
				JObject jItem = this.JItem;
				string name = dataProperty.Name;
				JObject jObject = new JObject();
				jObject.Add(new JProperty("dataType", (object)dataProperty.DataType));
				jObject.Add(new JProperty("value", empty));
				jItem.set_Item(name, jObject);
			}
		}

		public ApiModuleDataValueViewModel GetDataProperty(string name)
		{
			return this.DataProperties.FirstOrDefault<ApiModuleDataValueViewModel>((ApiModuleDataValueViewModel p) => p.Name == name);
		}

		public string GetStringValue(string name)
		{
			ApiModuleDataValueViewModel apiModuleDataValueViewModel = this.DataProperties.FirstOrDefault<ApiModuleDataValueViewModel>((ApiModuleDataValueViewModel p) => p.Name == name);
			if (apiModuleDataValueViewModel == null || apiModuleDataValueViewModel.Value == null)
			{
				return string.Empty;
			}
			return apiModuleDataValueViewModel.Value.ToString();
		}

		public JObject InitValue()
		{
			JObject jObject = new JObject();
			foreach (ApiModuleDataValueViewModel dataProperty in this.DataProperties)
			{
				JObject jObject1 = new JObject();
				jObject1.Add(new JProperty("dataType", (object)dataProperty.DataType));
				jObject1.Add(new JProperty("value", dataProperty.Value));
				jObject.Add(new JProperty(dataProperty.Name, jObject1));
			}
			return jObject;
		}

		public override MixModuleData ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (!string.IsNullOrEmpty(this.Id))
			{
				this.LastModified = new DateTime?(DateTime.UtcNow);
			}
			else
			{
				this.Id = Guid.NewGuid().ToString();
				this.CreatedDateTime = DateTime.UtcNow;
			}
			this.Value = JsonConvert.SerializeObject(this.JItem);
			this.Fields = JsonConvert.SerializeObject(this.DataProperties);
			return base.ParseModel(_context, _transaction);
		}

		public string ParseObjectValue()
		{
			JObject jObject = new JObject();
			foreach (ApiModuleDataValueViewModel dataProperty in this.DataProperties)
			{
				JObject jObject1 = new JObject();
				jObject1.Add(new JProperty("dataType", (object)dataProperty.DataType));
				jObject1.Add(new JProperty("value", dataProperty.Value));
				jObject.Add(new JProperty(dataProperty.Name, jObject1));
			}
			return jObject.ToString(0, Array.Empty<JsonConverter>());
		}

		public string Property(string name)
		{
			JToken item = this.JItem.get_Item(name);
			if (item == null)
			{
				return null;
			}
			return Newtonsoft.Json.Linq.Extensions.Value<JObject>(item).Value<string>("value");
		}

		public static async Task<RepositoryResponse<List<ReadViewModel>>> UpdateInfosAsync(List<ReadViewModel> data)
		{
			ImportViewModel.u003cUpdateInfosAsyncu003ed__78 variable = new ImportViewModel.u003cUpdateInfosAsyncu003ed__78();
			variable.data = data;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<List<ReadViewModel>>>.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<ImportViewModel.u003cUpdateInfosAsyncu003ed__78>(ref variable);
			return variable.u003cu003et__builder.Task;
		}

		public override void Validate(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			base.Validate(_context, _transaction);
			if (base.get_IsValid())
			{
				foreach (ApiModuleDataValueViewModel dataProperty in this.DataProperties)
				{
					RepositoryResponse<bool> repositoryResponse = dataProperty.Validate<MixModuleData>(this.Id, this.Specificulture, this.JItem, _context, _transaction);
					if (repositoryResponse.get_IsSucceed())
					{
						continue;
					}
					base.set_IsValid(false);
					base.get_Errors().AddRange(repositoryResponse.get_Errors());
				}
			}
		}
	}
}