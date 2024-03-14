using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib;
using Mix.Cms.Lib.Models.Cms;
using Mix.Domain.Core.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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
		public JArray Options { get; set; } = new JArray();

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
		}

		public RepositoryResponse<bool> Validate<T>(IConvertible id, string specificulture, JObject jItem, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
		where T : class
		{
			ApiModuleDataValueViewModel.u003cu003ec__DisplayClass40_0<T> variable = null;
			string str;
			string str1 = Newtonsoft.Json.Linq.Extensions.Value<string>(jItem.get_Item(this.Name).get_Item("value"));
			JProperty jProperty = new JProperty(this.Name, jItem.get_Item(this.Name));
			RepositoryResponse<bool> repositoryResponse = new RepositoryResponse<bool>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<bool> repositoryResponse1 = repositoryResponse;
			if (this.IsUnique)
			{
				if (id != null)
				{
					str = id.ToString();
				}
				else
				{
					str = null;
				}
				DbSet<MixModuleData> mixModuleData = _context.MixModuleData;
				ParameterExpression parameterExpression = Expression.Parameter(typeof(MixModuleData), "d");
				if (mixModuleData.Count<MixModuleData>(Expression.Lambda<Func<MixModuleData, bool>>(Expression.AndAlso(Expression.AndAlso(Expression.Equal(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixModuleData).GetMethod("get_Specificulture").MethodHandle)), Expression.Field(Expression.Constant(variable, typeof(ApiModuleDataValueViewModel.u003cu003ec__DisplayClass40_0<T>)), FieldInfo.GetFieldFromHandle(typeof(ApiModuleDataValueViewModel.u003cu003ec__DisplayClass40_0<T>).GetField("specificulture").FieldHandle, typeof(ApiModuleDataValueViewModel.u003cu003ec__DisplayClass40_0<T>).TypeHandle))), Expression.Call(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixModuleData).GetMethod("get_Value").MethodHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(string).GetMethod("Contains", new Type[] { typeof(string) }).MethodHandle), new Expression[] { Expression.Call(Expression.Field(Expression.Constant(variable, typeof(ApiModuleDataValueViewModel.u003cu003ec__DisplayClass40_0<T>)), FieldInfo.GetFieldFromHandle(typeof(ApiModuleDataValueViewModel.u003cu003ec__DisplayClass40_0<T>).GetField("jVal").FieldHandle, typeof(ApiModuleDataValueViewModel.u003cu003ec__DisplayClass40_0<T>).TypeHandle)), (MethodInfo)MethodBase.GetMethodFromHandle(typeof(JToken).GetMethod("ToString", new Type[] { typeof(Formatting), typeof(JsonConverter[]) }).MethodHandle), new Expression[] { Expression.Constant((Formatting)0, typeof(Formatting)), Expression.NewArrayInit(typeof(JsonConverter), Array.Empty<Expression>()) }) })), Expression.NotEqual(Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(MixModuleData).GetMethod("get_Id").MethodHandle)), Expression.Field(Expression.Constant(variable, typeof(ApiModuleDataValueViewModel.u003cu003ec__DisplayClass40_0<T>)), FieldInfo.GetFieldFromHandle(typeof(ApiModuleDataValueViewModel.u003cu003ec__DisplayClass40_0<T>).GetField("strId").FieldHandle, typeof(ApiModuleDataValueViewModel.u003cu003ec__DisplayClass40_0<T>).TypeHandle)))), new ParameterExpression[] { parameterExpression })) > 0)
				{
					repositoryResponse1.set_IsSucceed(false);
					repositoryResponse1.get_Errors().Add(string.Concat(this.Title, " is existed"));
				}
			}
			if (this.IsRequired && string.IsNullOrEmpty(str1))
			{
				repositoryResponse1.set_IsSucceed(false);
				repositoryResponse1.get_Errors().Add(string.Concat(this.Title, " is required"));
			}
			return repositoryResponse1;
		}
	}
}