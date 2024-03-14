using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Squidex.Areas.Api.Controllers;
using Squidex.Domain.Apps.Entities.Apps;
using Squidex.Domain.Apps.Entities.Apps.Commands;
using Squidex.Domain.Apps.Entities.Billing;
using Squidex.Domain.Apps.Entities.Invitation;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Commands;
using Squidex.Infrastructure.Reflection;
using Squidex.Shared.Users;
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
	public sealed class AppContributorsController : ApiController
	{
		private readonly IUsageGate usageGate;

		private readonly IUserResolver userResolver;

		public AppContributorsController(ICommandBus commandBus, IUsageGate usageGate, IUserResolver userResolver) : base(commandBus)
		{
			this.usageGate = usageGate;
			this.userResolver = userResolver;
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.contributors.revoke" })]
		[HttpDelete]
		[ProducesResponseType(typeof(ContributorsDto), 200)]
		[Route("apps/{app}/contributors/{id}/")]
		public async Task<IActionResult> DeleteContributor(string app, string id)
		{
			RemoveContributor removeContributor = new RemoveContributor();
			removeContributor.set_ContributorId(id);
			return this.Ok(await this.InvokeCommandAsync(removeContributor));
		}

		[ApiCosts(1)]
		[ApiPermission(new string[] {  })]
		[HttpDelete]
		[ProducesResponseType(typeof(ContributorsDto), 200)]
		[Route("apps/{app}/contributors/me/")]
		public async Task<IActionResult> DeleteMyself(string app)
		{
			RemoveContributor removeContributor = new RemoveContributor();
			removeContributor.set_ContributorId(base.get_UserId());
			return this.Ok(await this.InvokeCommandAsync(removeContributor));
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.contributors.read" })]
		[HttpGet]
		[ProducesResponseType(typeof(ContributorsDto), 200)]
		[Route("apps/{app}/contributors/")]
		public IActionResult GetContributors(string app)
		{
			Deferred deferred = Deferred.AsyncResponse<ContributorsDto>(() => this.GetResponseAsync(base.get_App(), false));
			base.get_Response().get_Headers().set_Item(HeaderNames.ETag, ETagExtensions.ToEtag<IAppEntity>(base.get_App()));
			return this.Ok(deferred);
		}

		private async Task<ContributorsDto> GetResponseAsync(IAppEntity app, bool invited)
		{
			Plan item1 = await this.usageGate.GetPlanForAppAsync(app, base.get_HttpContext().get_RequestAborted()).Item1;
			ContributorsDto contributorsDto = await ContributorsDto.FromDomainAsync(app, base.get_Resources(), this.userResolver, item1, invited);
			return contributorsDto;
		}

		private async Task<ContributorsDto> InvokeCommandAsync(ICommand command)
		{
			ContributorsDto contributorsDto;
			CommandContext commandContext = await base.get_CommandBus().PublishAsync(command, base.get_HttpContext().get_RequestAborted());
			InvitedResult<IAppEntity> plainResult = commandContext.get_PlainResult() as InvitedResult<IAppEntity>;
			contributorsDto = (plainResult == null ? await this.GetResponseAsync(commandContext.Result<IAppEntity>(), false) : await this.GetResponseAsync(plainResult.get_Entity(), true));
			return contributorsDto;
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.contributors.assign" })]
		[HttpPost]
		[ProducesResponseType(typeof(ContributorsDto), 201)]
		[Route("apps/{app}/contributors/")]
		public async Task<IActionResult> PostContributor(string app, [FromBody] AssignContributorDto request)
		{
			AssignContributor assignContributor = SimpleMapper.Map<AssignContributorDto, AssignContributor>(request, new AssignContributor());
			ContributorsDto contributorsDto = await this.InvokeCommandAsync(assignContributor);
			IActionResult actionResult = this.CreatedAtAction("GetContributors", new { app = app }, contributorsDto);
			return actionResult;
		}
	}
}