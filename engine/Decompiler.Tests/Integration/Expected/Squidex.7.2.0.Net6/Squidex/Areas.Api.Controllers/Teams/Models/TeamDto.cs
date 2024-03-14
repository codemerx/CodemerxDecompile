using NodaTime;
using Squidex.Areas.Api.Controllers.Plans;
using Squidex.Areas.Api.Controllers.Teams;
using Squidex.Domain.Apps.Entities.Teams;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Security;
using Squidex.Infrastructure.Validation;
using Squidex.Shared;
using Squidex.Web;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Teams.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class TeamDto : Resource
	{
		public Instant Created
		{
			get;
			set;
		}

		public DomainId Id
		{
			get;
			set;
		}

		public Instant LastModified
		{
			get;
			set;
		}

		[LocalizedRequired]
		public string Name
		{
			get;
			set;
		}

		[Nullable(2)]
		public string RoleName
		{
			[NullableContext(2)]
			get;
			[NullableContext(2)]
			set;
		}

		public long Version
		{
			get;
			set;
		}

		public TeamDto()
		{
		}

		private TeamDto CreateLinks(ITeamEntity team, Resources resources, PermissionSet permissions, bool isContributor)
		{
			DomainId id = this.Id;
			var variable = new { team = id.ToString() };
			if (isContributor)
			{
				base.AddDeleteLink("leave", resources.Url<TeamContributorsController>((TeamContributorsController x) => "DeleteMyself", variable), null);
			}
			if (resources.IsAllowed("squidex.teams.{team}.update", "*", "*", variable.team, permissions))
			{
				base.AddPutLink("update", resources.Url<TeamsController>((TeamsController x) => "PutTeam", variable), null);
			}
			if (resources.IsAllowed("squidex.teams.{team}.contributors.read", "*", "*", variable.team, permissions))
			{
				base.AddGetLink("contributors", resources.Url<TeamContributorsController>((TeamContributorsController x) => "GetContributors", variable), null);
			}
			if (resources.IsAllowed("squidex.teams.{team}.plans.read", "*", "*", variable.team, permissions))
			{
				base.AddGetLink("plans", resources.Url<TeamPlansController>((TeamPlansController x) => "GetTeamPlans", variable), null);
			}
			return this;
		}

		public static TeamDto FromDomain(ITeamEntity team, string userId, Resources resources)
		{
			string str = null;
			TeamDto teamDto = SimpleMapper.Map<ITeamEntity, TeamDto>(team, new TeamDto());
			PermissionSet empty = PermissionSet.Empty;
			if (TeamExtensions.TryGetContributorRole(team, userId, ref str))
			{
				Permission[] permissionArray = new Permission[1];
				DomainId id = team.get_Id();
				permissionArray[0] = PermissionIds.ForApp("squidex.teams.{team}.*", "*", "*", id.ToString());
				empty = new PermissionSet(permissionArray);
			}
			return teamDto.CreateLinks(team, resources, empty, true);
		}
	}
}