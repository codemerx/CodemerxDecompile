using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Squidex.Areas.Api.Controllers;
using Squidex.Domain.Apps.Entities.Invitation;
using Squidex.Domain.Apps.Entities.Teams;
using Squidex.Domain.Apps.Entities.Teams.Commands;
using Squidex.Infrastructure.Commands;
using Squidex.Infrastructure.Reflection;
using Squidex.Shared.Users;
using Squidex.Web;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.Teams
{
	[ApiExplorerSettings(GroupName="Teams")]
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class TeamContributorsController : ApiController
	{
		private readonly IUserResolver userResolver;

		public TeamContributorsController(ICommandBus commandBus, IUserResolver userResolver) : base(commandBus)
		{
			this.userResolver = userResolver;
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.teams.{team}.contributors.revoke" })]
		[HttpDelete]
		[ProducesResponseType(typeof(ContributorsDto), 200)]
		[Route("teams/{team}/contributors/{id}/")]
		public async Task<IActionResult> DeleteContributor(string team, string id)
		{
			RemoveContributor removeContributor = new RemoveContributor();
			removeContributor.set_ContributorId(id);
			return this.Ok(await this.InvokeCommandAsync(removeContributor));
		}

		[ApiCosts(1)]
		[ApiPermission(new string[] {  })]
		[HttpDelete]
		[ProducesResponseType(typeof(ContributorsDto), 200)]
		[Route("teams/{team}/contributors/me/")]
		public async Task<IActionResult> DeleteMyself(string team)
		{
			RemoveContributor removeContributor = new RemoveContributor();
			removeContributor.set_ContributorId(base.get_UserId());
			return this.Ok(await this.InvokeCommandAsync(removeContributor));
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.teams.{team}.contributors.read" })]
		[HttpGet]
		[ProducesResponseType(typeof(ContributorsDto), 200)]
		[Route("teams/{team}/contributors/")]
		public IActionResult GetContributors(string team)
		{
			Deferred deferred = Deferred.AsyncResponse<ContributorsDto>(() => this.GetResponseAsync(base.get_Team(), false));
			base.get_Response().get_Headers().set_Item(HeaderNames.ETag, ETagExtensions.ToEtag<ITeamEntity>(base.get_Team()));
			return this.Ok(deferred);
		}

		private async Task<ContributorsDto> GetResponseAsync(ITeamEntity team, bool invited)
		{
			ContributorsDto contributorsDto = await ContributorsDto.FromDomainAsync(team, base.get_Resources(), this.userResolver, invited);
			return contributorsDto;
		}

		private async Task<ContributorsDto> InvokeCommandAsync(ICommand command)
		{
			ContributorsDto contributorsDto;
			CommandContext commandContext = await base.get_CommandBus().PublishAsync(command, base.get_HttpContext().get_RequestAborted());
			InvitedResult<ITeamEntity> plainResult = commandContext.get_PlainResult() as InvitedResult<ITeamEntity>;
			contributorsDto = (plainResult == null ? await this.GetResponseAsync(commandContext.Result<ITeamEntity>(), false) : await this.GetResponseAsync(plainResult.get_Entity(), true));
			return contributorsDto;
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.teams.{team}.contributors.assign" })]
		[HttpPost]
		[ProducesResponseType(typeof(ContributorsDto), 201)]
		[Route("teams/{team}/contributors/")]
		public async Task<IActionResult> PostContributor(string team, [FromBody] AssignContributorDto request)
		{
			AssignContributor assignContributor = SimpleMapper.Map<AssignContributorDto, AssignContributor>(request, new AssignContributor());
			ContributorsDto contributorsDto = await this.InvokeCommandAsync(assignContributor);
			IActionResult actionResult = this.CreatedAtAction("GetContributors", new { team = team }, contributorsDto);
			return actionResult;
		}
	}
}