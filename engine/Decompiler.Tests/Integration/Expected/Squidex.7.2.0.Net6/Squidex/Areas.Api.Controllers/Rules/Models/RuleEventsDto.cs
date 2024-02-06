using Squidex.Areas.Api.Controllers.Rules;
using Squidex.Domain.Apps.Entities.Rules;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Validation;
using Squidex.Web;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Rules.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class RuleEventsDto : Resource
	{
		[LocalizedRequired]
		public RuleEventDto[] Items
		{
			get;
			set;
		}

		public long Total
		{
			get;
			set;
		}

		public RuleEventsDto()
		{
		}

		private RuleEventsDto CreateLinks(Resources resources, DomainId? ruleId)
		{
			var variable = new { app = resources.get_App() };
			base.AddSelfLink(resources.Url<RulesController>((RulesController x) => "GetEvents", variable));
			if (resources.get_CanDeleteRuleEvents())
			{
				if (!ruleId.HasValue)
				{
					base.AddDeleteLink("cancel", resources.Url<RulesController>((RulesController x) => "DeleteEvents", variable), null);
				}
				else
				{
					var variable1 = new { app = variable.app, id = ruleId };
					base.AddDeleteLink("cancel", resources.Url<RulesController>((RulesController x) => "DeleteRuleEvents", variable1), null);
				}
			}
			return this;
		}

		public static RuleEventsDto FromDomain(IResultList<IRuleEventEntity> ruleEvents, Resources resources, DomainId? ruleId)
		{
			return (new RuleEventsDto()
			{
				Total = ruleEvents.get_Total(),
				Items = (
					from x in ruleEvents
					select RuleEventDto.FromDomain(x, resources)).ToArray<RuleEventDto>()
			}).CreateLinks(resources, ruleId);
		}
	}
}