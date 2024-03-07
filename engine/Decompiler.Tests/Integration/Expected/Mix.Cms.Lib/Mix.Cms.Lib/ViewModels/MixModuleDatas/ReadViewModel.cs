using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
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
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixModuleDatas
{
	public class ReadViewModel : ViewModelBase<MixCmsContext, MixModuleData, ReadViewModel>
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

		[JsonProperty("value")]
		public string Value
		{
			get;
			set;
		}

		public ReadViewModel()
		{
		}

		public ReadViewModel(MixModuleData model, MixCmsContext _context = null, IDbContextTransaction _transaction = null) : base(model, _context, _transaction)
		{
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			List<ApiModuleDataValueViewModel> apiModuleDataValueViewModels;
			object obj;
			object fields = this.Fields;
			if (fields == null)
			{
				MixModule mixModule = _context.MixModule.First<MixModule>((MixModule m) => m.Id == this.ModuleId && m.Specificulture == this.Specificulture);
				if (mixModule != null)
				{
					fields = mixModule.Fields;
				}
				else
				{
					fields = null;
				}
			}
			this.Fields = (string)fields;
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
				this.JItem.set_Item(dataProperty.Name, Helper.ParseValue(this.JItem, dataProperty));
				if (this.JItem.get_Item(dataProperty.Name) != null)
				{
					continue;
				}
				JObject jItem = this.JItem;
				string name = dataProperty.Name;
				JObject jObject = new JObject();
				jObject.Add(new JProperty("dataType", (object)dataProperty.DataType));
				JToken item = this.JItem.get_Item(dataProperty.Name);
				if (item != null)
				{
					obj = Newtonsoft.Json.Linq.Extensions.Value<JObject>(item).Value<string>("value");
				}
				else
				{
					obj = null;
				}
				jObject.Add(new JProperty("value", obj));
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
			if (string.IsNullOrEmpty(this.Id))
			{
				this.Id = Guid.NewGuid().ToString();
				this.CreatedDateTime = DateTime.UtcNow;
			}
			this.LastModified = new DateTime?(DateTime.UtcNow);
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
			RepositoryResponse<List<ReadViewModel>> repositoryResponse;
			MixCmsContext mixCmsContext = new MixCmsContext();
			IDbContextTransaction dbContextTransaction = mixCmsContext.get_Database().BeginTransaction();
			RepositoryResponse<List<ReadViewModel>> repositoryResponse1 = new RepositoryResponse<List<ReadViewModel>>();
			try
			{
				try
				{
					foreach (ReadViewModel datum in data)
					{
						DbSet<MixModuleData> mixModuleData = mixCmsContext.MixModuleData;
						MixModuleData priority = mixModuleData.FirstOrDefault<MixModuleData>((MixModuleData m) => m.Id == datum.Id && m.Specificulture == datum.Specificulture);
						if (priority == null)
						{
							continue;
						}
						priority.Priority = datum.Priority;
						mixCmsContext.Entry<MixModuleData>(priority).set_State(3);
					}
					RepositoryResponse<List<ReadViewModel>> repositoryResponse2 = repositoryResponse1;
					MixCmsContext mixCmsContext1 = mixCmsContext;
					CancellationToken cancellationToken = new CancellationToken();
					int num = await ((DbContext)mixCmsContext1).SaveChangesAsync(cancellationToken);
					repositoryResponse2.set_IsSucceed(num > 0);
					repositoryResponse2 = null;
					if (!repositoryResponse1.get_IsSucceed())
					{
						repositoryResponse1.get_Errors().Add("Nothing changed");
					}
					UnitOfWorkHelper<MixCmsContext>.HandleTransaction(repositoryResponse1.get_IsSucceed(), true, dbContextTransaction);
					repositoryResponse = repositoryResponse1;
				}
				catch (Exception exception1)
				{
					Exception exception = exception1;
					UnitOfWorkHelper<MixCmsContext>.HandleException<ReadViewModel>(exception, true, dbContextTransaction);
					RepositoryResponse<List<ReadViewModel>> repositoryResponse3 = new RepositoryResponse<List<ReadViewModel>>();
					repositoryResponse3.set_IsSucceed(false);
					repositoryResponse3.set_Data(null);
					repositoryResponse3.set_Exception(exception);
					repositoryResponse = repositoryResponse3;
				}
			}
			finally
			{
				RelationalDatabaseFacadeExtensions.CloseConnection(mixCmsContext.get_Database());
				dbContextTransaction.Dispose();
				mixCmsContext.Dispose();
			}
			return repositoryResponse;
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