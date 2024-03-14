using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Squidex.Areas.Api.Controllers.Apps.Models;
using Squidex.Domain.Apps.Entities.Apps;
using Squidex.Domain.Apps.Entities.Apps.Commands;
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
	public sealed class AppClientsController : ApiController
	{
		public AppClientsController(ICommandBus commandBus) : base(commandBus)
		{
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.clients.delete" })]
		[HttpDelete]
		[ProducesResponseType(typeof(ClientsDto), 200)]
		[Route("apps/{app}/clients/{id}/")]
		public async Task<IActionResult> DeleteClient(string app, string id)
		{
			RevokeClient revokeClient = new RevokeClient();
			revokeClient.set_Id(id);
			return this.Ok(await this.InvokeCommandAsync(revokeClient));
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.clients.read" })]
		[HttpGet]
		[ProducesResponseType(typeof(ClientsDto), 200)]
		[Route("apps/{app}/clients/")]
		public IActionResult GetClients(string app)
		{
			Deferred deferred = Deferred.Response(() => this.GetResponse(base.get_App()));
			base.get_Response().get_Headers().set_Item(HeaderNames.ETag, ETagExtensions.ToEtag<IAppEntity>(base.get_App()));
			return this.Ok(deferred);
		}

		private ClientsDto GetResponse(IAppEntity app)
		{
			return ClientsDto.FromApp(app, base.get_Resources());
		}

		private async Task<ClientsDto> InvokeCommandAsync(ICommand command)
		{
			CommandContext commandContext = await base.get_CommandBus().PublishAsync(command, base.get_HttpContext().get_RequestAborted());
			return this.GetResponse(commandContext.Result<IAppEntity>());
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.clients.create" })]
		[HttpPost]
		[ProducesResponseType(typeof(ClientsDto), 201)]
		[Route("apps/{app}/clients/")]
		public async Task<IActionResult> PostClient(string app, [FromBody] CreateClientDto request)
		{
			ClientsDto clientsDto = await this.InvokeCommandAsync(request.ToCommand());
			IActionResult actionResult = this.CreatedAtAction("GetClients", new { app = app }, clientsDto);
			return actionResult;
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.clients.update" })]
		[HttpPut]
		[ProducesResponseType(typeof(ClientsDto), 200)]
		[Route("apps/{app}/clients/{id}/")]
		public async Task<IActionResult> PutClient(string app, string id, [FromBody] UpdateClientDto request)
		{
			UpdateClient command = request.ToCommand(id);
			return this.Ok(await this.InvokeCommandAsync(command));
		}
	}
}