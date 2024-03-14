using Microsoft.Extensions.DependencyInjection;
using Squidex.Domain.Teams.Entities.Teams;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Config.Domain
{
	public static class TeamServices
	{
		[NullableContext(1)]
		public static void AddSquidexTeams(IServiceCollection services)
		{
			DependencyInjectionExtensions.AddSingletonAs<TeamHistoryEventsCreator>(services).As<IHistoryEventsCreator>();
		}
	}
}