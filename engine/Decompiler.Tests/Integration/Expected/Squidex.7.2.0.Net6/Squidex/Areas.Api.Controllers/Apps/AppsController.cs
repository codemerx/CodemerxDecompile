using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Squidex.Areas.Api.Controllers.Apps.Models;
using Squidex.Domain.Apps.Entities;
using Squidex.Domain.Apps.Entities.Apps;
using Squidex.Domain.Apps.Entities.Apps.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Commands;
using Squidex.Infrastructure.Security;
using Squidex.Infrastructure.Translations;
using Squidex.Infrastructure.Validation;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.Apps
{
	[ApiExplorerSettings(GroupName="Apps")]
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class AppsController : ApiController
	{
		private readonly IAppProvider appProvider;

		public AppsController(ICommandBus commandBus, IAppProvider appProvider) : base(commandBus)
		{
			this.appProvider = appProvider;
		}

		private UploadAppImage CreateCommand([Nullable(2)] IFormFile file)
		{
			if (file == null || base.get_Request().get_Form().get_Files().Count != 1)
			{
				throw new ValidationException(T.Get("validation.onlyOneFile", null), null);
			}
			UploadAppImage uploadAppImage = new UploadAppImage();
			uploadAppImage.set_File(Squidex.Web.FileExtensions.ToAssetFile(file));
			return uploadAppImage;
		}

		[ApiCosts(0)]
		[ApiPermission(new string[] { "squidex.apps.{app}.delete" })]
		[HttpDelete]
		[Route("apps/{app}/")]
		public async Task<IActionResult> DeleteApp(string app)
		{
			DeleteApp deleteApp = new DeleteApp();
			await base.get_CommandBus().PublishAsync(deleteApp, base.get_HttpContext().get_RequestAborted());
			return this.NoContent();
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.image" })]
		[HttpDelete]
		[ProducesResponseType(typeof(AppDto), 200)]
		[Route("apps/{app}/image")]
		public async Task<IActionResult> DeleteImage(string app)
		{
			IActionResult actionResult = this.Ok(await this.InvokeCommandAsync(new RemoveAppImage()));
			return actionResult;
		}

		[ApiCosts(0)]
		[ApiPermission(new string[] {  })]
		[HttpGet]
		[ProducesResponseType(typeof(AppDto), 200)]
		[Route("apps/{app}")]
		public IActionResult GetApp(string app)
		{
			Deferred deferred = Deferred.Response(() => {
				Squidex.Infrastructure.Security.Extensions.IsInClient(base.get_HttpContext().get_User(), "squidex-frontend");
				return AppDto.FromDomain(base.get_App(), base.get_UserOrClientId(), base.get_IsFrontend(), base.get_Resources());
			});
			base.get_Response().get_Headers().set_Item(HeaderNames.ETag, ETagExtensions.ToEtag<IAppEntity>(base.get_App()));
			return this.Ok(deferred);
		}

		[ApiCosts(0)]
		[ApiPermission(new string[] {  })]
		[HttpGet]
		[ProducesResponseType(typeof(AppDto[]), 200)]
		[Route("apps/")]
		public async Task<IActionResult> GetApps()
		{
			Func<IAppEntity, AppDto> func2 = null;
			string userOrClientId = base.get_UserOrClientId();
			PermissionSet userPermissions = base.get_Resources().get_Context().get_UserPermissions();
			List<IAppEntity> userAppsAsync = await this.appProvider.GetUserAppsAsync(userOrClientId, userPermissions, base.get_HttpContext().get_RequestAborted());
			List<IAppEntity> appEntities2 = userAppsAsync;
			Deferred deferred = Deferred.Response(() => {
				List<IAppEntity> appEntities = appEntities2;
				Func<IAppEntity, string> u003cu003e9_21 = AppsController.u003cu003ec.u003cu003e9__2_1;
				if (u003cu003e9_21 == null)
				{
					u003cu003e9_21 = (IAppEntity x) => x.get_Name();
					AppsController.u003cu003ec.u003cu003e9__2_1 = u003cu003e9_21;
				}
				IOrderedEnumerable<IAppEntity> appEntities1 = appEntities.OrderBy<IAppEntity, string>(u003cu003e9_21);
				Func<IAppEntity, AppDto> u003cu003e9_2 = func2;
				if (u003cu003e9_2 == null)
				{
					Func<IAppEntity, AppDto> func = (IAppEntity a) => AppDto.FromDomain(a, userOrClientId, base.get_IsFrontend(), base.get_Resources());
					Func<IAppEntity, AppDto> func1 = func;
					func2 = func;
					u003cu003e9_2 = func1;
				}
				return appEntities1.Select<IAppEntity, AppDto>(u003cu003e9_2).ToArray<AppDto>();
			});
			base.get_Response().get_Headers().set_Item(HeaderNames.ETag, ETagExtensions.ToEtag<IAppEntity>(appEntities2));
			IActionResult actionResult = this.Ok(deferred);
			return actionResult;
		}

		[ApiCosts(0)]
		[ApiPermission(new string[] {  })]
		[HttpGet]
		[ProducesResponseType(typeof(AppDto[]), 200)]
		[Route("teams/{team}/apps")]
		public async Task<IActionResult> GetTeamApps(string team)
		{
			Func<IAppEntity, AppDto> func2 = null;
			List<IAppEntity> teamAppsAsync = await this.appProvider.GetTeamAppsAsync(base.get_Team().get_Id(), base.get_HttpContext().get_RequestAborted());
			List<IAppEntity> appEntities2 = teamAppsAsync;
			Deferred deferred = Deferred.Response(() => {
				List<IAppEntity> appEntities = appEntities2;
				Func<IAppEntity, string> u003cu003e9_31 = AppsController.u003cu003ec.u003cu003e9__3_1;
				if (u003cu003e9_31 == null)
				{
					u003cu003e9_31 = (IAppEntity x) => x.get_Name();
					AppsController.u003cu003ec.u003cu003e9__3_1 = u003cu003e9_31;
				}
				IOrderedEnumerable<IAppEntity> appEntities1 = appEntities.OrderBy<IAppEntity, string>(u003cu003e9_31);
				Func<IAppEntity, AppDto> u003cu003e9_2 = func2;
				if (u003cu003e9_2 == null)
				{
					Func<IAppEntity, AppDto> func = (IAppEntity a) => AppDto.FromDomain(a, base.get_UserOrClientId(), base.get_IsFrontend(), base.get_Resources());
					Func<IAppEntity, AppDto> func1 = func;
					func2 = func;
					u003cu003e9_2 = func1;
				}
				return appEntities1.Select<IAppEntity, AppDto>(u003cu003e9_2).ToArray<AppDto>();
			});
			base.get_Response().get_Headers().set_Item(HeaderNames.ETag, ETagExtensions.ToEtag<IAppEntity>(appEntities2));
			IActionResult actionResult = this.Ok(deferred);
			return actionResult;
		}

		private Task<AppDto> InvokeCommandAsync(ICommand command)
		{
			return this.InvokeCommandAsync<AppDto>(command, (IAppEntity x) => AppDto.FromDomain(x, base.get_UserOrClientId(), base.get_IsFrontend(), base.get_Resources()));
		}

		private async Task<T> InvokeCommandAsync<T>(ICommand command, Func<IAppEntity, T> converter)
		{
			IAppEntity appEntity = await base.get_CommandBus().PublishAsync(command, base.get_HttpContext().get_RequestAborted()).Result<IAppEntity>();
			return converter(appEntity);
		}

		[ApiCosts(0)]
		[ApiPermission(new string[] {  })]
		[HttpPost]
		[ProducesResponseType(typeof(AppDto), 201)]
		[Route("apps/")]
		public async Task<IActionResult> PostApp([FromBody] CreateAppDto request)
		{
			AppDto appDto = await this.InvokeCommandAsync(request.ToCommand());
			return this.CreatedAtAction("GetApps", appDto);
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.update" })]
		[HttpPut]
		[ProducesResponseType(typeof(AppDto), 200)]
		[Route("apps/{app}/")]
		public async Task<IActionResult> PutApp(string app, [FromBody] UpdateAppDto request)
		{
			IActionResult actionResult = this.Ok(await this.InvokeCommandAsync(request.ToCommand()));
			return actionResult;
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.transfer" })]
		[HttpPut]
		[ProducesResponseType(typeof(AppDto), 200)]
		[Route("apps/{app}/team")]
		public async Task<IActionResult> PutAppTeam(string app, [FromBody] TransferToTeamDto request)
		{
			IActionResult actionResult = this.Ok(await this.InvokeCommandAsync(request.ToCommand()));
			return actionResult;
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.image" })]
		[HttpPost]
		[ProducesResponseType(typeof(AppDto), 200)]
		[Route("apps/{app}/image")]
		public async Task<IActionResult> UploadImage(string app, IFormFile file)
		{
			IActionResult actionResult = this.Ok(await this.InvokeCommandAsync(this.CreateCommand(file)));
			return actionResult;
		}
	}
}