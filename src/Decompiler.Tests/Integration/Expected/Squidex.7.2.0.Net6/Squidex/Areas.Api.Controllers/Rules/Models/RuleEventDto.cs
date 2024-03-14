using NodaTime;
using Squidex.Areas.Api.Controllers.Rules;
using Squidex.Domain.Apps.Core.HandleRules;
using Squidex.Domain.Apps.Entities.Rules;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using Squidex.Web;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Rules.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class RuleEventDto : Resource
	{
		public Instant Created
		{
			get;
			set;
		}

		[LocalizedRequired]
		public string Description
		{
			get;
			set;
		}

		[LocalizedRequired]
		public string EventName
		{
			get;
			set;
		}

		public DomainId Id
		{
			get;
			set;
		}

		public RuleJobResult JobResult
		{
			get;
			set;
		}

		[Nullable(2)]
		public string LastDump
		{
			[NullableContext(2)]
			get;
			[NullableContext(2)]
			set;
		}

		public Instant? NextAttempt
		{
			get;
			set;
		}

		public int NumCalls
		{
			get;
			set;
		}

		public RuleResult Result
		{
			get;
			set;
		}

		public RuleEventDto()
		{
		}

		private RuleEventDto CreateLinks(Resources resources)
		{
			var variable = new { app = resources.get_App(), id = this.Id };
			if (resources.get_CanUpdateRuleEvents())
			{
				base.AddPutLink("update", resources.Url<RulesController>((RulesController x) => "PutEvent", variable), null);
			}
			if (resources.get_CanDeleteRuleEvents() && this.NextAttempt.HasValue)
			{
				base.AddDeleteLink("cancel", resources.Url<RulesController>((RulesController x) => "DeleteEvent", variable), null);
			}
			return this;
		}

		public static RuleEventDto FromDomain(IRuleEventEntity ruleEvent, Resources resources)
		{
			RuleEventDto ruleEventDto = new RuleEventDto();
			SimpleMapper.Map<IRuleEventEntity, RuleEventDto>(ruleEvent, ruleEventDto);
			SimpleMapper.Map<RuleJob, RuleEventDto>(ruleEvent.get_Job(), ruleEventDto);
			return ruleEventDto.CreateLinks(resources);
		}
	}
}