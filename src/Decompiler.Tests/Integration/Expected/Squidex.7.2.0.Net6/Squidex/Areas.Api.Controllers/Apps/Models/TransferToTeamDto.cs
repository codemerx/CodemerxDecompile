using Squidex.Domain.Apps.Entities.Apps.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Reflection;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Apps.Models
{
	public sealed class TransferToTeamDto
	{
		public DomainId? TeamId
		{
			get;
			set;
		}

		public TransferToTeamDto()
		{
		}

		[NullableContext(1)]
		public TransferToTeam ToCommand()
		{
			return SimpleMapper.Map<TransferToTeamDto, TransferToTeam>(this, new TransferToTeam());
		}
	}
}