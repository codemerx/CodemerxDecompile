using Squidex.Domain.Apps.Core.Rules;
using Squidex.Domain.Apps.Entities.Rules.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Reflection;
using System;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Squidex.Areas.Api.Controllers.Rules.Models
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class UpdateRuleDto
	{
		[JsonConverter(typeof(RuleActionConverter))]
		public RuleAction Action
		{
			get;
			set;
		}

		public bool? IsEnabled
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public RuleTriggerDto Trigger
		{
			get;
			set;
		}

		public UpdateRuleDto()
		{
		}

		[NullableContext(1)]
		public UpdateRule ToCommand(DomainId id)
		{
			RuleTrigger trigger;
			UpdateRule updateRule = new UpdateRule();
			updateRule.set_RuleId(id);
			UpdateRule updateRule1 = SimpleMapper.Map<UpdateRuleDto, UpdateRule>(this, updateRule);
			UpdateRule updateRule2 = updateRule1;
			RuleTriggerDto ruleTriggerDto = this.Trigger;
			if (ruleTriggerDto != null)
			{
				trigger = ruleTriggerDto.ToTrigger();
			}
			else
			{
				trigger = null;
			}
			updateRule2.set_Trigger(trigger);
			return updateRule1;
		}
	}
}