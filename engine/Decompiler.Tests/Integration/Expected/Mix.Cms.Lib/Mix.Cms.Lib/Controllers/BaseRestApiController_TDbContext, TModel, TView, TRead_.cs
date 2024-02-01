using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Primitives;
using Mix.Cms.Lib.Repositories;
using Mix.Cms.Lib.Services;
using Mix.Cms.Lib.ViewModels;
using Mix.Common.Helper;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.Repository;
using Mix.Domain.Data.ViewModels;
using Mix.Heart.Enums;
using Mix.Heart.Extensions;
using Mix.Heart.Helpers;
using Mix.Services;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.Controllers
{
	[Produces("application/json", new string[] {  })]
	public class BaseRestApiController<TDbContext, TModel, TView, TRead> : Controller
	where TDbContext : DbContext
	where TModel : class
	where TView : ViewModelBase<TDbContext, TModel, TView>
	where TRead : ViewModelBase<TDbContext, TModel, TRead>
	{
		protected static TDbContext _context;

		protected static IDbContextTransaction _transaction;

		protected string _lang;

		protected bool _forbidden;

		protected string _domain;

		public BaseRestApiController()
		{
		}

		[HttpGet("remove-cache/{id}")]
		public async Task<ActionResult> ClearCacheAsync(string id)
		{
			string str;
			string str1 = string.Concat("_", id);
			str = (!string.IsNullOrEmpty(this._lang) ? string.Concat("_", this._lang) : string.Empty);
			string str2 = string.Concat(str1, str);
			await CacheService.RemoveCacheAsync(typeof(TView), str2);
			return this.NoContent();
		}

		[HttpGet("remove-cache")]
		public async Task<ActionResult> ClearCacheAsync()
		{
			await CacheService.RemoveCacheAsync(typeof(TView), null);
			return this.NoContent();
		}

		[HttpPost]
		public async Task<ActionResult<TModel>> Create([FromBody] TView data)
		{
			RepositoryResponse<TView> repositoryResponse = await this.SaveAsync(data, true);
			return (!repositoryResponse.get_IsSucceed() ? this.BadRequest(repositoryResponse.get_Errors()) : this.Ok(repositoryResponse.get_Data()));
		}

		[HttpGet("default")]
		public ActionResult<TView> Default()
		{
			ActionResult<TView> actionResult;
			TDbContext tDbContext = UnitOfWorkHelper<TDbContext>.InitContext();
			try
			{
				IDbContextTransaction dbContextTransaction = tDbContext.get_Database().BeginTransaction();
				TView tView = ReflectionHelper.InitModel<TView>();
				ReflectionHelper.SetPropertyValue<TView>(tView, new JProperty("Specificulture", this._lang));
				ReflectionHelper.SetPropertyValue<TView>(tView, new JProperty("Status", MixService.GetConfig<string>("DefaultContentStatus")));
				tView.ExpandView(tDbContext, dbContextTransaction);
				actionResult = this.Ok(tView);
			}
			finally
			{
				if (tDbContext != null)
				{
					((IDisposable)(object)tDbContext).Dispose();
				}
			}
			return actionResult;
		}

		[HttpDelete("{id}")]
		public virtual async Task<ActionResult<TModel>> Delete(string id)
		{
			RepositoryResponse<TModel> repositoryResponse = await this.DeleteAsync(id, true);
			return (!repositoryResponse.get_IsSucceed() ? this.BadRequest(repositoryResponse.get_Errors()) : this.Ok(repositoryResponse.get_Data()));
		}

		protected async Task<RepositoryResponse<TModel>> DeleteAsync<T>(string id, bool isDeleteRelated = false)
		where T : ViewModelBase<TDbContext, TModel, T>
		{
			RepositoryResponse<TModel> repositoryResponse;
			RepositoryResponse<T> singleAsync = await this.GetSingleAsync<T>(id);
			if (!singleAsync.get_IsSucceed())
			{
				RepositoryResponse<TModel> repositoryResponse1 = new RepositoryResponse<TModel>();
				repositoryResponse1.set_IsSucceed(false);
				repositoryResponse = repositoryResponse1;
			}
			else
			{
				repositoryResponse = await this.DeleteAsync<T>(singleAsync.get_Data(), isDeleteRelated);
			}
			return repositoryResponse;
		}

		protected async Task<RepositoryResponse<TModel>> DeleteAsync(string id, bool isDeleteRelated = false)
		{
			RepositoryResponse<TModel> repositoryResponse;
			RepositoryResponse<TView> singleAsync = await this.GetSingleAsync(id);
			if (!singleAsync.get_IsSucceed())
			{
				RepositoryResponse<TModel> repositoryResponse1 = new RepositoryResponse<TModel>();
				repositoryResponse1.set_IsSucceed(false);
				repositoryResponse = repositoryResponse1;
			}
			else
			{
				repositoryResponse = await this.DeleteAsync(singleAsync.get_Data(), isDeleteRelated);
			}
			return repositoryResponse;
		}

		protected async Task<RepositoryResponse<TModel>> DeleteAsync(TView data, bool isDeleteRelated = false)
		{
			RepositoryResponse<TModel> repositoryResponse;
			if (data == null)
			{
				RepositoryResponse<TModel> repositoryResponse1 = new RepositoryResponse<TModel>();
				repositoryResponse1.set_IsSucceed(false);
				repositoryResponse = repositoryResponse1;
			}
			else
			{
				TDbContext tDbContext = default(TDbContext);
				ConfiguredTaskAwaitable<RepositoryResponse<TModel>> configuredTaskAwaitable = data.RemoveModelAsync(isDeleteRelated, tDbContext, null).ConfigureAwait(false);
				repositoryResponse = await configuredTaskAwaitable;
			}
			return repositoryResponse;
		}

		protected async Task<RepositoryResponse<TModel>> DeleteAsync<T>(T data, bool isDeleteRelated = false)
		where T : ViewModelBase<TDbContext, TModel, T>
		{
			RepositoryResponse<TModel> repositoryResponse;
			if (data == null)
			{
				RepositoryResponse<TModel> repositoryResponse1 = new RepositoryResponse<TModel>();
				repositoryResponse1.set_IsSucceed(false);
				repositoryResponse = repositoryResponse1;
			}
			else
			{
				TDbContext tDbContext = default(TDbContext);
				ConfiguredTaskAwaitable<RepositoryResponse<TModel>> configuredTaskAwaitable = data.RemoveModelAsync(isDeleteRelated, tDbContext, null).ConfigureAwait(false);
				repositoryResponse = await configuredTaskAwaitable;
			}
			return repositoryResponse;
		}

		protected async Task<RepositoryResponse<List<TModel>>> DeleteListAsync(Expression<Func<TModel, bool>> predicate, bool isRemoveRelatedModel = false)
		{
			TDbContext tDbContext = default(TDbContext);
			RepositoryResponse<List<TModel>> repositoryResponse = await DefaultRepository<TDbContext, TModel, TView>.get_Instance().RemoveListModelAsync(isRemoveRelatedModel, predicate, tDbContext, null);
			return repositoryResponse;
		}

		protected async Task<RepositoryResponse<FileViewModel>> ExportListAsync(Expression<Func<TModel, bool>> predicate, string type)
		{
			RepositoryResponse<List<TModel>> modelListByAsync = await DefaultModelRepository<TDbContext, TModel>.get_Instance().GetModelListByAsync(predicate, BaseRestApiController<TDbContext, TModel, TView, TRead>._context, null);
			FileViewModel fileViewModel = null;
			if (modelListByAsync.get_IsSucceed())
			{
				string str = string.Concat("Exports/Structures/", typeof(TModel).Name);
				string str1 = type.ToString();
				DateTime utcNow = DateTime.UtcNow;
				string str2 = string.Concat(str1, "_", utcNow.ToString("ddMMyyyy"));
				object[] jProperty = new object[] { new JProperty("type", type.ToString()), new JProperty("data", JArray.FromObject(modelListByAsync.get_Data())) };
				JObject jObject = new JObject(jProperty);
				FileViewModel fileViewModel1 = new FileViewModel()
				{
					Filename = str2,
					Extension = ".json",
					FileFolder = str,
					Content = jObject.ToString()
				};
				fileViewModel = fileViewModel1;
				FileRepository.Instance.SaveWebFile(fileViewModel);
			}
			UnitOfWorkHelper<TDbContext>.HandleTransaction(modelListByAsync.get_IsSucceed(), true, BaseRestApiController<TDbContext, TModel, TView, TRead>._transaction);
			RepositoryResponse<FileViewModel> repositoryResponse = new RepositoryResponse<FileViewModel>();
			repositoryResponse.set_IsSucceed(true);
			repositoryResponse.set_Data(fileViewModel);
			return repositoryResponse;
		}

		[HttpGet]
		public virtual async Task<ActionResult<PaginationModel<TRead>>> Get()
		{
			DateTime dateTime;
			DateTime dateTime1;
			int num;
			MixHeartEnums.DisplayDirection displayDirection;
			int num1;
			int num2;
			DateTime.TryParse(base.get_Request().get_Query().get_Item("fromDate"), out dateTime);
			DateTime.TryParse(base.get_Request().get_Query().get_Item("toDate"), out dateTime1);
			int.TryParse(base.get_Request().get_Query().get_Item("pageIndex"), out num);
			Enum.TryParse<MixHeartEnums.DisplayDirection>(base.get_Request().get_Query().get_Item("direction"), out displayDirection);
			bool flag = int.TryParse(base.get_Request().get_Query().get_Item("pageSize"), out num1);
			RequestPaging requestPaging = new RequestPaging();
			requestPaging.set_PageIndex(num);
			num2 = (flag ? num1 : 100);
			requestPaging.set_PageSize(new int?(num2));
			StringValues item = base.get_Request().get_Query().get_Item("orderBy");
			requestPaging.set_OrderBy(StringExtension.ToTitleCase(item.ToString()));
			requestPaging.set_Direction(displayDirection);
			RequestPaging requestPaging1 = requestPaging;
			int? nullable = null;
			int? nullable1 = nullable;
			nullable = null;
			TDbContext tDbContext = default(TDbContext);
			ConfiguredTaskAwaitable<RepositoryResponse<PaginationModel<TRead>>> configuredTaskAwaitable = DefaultRepository<TDbContext, TModel, TRead>.get_Instance().GetModelListAsync(requestPaging1.get_OrderBy(), requestPaging1.get_Direction(), requestPaging1.get_PageSize(), new int?(requestPaging1.get_PageIndex()), nullable1, nullable, tDbContext, null).ConfigureAwait(false);
			RepositoryResponse<PaginationModel<TRead>> repositoryResponse = await configuredTaskAwaitable;
			return (!repositoryResponse.get_IsSucceed() ? this.BadRequest(repositoryResponse.get_Errors()) : this.Ok(repositoryResponse.get_Data()));
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<TView>> Get(string id)
		{
			RepositoryResponse<TView> singleAsync = await this.GetSingleAsync(id);
			return (!singleAsync.get_IsSucceed() ? this.NotFound() : singleAsync.get_Data());
		}

		protected void GetLanguage()
		{
			bool item;
			RouteData routeData = base.get_RouteData();
			if (routeData != null)
			{
				item = routeData.get_Values().get_Item("culture");
			}
			else
			{
				item = false;
			}
			this._lang = (item ? base.get_RouteData().get_Values().get_Item("culture").ToString() : string.Empty);
			this._domain = string.Format("{0}://{1}", base.get_Request().get_Scheme(), base.get_Request().get_Host());
		}

		protected async Task<RepositoryResponse<PaginationModel<TRead>>> GetListAsync(Expression<Func<TModel, bool>> predicate = null)
		{
			DateTime dateTime;
			DateTime dateTime1;
			int num;
			MixHeartEnums.DisplayDirection displayDirection;
			int num1;
			int? nullable;
			TDbContext tDbContext;
			int num2;
			DateTime.TryParse(base.get_Request().get_Query().get_Item("fromDate"), out dateTime);
			DateTime.TryParse(base.get_Request().get_Query().get_Item("toDate"), out dateTime1);
			int.TryParse(base.get_Request().get_Query().get_Item("pageIndex"), out num);
			Enum.TryParse<MixHeartEnums.DisplayDirection>(base.get_Request().get_Query().get_Item("direction"), out displayDirection);
			bool flag = int.TryParse(base.get_Request().get_Query().get_Item("pageSize"), out num1);
			RequestPaging requestPaging = new RequestPaging();
			requestPaging.set_PageIndex(num);
			num2 = (flag ? num1 : 100);
			requestPaging.set_PageSize(new int?(num2));
			StringValues item = base.get_Request().get_Query().get_Item("orderBy");
			requestPaging.set_OrderBy(StringExtension.ToTitleCase(item.ToString()));
			requestPaging.set_Direction(displayDirection);
			RequestPaging requestPaging1 = requestPaging;
			RepositoryResponse<PaginationModel<TRead>> modelListByAsync = null;
			if (modelListByAsync == null)
			{
				if (predicate == null)
				{
					nullable = null;
					int? nullable1 = nullable;
					nullable = null;
					tDbContext = default(TDbContext);
					ConfiguredTaskAwaitable<RepositoryResponse<PaginationModel<TRead>>> configuredTaskAwaitable = DefaultRepository<TDbContext, TModel, TRead>.get_Instance().GetModelListAsync(requestPaging1.get_OrderBy(), requestPaging1.get_Direction(), requestPaging1.get_PageSize(), new int?(requestPaging1.get_PageIndex()), nullable1, nullable, tDbContext, null).ConfigureAwait(false);
					modelListByAsync = await configuredTaskAwaitable;
				}
				else
				{
					nullable = null;
					int? nullable2 = nullable;
					nullable = null;
					tDbContext = default(TDbContext);
					modelListByAsync = await DefaultRepository<TDbContext, TModel, TRead>.get_Instance().GetModelListByAsync(predicate, requestPaging1.get_OrderBy(), requestPaging1.get_Direction(), requestPaging1.get_PageSize(), new int?(requestPaging1.get_PageIndex()), nullable2, nullable, tDbContext, null);
				}
			}
			return modelListByAsync;
		}

		protected async Task<RepositoryResponse<T>> GetSingleAsync<T>(string id)
		where T : ViewModelBase<TDbContext, TModel, T>
		{
			Expression<Func<TModel, bool>> expression = ReflectionHelper.GetExpression<TModel>("Id", id, 1, "model");
			if (!string.IsNullOrEmpty(this._lang))
			{
				Expression<Func<TModel, bool>> expression1 = ReflectionHelper.GetExpression<TModel>("Specificulture", this._lang, 1, "model");
				expression = ReflectionHelper.CombineExpression<TModel>(expression, expression1, 6, "model");
			}
			return await this.GetSingleAsync<T>(expression);
		}

		protected async Task<RepositoryResponse<TView>> GetSingleAsync(string id)
		{
			Expression<Func<TModel, bool>> expression = ReflectionHelper.GetExpression<TModel>("Id", id, 1, "model");
			if (!string.IsNullOrEmpty(this._lang))
			{
				Expression<Func<TModel, bool>> expression1 = ReflectionHelper.GetExpression<TModel>("Specificulture", this._lang, 1, "model");
				expression = ReflectionHelper.CombineExpression<TModel>(expression, expression1, 6, "model");
			}
			return await this.GetSingleAsync(expression);
		}

		protected async Task<RepositoryResponse<TView>> GetSingleAsync(Expression<Func<TModel, bool>> predicate = null)
		{
			RepositoryResponse<TView> singleModelAsync = null;
			if (predicate != null)
			{
				TDbContext tDbContext = default(TDbContext);
				singleModelAsync = await DefaultRepository<TDbContext, TModel, TView>.get_Instance().GetSingleModelAsync(predicate, tDbContext, null);
			}
			return singleModelAsync;
		}

		protected async Task<RepositoryResponse<T>> GetSingleAsync<T>(Expression<Func<TModel, bool>> predicate = null)
		where T : ViewModelBase<TDbContext, TModel, T>
		{
			RepositoryResponse<T> singleModelAsync = null;
			if (predicate != null)
			{
				TDbContext tDbContext = default(TDbContext);
				singleModelAsync = await DefaultRepository<TDbContext, TModel, T>.get_Instance().GetSingleModelAsync(predicate, tDbContext, null);
			}
			return singleModelAsync;
		}

		public override void OnActionExecuting(ActionExecutingContext context)
		{
			string str;
			this.GetLanguage();
			if (MixService.GetIpConfig<bool>("IsRetrictIp"))
			{
				JArray ipConfig = MixService.GetIpConfig<JArray>("AllowedIps") ?? new JArray();
				JArray jArray = MixService.GetIpConfig<JArray>("ExceptIps") ?? new JArray();
				HttpContext httpContext = base.get_Request().get_HttpContext();
				if (httpContext != null)
				{
					ConnectionInfo connection = httpContext.get_Connection();
					if (connection != null)
					{
						IPAddress remoteIpAddress = connection.get_RemoteIpAddress();
						if (remoteIpAddress != null)
						{
							str = remoteIpAddress.ToString();
						}
						else
						{
							str = null;
						}
					}
					else
					{
						str = null;
					}
				}
				else
				{
					str = null;
				}
				string str1 = str;
				if (!ipConfig.Any<JToken>((JToken t) => Newtonsoft.Json.Linq.Extensions.Value<string>(t) == "*") && !ipConfig.Contains(str1) || jArray.Any<JToken>((JToken t) => Newtonsoft.Json.Linq.Extensions.Value<string>(t) == str1))
				{
					this._forbidden = true;
				}
			}
			base.OnActionExecuting(context);
		}

		[HttpPatch("{id}")]
		public async Task<IActionResult> Patch(string id, [FromBody] JObject fields)
		{
			IActionResult actionResult;
			RepositoryResponse<TView> singleAsync = await this.GetSingleAsync(id);
			if (!singleAsync.get_IsSucceed())
			{
				actionResult = this.NotFound();
			}
			else
			{
				RepositoryResponse<bool> repositoryResponse = await singleAsync.get_Data().UpdateFieldsAsync(fields);
				if (!repositoryResponse.get_IsSucceed())
				{
					actionResult = this.BadRequest(repositoryResponse.get_Errors());
				}
				else
				{
					actionResult = this.NoContent();
				}
			}
			return actionResult;
		}

		protected async Task<RepositoryResponse<TView>> SaveAsync(TView vm, bool isSaveSubModel)
		{
			RepositoryResponse<TView> repositoryResponse;
			if (vm == null)
			{
				repositoryResponse = new RepositoryResponse<TView>();
			}
			else
			{
				TDbContext tDbContext = default(TDbContext);
				ConfiguredTaskAwaitable<RepositoryResponse<TView>> configuredTaskAwaitable = vm.SaveModelAsync(isSaveSubModel, tDbContext, null).ConfigureAwait(false);
				repositoryResponse = await configuredTaskAwaitable;
			}
			return repositoryResponse;
		}

		protected async Task<RepositoryResponse<TModel>> SaveAsync(JObject obj, Expression<Func<TModel, bool>> predicate)
		{
			RepositoryResponse<TModel> repositoryResponse;
			if (obj == null)
			{
				repositoryResponse = new RepositoryResponse<TModel>();
			}
			else
			{
				List<EntityField> entityFields = new List<EntityField>();
				Type type = typeof(TModel);
				foreach (JProperty jProperty in obj.Properties())
				{
					string titleCase = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(jProperty.get_Name());
					PropertyInfo property = type.GetProperty(titleCase);
					if (property == null)
					{
						continue;
					}
					object obj1 = Convert.ChangeType(jProperty.get_Value(), property.PropertyType);
					EntityField entityField = new EntityField();
					entityField.set_PropertyName(titleCase);
					entityField.set_PropertyValue(obj1);
					entityFields.Add(entityField);
				}
				TDbContext tDbContext = default(TDbContext);
				repositoryResponse = await DefaultRepository<TDbContext, TModel, TView>.get_Instance().UpdateFieldsAsync(predicate, entityFields, tDbContext, null);
			}
			return repositoryResponse;
		}

		protected RepositoryResponse<List<TView>> SaveList(List<TView> lstVm, bool isSaveSubModel)
		{
			RepositoryResponse<List<TView>> repositoryResponse = new RepositoryResponse<List<TView>>();
			repositoryResponse.set_IsSucceed(true);
			RepositoryResponse<List<TView>> repositoryResponse1 = repositoryResponse;
			if (lstVm == null)
			{
				return repositoryResponse1;
			}
			foreach (TView tView in lstVm)
			{
				RepositoryResponse<TView> repositoryResponse2 = tView.SaveModel(isSaveSubModel, BaseRestApiController<TDbContext, TModel, TView, TRead>._context, BaseRestApiController<TDbContext, TModel, TView, TRead>._transaction);
				repositoryResponse1.set_IsSucceed((!repositoryResponse1.get_IsSucceed() ? false : repositoryResponse2.get_IsSucceed()));
				if (repositoryResponse2.get_IsSucceed())
				{
					continue;
				}
				repositoryResponse1.set_Exception(repositoryResponse2.get_Exception());
				repositoryResponse1.get_Errors().AddRange(repositoryResponse2.get_Errors());
			}
			return repositoryResponse1;
		}

		protected async Task<RepositoryResponse<List<TView>>> SaveListAsync(List<TView> lstVm, bool isSaveSubModel)
		{
			TDbContext tDbContext = default(TDbContext);
			RepositoryResponse<List<TView>> repositoryResponse = await DefaultRepository<TDbContext, TModel, TView>.get_Instance().SaveListModelAsync(lstVm, isSaveSubModel, tDbContext, null);
			return repositoryResponse;
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(string id, [FromBody] TView data)
		{
			IActionResult actionResult;
			if (data == null)
			{
				actionResult = this.BadRequest(new NullReferenceException());
			}
			else
			{
				string str = ReflectionHelper.GetPropertyValue<TView>(data, "Id").ToString();
				if (id == str)
				{
					RepositoryResponse<TView> repositoryResponse = await this.SaveAsync(data, true);
					if (repositoryResponse.get_IsSucceed())
					{
						actionResult = this.Ok(repositoryResponse.get_Data());
					}
					else if (await this.GetSingleAsync(str).get_IsSucceed())
					{
						actionResult = this.BadRequest(repositoryResponse.get_Errors());
					}
					else
					{
						actionResult = this.NotFound();
					}
				}
				else
				{
					actionResult = this.BadRequest();
				}
			}
			return actionResult;
		}
	}
}