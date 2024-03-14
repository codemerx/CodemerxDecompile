using Squidex.Domain.Apps.Core.Rules;
using Squidex.Domain.Apps.Entities.Rules.Commands;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Squidex.Areas.Api.Controllers.Rules.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class CreateRuleDto
	{
		[JsonConverter(typeof(RuleActionConverter))]
		[LocalizedRequired]
		public RuleAction Action
		{
			get;
			set;
		}

		[LocalizedRequired]
		public RuleTriggerDto Trigger
		{
			get;
			set;
		}

		public CreateRuleDto()
		{
		}

		public CreateRule ToCommand()
		{
			RuleTrigger trigger;
			CreateRule createRule = new CreateRule();
			createRule.set_Action(this.Action);
			RuleTriggerDto ruleTriggerDto = this.Trigger;
			if (ruleTriggerDto != null)
			{
				trigger = ruleTriggerDto.ToTrigger();
			}
			else
			{
				trigger = null;
			}
			createRule.set_Trigger(trigger);
			return createRule;
		}

		public Rule ToRule()
		{
			return new Rule(this.Trigger.ToTrigger(), this.Action);
		}
	}
}