using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Primitives;
using Mix.Cms.Lib.ViewModels;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.ViewModels;
using Mix.Heart.Enums;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
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
			base();
			return;
		}

		[HttpGet("remove-cache/{id}")]
		public async Task<ActionResult> ClearCacheAsync(string id)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<ActionResult>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<BaseRestApiController<TDbContext, TModel, TView, TRead>.u003cClearCacheAsyncu003ed__8>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		[HttpGet("remove-cache")]
		public async Task<ActionResult> ClearCacheAsync()
		{
			V_0.u003cu003e4__this = this;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<ActionResult>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<BaseRestApiController<TDbContext, TModel, TView, TRead>.u003cClearCacheAsyncu003ed__9>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		[HttpPost]
		public async Task<ActionResult<TModel>> Create([FromBody] TView data)
		{
			V_0.u003cu003e4__this = this;
			V_0.data = data;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<ActionResult<TModel>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<BaseRestApiController<TDbContext, TModel, TView, TRead>.u003cCreateu003ed__10>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		[HttpGet("default")]
		public ActionResult<TView> Default()
		{
			V_0 = UnitOfWorkHelper<TDbContext>.InitContext();
			try
			{
				V_1 = V_0.get_Database().BeginTransaction();
				V_2 = ReflectionHelper.InitModel<TView>();
				ReflectionHelper.SetPropertyValue<TView>(V_2, new JProperty("Specificulture", this._lang));
				ReflectionHelper.SetPropertyValue<TView>(V_2, new JProperty("Status", MixService.GetConfig<string>("DefaultContentStatus")));
				V_2.ExpandView(V_0, V_1);
				V_3 = ActionResult<TView>.op_Implicit(this.Ok(V_2));
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
			return V_3;
		}

		[HttpDelete("{id}")]
		public virtual async Task<ActionResult<TModel>> Delete(string id)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<ActionResult<TModel>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<BaseRestApiController<TDbContext, TModel, TView, TRead>.u003cDeleteu003ed__13>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		protected async Task<RepositoryResponse<TModel>> DeleteAsync<T>(string id, bool isDeleteRelated = false)
		where T : ViewModelBase<TDbContext, TModel, T>
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.isDeleteRelated = isDeleteRelated;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<TModel>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<BaseRestApiController<TDbContext, TModel, TView, TRead>.u003cDeleteAsyncu003ed__20<T>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		protected async Task<RepositoryResponse<TModel>> DeleteAsync(string id, bool isDeleteRelated = false)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.isDeleteRelated = isDeleteRelated;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<TModel>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<BaseRestApiController<TDbContext, TModel, TView, TRead>.u003cDeleteAsyncu003ed__21>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		protected async Task<RepositoryResponse<TModel>> DeleteAsync(TView data, bool isDeleteRelated = false)
		{
			V_0.data = data;
			V_0.isDeleteRelated = isDeleteRelated;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<TModel>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<BaseRestApiController<TDbContext, TModel, TView, TRead>.u003cDeleteAsyncu003ed__22>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		protected async Task<RepositoryResponse<TModel>> DeleteAsync<T>(T data, bool isDeleteRelated = false)
		where T : ViewModelBase<TDbContext, TModel, T>
		{
			V_0.data = data;
			V_0.isDeleteRelated = isDeleteRelated;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<TModel>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<BaseRestApiController<TDbContext, TModel, TView, TRead>.u003cDeleteAsyncu003ed__23<T>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		protected async Task<RepositoryResponse<List<TModel>>> DeleteListAsync(Expression<Func<TModel, bool>> predicate, bool isRemoveRelatedModel = false)
		{
			V_0.predicate = predicate;
			V_0.isRemoveRelatedModel = isRemoveRelatedModel;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<List<TModel>>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<BaseRestApiController<TDbContext, TModel, TView, TRead>.u003cDeleteListAsyncu003ed__24>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		protected async Task<RepositoryResponse<FileViewModel>> ExportListAsync(Expression<Func<TModel, bool>> predicate, string type)
		{
			V_0.predicate = predicate;
			V_0.type = type;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<FileViewModel>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<BaseRestApiController<TDbContext, TModel, TView, TRead>.u003cExportListAsyncu003ed__25>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		[HttpGet]
		public virtual async Task<ActionResult<PaginationModel<TRead>>> Get()
		{
			V_0.u003cu003e4__this = this;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<ActionResult<PaginationModel<TRead>>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<BaseRestApiController<TDbContext, TModel, TView, TRead>.u003cGetu003ed__5>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<TView>> Get(string id)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<ActionResult<TView>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<BaseRestApiController<TDbContext, TModel, TView, TRead>.u003cGetu003ed__6>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		protected void GetLanguage()
		{
			stackVariable2 = this.get_RouteData();
			if (stackVariable2 != null)
			{
				stackVariable5 = stackVariable2.get_Values().get_Item("culture");
			}
			else
			{
				dummyVar0 = stackVariable2;
				stackVariable5 = false;
			}
			if (stackVariable5)
			{
				stackVariable11 = this.get_RouteData().get_Values().get_Item("culture").ToString();
			}
			else
			{
				stackVariable11 = string.Empty;
			}
			this._lang = stackVariable11;
			this._domain = string.Format("{0}://{1}", this.get_Request().get_Scheme(), this.get_Request().get_Host());
			return;
		}

		protected async Task<RepositoryResponse<PaginationModel<TRead>>> GetListAsync(Expression<Func<TModel, bool>> predicate = null)
		{
			V_0.u003cu003e4__this = this;
			V_0.predicate = predicate;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<PaginationModel<TRead>>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<BaseRestApiController<TDbContext, TModel, TView, TRead>.u003cGetListAsyncu003ed__26>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		protected async Task<RepositoryResponse<T>> GetSingleAsync<T>(string id)
		where T : ViewModelBase<TDbContext, TModel, T>
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<T>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<BaseRestApiController<TDbContext, TModel, TView, TRead>.u003cGetSingleAsyncu003ed__16<T>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		protected async Task<RepositoryResponse<TView>> GetSingleAsync(string id)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<TView>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<BaseRestApiController<TDbContext, TModel, TView, TRead>.u003cGetSingleAsyncu003ed__17>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		protected async Task<RepositoryResponse<TView>> GetSingleAsync(Expression<Func<TModel, bool>> predicate = null)
		{
			V_0.predicate = predicate;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<TView>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<BaseRestApiController<TDbContext, TModel, TView, TRead>.u003cGetSingleAsyncu003ed__18>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		protected async Task<RepositoryResponse<T>> GetSingleAsync<T>(Expression<Func<TModel, bool>> predicate = null)
		where T : ViewModelBase<TDbContext, TModel, T>
		{
			V_0.predicate = predicate;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<T>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<BaseRestApiController<TDbContext, TModel, TView, TRead>.u003cGetSingleAsyncu003ed__19<T>>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		public override void OnActionExecuting(ActionExecutingContext context)
		{
			this.GetLanguage();
			if (MixService.GetIpConfig<bool>("IsRetrictIp"))
			{
				V_0 = new BaseRestApiController<TDbContext, TModel, TView, TRead>.u003cu003ec__DisplayClass14_0();
				stackVariable7 = MixService.GetIpConfig<JArray>("AllowedIps");
				if (stackVariable7 == null)
				{
					dummyVar0 = stackVariable7;
					stackVariable7 = new JArray();
				}
				V_1 = stackVariable7;
				stackVariable9 = MixService.GetIpConfig<JArray>("ExceptIps");
				if (stackVariable9 == null)
				{
					dummyVar1 = stackVariable9;
					stackVariable9 = new JArray();
				}
				V_2 = stackVariable9;
				stackVariable10 = V_0;
				stackVariable13 = this.get_Request().get_HttpContext();
				if (stackVariable13 != null)
				{
					stackVariable14 = stackVariable13.get_Connection();
					if (stackVariable14 != null)
					{
						stackVariable15 = stackVariable14.get_RemoteIpAddress();
						if (stackVariable15 != null)
						{
							stackVariable16 = stackVariable15.ToString();
						}
						else
						{
							dummyVar4 = stackVariable15;
							stackVariable16 = null;
						}
					}
					else
					{
						dummyVar3 = stackVariable14;
						stackVariable16 = null;
					}
				}
				else
				{
					dummyVar2 = stackVariable13;
					stackVariable16 = null;
				}
				stackVariable10.remoteIp = stackVariable16;
				stackVariable17 = V_1;
				stackVariable18 = BaseRestApiController<TDbContext, TModel, TView, TRead>.u003cu003ec.u003cu003e9__14_0;
				if (stackVariable18 == null)
				{
					dummyVar5 = stackVariable18;
					stackVariable18 = new Func<JToken, bool>(BaseRestApiController<TDbContext, TModel, TView, TRead>.u003cu003ec.u003cu003e9, BaseRestApiController<TDbContext, TModel, TView, TRead>.u003cu003ec.u003cOnActionExecutingu003eb__14_0);
					BaseRestApiController<TDbContext, TModel, TView, TRead>.u003cu003ec.u003cu003e9__14_0 = stackVariable18;
				}
				if (!Enumerable.Any<JToken>(stackVariable17, stackVariable18) && !V_1.Contains(JToken.op_Implicit(V_0.remoteIp)) || Enumerable.Any<JToken>(V_2, new Func<JToken, bool>(V_0, BaseRestApiController<TDbContext, TModel, TView, TRead>.u003cu003ec__DisplayClass14_0.u003cOnActionExecutingu003eb__1)))
				{
					this._forbidden = true;
				}
			}
			this.OnActionExecuting(context);
			return;
		}

		[HttpPatch("{id}")]
		public async Task<IActionResult> Patch(string id, [FromBody] JObject fields)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.fields = fields;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<IActionResult>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<BaseRestApiController<TDbContext, TModel, TView, TRead>.u003cPatchu003ed__12>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		protected async Task<RepositoryResponse<TView>> SaveAsync(TView vm, bool isSaveSubModel)
		{
			V_0.vm = vm;
			V_0.isSaveSubModel = isSaveSubModel;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<TView>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<BaseRestApiController<TDbContext, TModel, TView, TRead>.u003cSaveAsyncu003ed__27>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		protected async Task<RepositoryResponse<TModel>> SaveAsync(JObject obj, Expression<Func<TModel, bool>> predicate)
		{
			V_0.obj = obj;
			V_0.predicate = predicate;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<TModel>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<BaseRestApiController<TDbContext, TModel, TView, TRead>.u003cSaveAsyncu003ed__28>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		protected RepositoryResponse<List<TView>> SaveList(List<TView> lstVm, bool isSaveSubModel)
		{
			stackVariable0 = new RepositoryResponse<List<TView>>();
			stackVariable0.set_IsSucceed(true);
			V_0 = stackVariable0;
			if (lstVm == null)
			{
				return V_0;
			}
			V_1 = lstVm.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current().SaveModel(isSaveSubModel, BaseRestApiController<TDbContext, TModel, TView, TRead>._context, BaseRestApiController<TDbContext, TModel, TView, TRead>._transaction);
					stackVariable15 = V_0;
					if (!V_0.get_IsSucceed())
					{
						stackVariable18 = false;
					}
					else
					{
						stackVariable18 = V_2.get_IsSucceed();
					}
					stackVariable15.set_IsSucceed(stackVariable18);
					if (V_2.get_IsSucceed())
					{
						continue;
					}
					V_0.set_Exception(V_2.get_Exception());
					V_0.get_Errors().AddRange(V_2.get_Errors());
				}
			}
			finally
			{
				V_1.Dispose();
			}
			return V_0;
		}

		protected async Task<RepositoryResponse<List<TView>>> SaveListAsync(List<TView> lstVm, bool isSaveSubModel)
		{
			V_0.lstVm = lstVm;
			V_0.isSaveSubModel = isSaveSubModel;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<RepositoryResponse<List<TView>>>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<BaseRestApiController<TDbContext, TModel, TView, TRead>.u003cSaveListAsyncu003ed__29>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(string id, [FromBody] TView data)
		{
			V_0.u003cu003e4__this = this;
			V_0.id = id;
			V_0.data = data;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder<IActionResult>.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<BaseRestApiController<TDbContext, TModel, TView, TRead>.u003cUpdateu003ed__11>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}