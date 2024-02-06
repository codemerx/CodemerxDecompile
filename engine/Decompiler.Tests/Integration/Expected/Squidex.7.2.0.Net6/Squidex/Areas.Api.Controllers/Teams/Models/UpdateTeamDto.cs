using Squidex.Domain.Apps.Entities.Teams.Commands;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Teams.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class UpdateTeamDto
	{
		[LocalizedRequired]
		public string Name
		{
			get;
			set;
		}

		public UpdateTeamDto()
		{
		}

		public UpdateTeam ToCommand()
		{
			return SimpleMapper.Map<UpdateTeamDto, UpdateTeam>(this, new UpdateTeam());
		}
	}
}