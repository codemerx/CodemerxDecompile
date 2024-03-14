using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Squidex.Areas.Api.Controllers.Teams.Models;
using Squidex.Domain.Apps.Entities;
using Squidex.Domain.Apps.Entities.Teams;
using Squidex.Infrastructure.Commands;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.Teams
{
	[ApiExplorerSettings(GroupName="Teams")]
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class TeamsController : ApiController
	{
		private readonly IAppProvider appProvider;

		public TeamsController(ICommandBus commandBus, IAppProvider appProvider) : base(commandBus)
		{
			this.appProvider = appProvider;
		}

		[ApiCosts(0)]
		[ApiPermission(new string[] {  })]
		[HttpGet]
		[ProducesResponseType(typeof(TeamDto), 200)]
		[Route("teams/{team}")]
		public IActionResult GetTeam(string team)
		{
			Deferred deferred = Deferred.Response(() => TeamDto.FromDomain(base.get_Team(), base.get_UserOrClientId(), base.get_Resources()));
			base.get_Response().get_Headers().set_Item(HeaderNames.ETag, ETagExtensions.ToEtag<ITeamEntity>(base.get_Team()));
			return this.Ok(deferred);
		}

		[ApiCosts(0)]
		[ApiPermission(new string[] {  })]
		[HttpGet]
		[ProducesResponseType(typeof(TeamDto[]), 200)]
		[Route("teams/")]
		public async Task<IActionResult> GetTeams()
		{
			Func<ITeamEntity, TeamDto> func2 = null;
			List<ITeamEntity> userTeamsAsync = await this.appProvider.GetUserTeamsAsync(base.get_UserOrClientId(), base.get_HttpContext().get_RequestAborted());
			List<ITeamEntity> teamEntities2 = userTeamsAsync;
			Deferred deferred = Deferred.Response(() => {
				List<ITeamEntity> teamEntities = teamEntities2;
				Func<ITeamEntity, string> u003cu003e9_21 = TeamsController.u003cu003ec.u003cu003e9__2_1;
				if (u003cu003e9_21 == null)
				{
					u003cu003e9_21 = (ITeamEntity x) => x.get_Name();
					TeamsController.u003cu003ec.u003cu003e9__2_1 = u003cu003e9_21;
				}
				IOrderedEnumerable<ITeamEntity> teamEntities1 = teamEntities.OrderBy<ITeamEntity, string>(u003cu003e9_21);
				Func<ITeamEntity, TeamDto> u003cu003e9_2 = func2;
				if (u003cu003e9_2 == null)
				{
					Func<ITeamEntity, TeamDto> func = (ITeamEntity a) => TeamDto.FromDomain(a, base.get_UserOrClientId(), base.get_Resources());
					Func<ITeamEntity, TeamDto> func1 = func;
					func2 = func;
					u003cu003e9_2 = func1;
				}
				return teamEntities1.Select<ITeamEntity, TeamDto>(u003cu003e9_2).ToArray<TeamDto>();
			});
			base.get_Response().get_Headers().set_Item(HeaderNames.ETag, ETagExtensions.ToEtag<ITeamEntity>(teamEntities2));
			IActionResult actionResult = this.Ok(deferred);
			return actionResult;
		}

		private Task<TeamDto> InvokeCommandAsync(ICommand command)
		{
			return this.InvokeCommandAsync<TeamDto>(command, (ITeamEntity x) => TeamDto.FromDomain(x, base.get_UserOrClientId(), base.get_Resources()));
		}

		private async Task<T> InvokeCommandAsync<T>(ICommand command, Func<ITeamEntity, T> converter)
		{
			ITeamEntity teamEntity = await base.get_CommandBus().PublishAsync(command, base.get_HttpContext().get_RequestAborted()).Result<ITeamEntity>();
			return converter(teamEntity);
		}

		[ApiCosts(0)]
		[ApiPermission(new string[] {  })]
		[HttpPost]
		[ProducesResponseType(typeof(TeamDto), 201)]
		[Route("teams/")]
		public async Task<IActionResult> PostTeam([FromBody] CreateTeamDto request)
		{
			TeamDto teamDto = await this.InvokeCommandAsync(request.ToCommand());
			return this.CreatedAtAction("GetTeams", teamDto);
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.teams.{team}.update" })]
		[HttpPut]
		[ProducesResponseType(typeof(TeamDto), 200)]
		[Route("teams/{team}/")]
		public async Task<IActionResult> PutTeam(string team, [FromBody] UpdateTeamDto request)
		{
			IActionResult actionResult = this.Ok(await this.InvokeCommandAsync(request.ToCommand()));
			return actionResult;
		}
	}
}