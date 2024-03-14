using Squidex.Domain.Apps.Core.HandleRules;
using Squidex.Domain.Apps.Entities.Rules.Runner;
using Squidex.Infrastructure.Reflection;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text;

namespace Squidex.Areas.Api.Controllers.Rules.Models
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class SimulatedRuleEventDto : IEquatable<SimulatedRuleEventDto>
	{
		public string ActionData
		{
			get;
			set;
		}

		public string ActionName
		{
			get;
			set;
		}

		public object EnrichedEvent
		{
			get;
			set;
		}

		[CompilerGenerated]
		[Nullable(1)]
		private Type EqualityContract
		{
			[NullableContext(1)]
			get
			{
				return typeof(SimulatedRuleEventDto);
			}
		}

		public string Error
		{
			get;
			set;
		}

		[Nullable(1)]
		[Required]
		public object Event
		{
			[NullableContext(1)]
			get;
			[NullableContext(1)]
			set;
		}

		[Required]
		public Guid EventId
		{
			get;
			set;
		}

		[Nullable(1)]
		[Required]
		public string EventName
		{
			[NullableContext(1)]
			get;
			[NullableContext(1)]
			set;
		}

		[Nullable(1)]
		[Required]
		public List<SkipReason> SkipReasons
		{
			[NullableContext(1)]
			get;
			[NullableContext(1)]
			set;
		}

		public SimulatedRuleEventDto()
		{
		}

		[NullableContext(1)]
		public static SimulatedRuleEventDto FromDomain(SimulatedRuleEvent ruleEvent)
		{
			SimulatedRuleEventDto simulatedRuleEventDto = SimpleMapper.Map<SimulatedRuleEvent, SimulatedRuleEventDto>(ruleEvent, new SimulatedRuleEventDto()
			{
				SkipReasons = new List<SkipReason>()
			});
			SkipReason[] values = Enum.GetValues<SkipReason>();
			for (int i = 0; i < (int)values.Length; i++)
			{
				SkipReason skipReason = values[i];
				if (skipReason != null && ruleEvent.get_SkipReason().HasFlag(skipReason))
				{
					simulatedRuleEventDto.SkipReasons.Add(skipReason);
				}
			}
			return simulatedRuleEventDto;
		}
	}
}