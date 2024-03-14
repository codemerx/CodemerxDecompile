using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Squidex.Areas.Api.Controllers.Apps.Models;
using Squidex.Domain.Apps.Entities.Apps;
using Squidex.Infrastructure.Commands;
using Squidex.Web;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.Apps
{
	[ApiExplorerSettings(GroupName="Apps")]
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class AppSettingsController : ApiController
	{
		public AppSettingsController(ICommandBus commandBus) : base(commandBus)
		{
		}

		private AppSettingsDto GetResponse(IAppEntity result)
		{
			return AppSettingsDto.FromDomain(result, base.get_Resources());
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] {  })]
		[HttpGet]
		[ProducesResponseType(typeof(AppSettingsDto), 200)]
		[Route("apps/{app}/settings")]
		public IActionResult GetSettings(string app)
		{
			Deferred deferred = Deferred.Response(() => this.GetResponse(base.get_App()));
			return this.Ok(deferred);
		}

		private async Task<AppSettingsDto> InvokeCommandAsync(ICommand command)
		{
			CommandContext commandContext = await base.get_CommandBus().PublishAsync(command, base.get_HttpContext().get_RequestAborted());
			return this.GetResponse(commandContext.Result<IAppEntity>());
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.settings" })]
		[HttpPut]
		[ProducesResponseType(typeof(AppSettingsDto), 200)]
		[Route("apps/{app}/settings")]
		public async Task<IActionResult> PutSettings(string app, [FromBody] UpdateAppSettingsDto request)
		{
			IActionResult actionResult = this.Ok(await this.InvokeCommandAsync(request.ToCommand()));
			return actionResult;
		}
	}
}