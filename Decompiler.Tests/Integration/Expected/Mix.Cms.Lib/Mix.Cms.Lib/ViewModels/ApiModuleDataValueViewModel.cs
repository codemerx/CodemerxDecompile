using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Domain.Core.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.ViewModels
{
	public class ApiModuleDataValueViewModel
	{
		[JsonProperty("dataType")]
		public MixEnums.MixDataType DataType
		{
			get;
			set;
		}

		[JsonProperty("isDisplay")]
		public bool IsDisplay
		{
			get;
			set;
		}

		[JsonProperty("isGroupBy")]
		public bool IsGroupBy
		{
			get;
			set;
		}

		[JsonProperty("isRequired")]
		public bool IsRequired
		{
			get;
			set;
		}

		[JsonProperty("isSelect")]
		public bool IsSelect
		{
			get;
			set;
		}

		[JsonProperty("isUnique")]
		public bool IsUnique
		{
			get;
			set;
		}

		[JsonProperty("name")]
		public string Name
		{
			get;
			set;
		}

		[JsonProperty("options")]
		public JArray Options
		{
			get;
			set;
		}

		[JsonProperty("title")]
		public string Title
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

		public ApiModuleDataValueViewModel()
		{
			this.u003cOptionsu003ek__BackingField = new JArray();
			base();
			return;
		}

		public RepositoryResponse<bool> Validate<T>(IConvertible id, string specificulture, JObject jItem, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		where T : class
		{
			V_0 = new ApiModuleDataValueViewModel.u003cu003ec__DisplayClass40_0<T>();
			V_0.specificulture = specificulture;
			V_1 = Newtonsoft.Json.Linq.Extensions.Value<string>(jItem.get_Item(this.get_Name()).get_Item("value"));
			V_0.jVal = new JProperty(this.get_Name(), jItem.get_Item(this.get_Name()));
			stackVariable18 = new RepositoryResponse<bool>();
			stackVariable18.set_IsSucceed(true);
			V_2 = stackVariable18;
			if (this.get_IsUnique())
			{
				stackVariable35 = V_0;
				if (id != null)
				{
					stackVariable38 = id.ToString();
				}
				else
				{
					stackVariable38 = null;
				}
				stackVariable35.strId = stackVariable38;
				stackVariable40 = _context.get_MixModuleData();
				V_3 = Expression.Parameter(Type.GetTypeFromHandle(// 
				// Current member / type: Mix.Domain.Core.ViewModels.RepositoryResponse`1<System.Boolean> Mix.Cms.Lib.ViewModels.ApiModuleDataValueViewModel::Validate(System.IConvertible,System.String,Newtonsoft.Json.Linq.JObject,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Exception in: Mix.Domain.Core.ViewModels.RepositoryResponse<System.Boolean> Validate(System.IConvertible,System.String,Newtonsoft.Json.Linq.JObject,Mix.Cms.Lib.Models.Cms.MixCmsContext,Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction)
				// Specified method is not supported.
				// 
				// mailto: JustDecompilePublicFeedback@telerik.com

	}
}