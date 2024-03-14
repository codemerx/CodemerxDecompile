using Squidex.Domain.Apps.Entities.Rules.Runner;
using Squidex.Infrastructure.Validation;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Rules.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class SimulatedRuleEventsDto : Resource
	{
		[LocalizedRequired]
		public SimulatedRuleEventDto[] Items
		{
			get;
			set;
		}

		public long Total
		{
			get;
			set;
		}

		public SimulatedRuleEventsDto()
		{
		}

		public static SimulatedRuleEventsDto FromDomain(IList<SimulatedRuleEvent> events)
		{
			SimulatedRuleEventsDto simulatedRuleEventsDto = new SimulatedRuleEventsDto()
			{
				Total = (long)events.Count,
				Items = events.Select<SimulatedRuleEvent, SimulatedRuleEventDto>(new Func<SimulatedRuleEvent, SimulatedRuleEventDto>(SimulatedRuleEventDto.FromDomain)).ToArray<SimulatedRuleEventDto>()
			};
			return simulatedRuleEventsDto;
		}
	}
}