using Squidex.Areas.Api.Controllers.Rules.Models;
using Squidex.Domain.Apps.Core.Rules;
using Squidex.Domain.Apps.Core.Rules.Triggers;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Rules.Models.Triggers
{
	public sealed class UsageRuleTriggerDto : RuleTriggerDto
	{
		public int Limit
		{
			get;
			set;
		}

		[LocalizedRange(1, 30)]
		public int? NumDays
		{
			get;
			set;
		}

		public UsageRuleTriggerDto()
		{
		}

		[NullableContext(1)]
		public override RuleTrigger ToTrigger()
		{
			return SimpleMapper.Map<UsageRuleTriggerDto, UsageTrigger>(this, new UsageTrigger());
		}
	}
}