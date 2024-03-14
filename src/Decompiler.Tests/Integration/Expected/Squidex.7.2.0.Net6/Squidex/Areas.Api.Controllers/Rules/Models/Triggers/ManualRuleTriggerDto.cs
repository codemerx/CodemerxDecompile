using Squidex.Areas.Api.Controllers.Rules.Models;
using Squidex.Domain.Apps.Core.Rules;
using Squidex.Domain.Apps.Core.Rules.Triggers;
using Squidex.Infrastructure.Reflection;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Rules.Models.Triggers
{
	public sealed class ManualRuleTriggerDto : RuleTriggerDto
	{
		public ManualRuleTriggerDto()
		{
		}

		[NullableContext(1)]
		public override RuleTrigger ToTrigger()
		{
			return SimpleMapper.Map<ManualRuleTriggerDto, ManualTrigger>(this, new ManualTrigger());
		}
	}
}