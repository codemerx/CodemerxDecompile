using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Cms.Lib.ViewModels;
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
		public string Fields
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
			this.u003cFieldsu003ek__BackingField = "[]";
			base();
			return;
		}

		public ReadViewModel(MixModuleData model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.u003cFieldsu003ek__BackingField = "[]";
			base(model, _context, _transaction);
			return;
		}

		public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			stackVariable2 = this.get_Fields();
			if (stackVariable2 == null)
			{
				dummyVar0 = stackVariable2;
				stackVariable57 = _context.get_MixModule();
				V_0 = Expression.Parameter(Type.GetTypeFromHandle(// 
				// Current member / type: System.Void Mix.Cms.Lib.ViewModels.MixModuleDatas.ReadViewModel::ExpandView(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Exception in: System.Void ExpandView(Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com


		public ApiModuleDataValueViewModel GetDataProperty(string name)
		{
			V_0 = new ReadViewModel.u003cu003ec__DisplayClass77_0();
			V_0.name = name;
			return Enumerable.FirstOrDefault<ApiModuleDataValueViewModel>(this.get_DataProperties(), new Func<ApiModuleDataValueViewModel, bool>(V_0, ReadViewModel.u003cu003ec__DisplayClass77_0.u003cGetDataPropertyu003eb__0));
		}

		public string GetStringValue(string name)
		{
			V_0 = new ReadViewModel.u003cu003ec__DisplayClass75_0();
			V_0.name = name;
			V_1 = Enumerable.FirstOrDefault<ApiModuleDataValueViewModel>(this.get_DataProperties(), new Func<ApiModuleDataValueViewModel, bool>(V_0, ReadViewModel.u003cu003ec__DisplayClass75_0.u003cGetStringValueu003eb__0));
			if (V_1 == null || V_1.get_Value() == null)
			{
				return string.Empty;
			}
			return V_1.get_Value().ToString();
		}

		public JObject InitValue()
		{
			V_0 = new JObject();
			V_1 = this.get_DataProperties().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_3 = new JObject();
					V_3.Add(new JProperty("dataType", (object)V_2.get_DataType()));
					V_3.Add(new JProperty("value", V_2.get_Value()));
					V_0.Add(new JProperty(V_2.get_Name(), V_3));
				}
			}
			finally
			{
				V_1.Dispose();
			}
			return V_0;
		}

		public override MixModuleData ParseModel(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			if (string.IsNullOrEmpty(this.get_Id()))
			{
				this.set_Id(Guid.NewGuid().ToString());
				this.set_CreatedDateTime(DateTime.get_UtcNow());
			}
			this.set_LastModified(new DateTime?(DateTime.get_UtcNow()));
			this.set_Value(JsonConvert.SerializeObject(this.get_JItem()));
			this.set_Fields(JsonConvert.SerializeObject(this.get_DataProperties()));
			return this.ParseModel(_context, _transaction);
		}

		public string ParseObjectValue()
		{
			V_0 = new JObject();
			V_1 = this.get_DataProperties().GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					V_3 = new JObject();
					V_3.Add(new JProperty("dataType", (object)V_2.get_DataType()));
					V_3.Add(new JProperty("value", V_2.get_Value()));
					V_0.Add(new JProperty(V_2.get_Name(), V_3));
				}
			}
			finally
			{
				V_1.Dispose();
			}
			return V_0.ToString(0, Array.Empty<JsonConverter>());
		}

		public string Property(string name)
		{
			stackVariable3 = this.get_JItem().get_Item(name);
			if (stackVariable3 == null)
			{
				dummyVar0 = stackVariable3;
				return null;
			}
			return Newtonsoft.Json.Linq.Extensions.Value<JObject>(stackVariable3).Value<string>("value");
		}

		public static async Task<RepositoryResponse<List<ReadViewModel>>> UpdateInfosAsync(List<ReadViewModel> data)
		{
			V_0.data = data;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<List<ReadViewModel>>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ReadViewModel.u003cUpdateInfosAsyncu003ed__78>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public override void Validate(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		{
			this.Validate(_context, _transaction);
			if (this.get_IsValid())
			{
				V_0 = this.get_DataProperties().GetEnumerator();
				try
				{
					while (V_0.MoveNext())
					{
						V_1 = V_0.get_Current().Validate<MixModuleData>(this.get_Id(), this.get_Specificulture(), this.get_JItem(), _context, _transaction);
						if (V_1.get_IsSucceed())
						{
							continue;
						}
						this.set_IsValid(false);
						this.get_Errors().AddRange(V_1.get_Errors());
					}
				}
				finally
				{
					V_0.Dispose();
				}
			}
			return;
		}
	}
}