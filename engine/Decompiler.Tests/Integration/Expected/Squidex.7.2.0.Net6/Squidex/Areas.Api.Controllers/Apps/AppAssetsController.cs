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
	public sealed class AppAssetsController : ApiController
	{
		public AppAssetsController(ICommandBus commandBus) : base(commandBus)
		{
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.asset-scripts.read" })]
		[HttpGet]
		[ProducesResponseType(typeof(AssetScriptsDto), 200)]
		[Route("apps/{app}/assets/scripts")]
		public IActionResult GetAssetScripts(string app)
		{
			Deferred deferred = Deferred.Response(() => this.GetResponse(base.get_App()));
			return this.Ok(deferred);
		}

		private AssetScriptsDto GetResponse(IAppEntity result)
		{
			return AssetScriptsDto.FromDomain(result, base.get_Resources());
		}

		private async Task<AssetScriptsDto> InvokeCommandAsync(ICommand command)
		{
			CommandContext commandContext = await base.get_CommandBus().PublishAsync(command, base.get_HttpContext().get_RequestAborted());
			return this.GetResponse(commandContext.Result<IAppEntity>());
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.asset-scripts.update" })]
		[HttpPut]
		[ProducesResponseType(typeof(AssetScriptsDto), 200)]
		[Route("apps/{app}/assets/scripts")]
		public async Task<IActionResult> PutAssetScripts(string app, [FromBody] UpdateAssetScriptsDto request)
		{
			IActionResult actionResult = this.Ok(await this.InvokeCommandAsync(request.ToCommand()));
			return actionResult;
		}
	}
}