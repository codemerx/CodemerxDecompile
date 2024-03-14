using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Squidex.Areas.Api.Controllers.Apps.Models;
using Squidex.Domain.Apps.Entities.Apps;
using Squidex.Domain.Apps.Entities.Apps.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Commands;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.Apps
{
	[ApiExplorerSettings(GroupName="Apps")]
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class AppRolesController : ApiController
	{
		private readonly RolePermissionsProvider permissionsProvider;

		public AppRolesController(ICommandBus commandBus, RolePermissionsProvider permissionsProvider) : base(commandBus)
		{
			this.permissionsProvider = permissionsProvider;
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.roles.delete" })]
		[HttpDelete]
		[ProducesResponseType(typeof(RolesDto), 200)]
		[Route("apps/{app}/roles/{roleName}/")]
		[UrlDecodeRouteParams]
		public async Task<IActionResult> DeleteRole(string app, string roleName)
		{
			DeleteRole deleteRole = new DeleteRole();
			deleteRole.set_Name(roleName);
			return this.Ok(await this.InvokeCommandAsync(deleteRole));
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.roles.read" })]
		[HttpGet]
		[ProducesResponseType(typeof(string[]), 200)]
		[Route("apps/{app}/roles/permissions")]
		public IActionResult GetPermissions(string app)
		{
			Deferred deferred = Deferred.AsyncResponse<List<string>>(() => this.permissionsProvider.GetPermissionsAsync(base.get_App()));
			base.get_Response().get_Headers().set_Item(HeaderNames.ETag, RandomHash.ToSha256Base64(string.Concat(deferred)));
			return this.Ok(deferred);
		}

		private RolesDto GetResponse(IAppEntity result)
		{
			return RolesDto.FromDomain(result, base.get_Resources());
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.roles.read" })]
		[HttpGet]
		[ProducesResponseType(typeof(RolesDto), 200)]
		[Route("apps/{app}/roles/")]
		public IActionResult GetRoles(string app)
		{
			Deferred deferred = Deferred.Response(() => this.GetResponse(base.get_App()));
			base.get_Response().get_Headers().set_Item(HeaderNames.ETag, ETagExtensions.ToEtag<IAppEntity>(base.get_App()));
			return this.Ok(deferred);
		}

		private async Task<RolesDto> InvokeCommandAsync(ICommand command)
		{
			CommandContext commandContext = await base.get_CommandBus().PublishAsync(command, base.get_HttpContext().get_RequestAborted());
			return this.GetResponse(commandContext.Result<IAppEntity>());
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.roles.create" })]
		[HttpPost]
		[ProducesResponseType(typeof(RolesDto), 201)]
		[Route("apps/{app}/roles/")]
		public async Task<IActionResult> PostRole(string app, [FromBody] AddRoleDto request)
		{
			RolesDto rolesDto = await this.InvokeCommandAsync(request.ToCommand());
			IActionResult actionResult = this.CreatedAtAction("GetRoles", new { app = app }, rolesDto);
			return actionResult;
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.roles.update" })]
		[HttpPut]
		[ProducesResponseType(typeof(RolesDto), 200)]
		[Route("apps/{app}/roles/{roleName}/")]
		[UrlDecodeRouteParams]
		public async Task<IActionResult> PutRole(string app, string roleName, [FromBody] UpdateRoleDto request)
		{
			UpdateRole command = request.ToCommand(roleName);
			return this.Ok(await this.InvokeCommandAsync(command));
		}
	}
}