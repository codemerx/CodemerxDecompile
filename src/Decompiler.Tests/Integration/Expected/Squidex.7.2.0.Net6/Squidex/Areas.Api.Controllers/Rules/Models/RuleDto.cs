using NodaTime;
using Squidex.Areas.Api.Controllers.Rules;
using Squidex.Areas.Api.Controllers.Rules.Models.Converters;
using Squidex.Domain.Apps.Core.Rules;
using Squidex.Domain.Apps.Entities.Rules;
using Squidex.Domain.Apps.Entities.Rules.Runner;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using Squidex.Web;
using System;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace Squidex.Areas.Api.Controllers.Rules.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class RuleDto : Resource
	{
		[JsonConverter(typeof(RuleActionConverter))]
		[LocalizedRequired]
		public RuleAction Action
		{
			get;
			set;
		}

		public Instant Created
		{
			get;
			set;
		}

		[LocalizedRequired]
		public RefToken CreatedBy
		{
			get;
			set;
		}

		public DomainId Id
		{
			get;
			set;
		}

		public bool IsEnabled
		{
			get;
			set;
		}

		public Instant? LastExecuted
		{
			get;
			set;
		}

		public Instant LastModified
		{
			get;
			set;
		}

		[LocalizedRequired]
		public RefToken LastModifiedBy
		{
			get;
			set;
		}

		[Nullable(2)]
		public string Name
		{
			[NullableContext(2)]
			get;
			[NullableContext(2)]
			set;
		}

		public int NumFailed
		{
			get;
			set;
		}

		public int NumSucceeded
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

		public long Version
		{
			get;
			set;
		}

		public RuleDto()
		{
		}

		private RuleDto CreateLinks(Resources resources, IEnrichedRuleEntity rule, bool canRun, IRuleRunnerService ruleRunnerService)
		{
			var variable = new { app = resources.get_App(), id = this.Id };
			if (resources.get_CanDisableRule())
			{
				if (!this.IsEnabled)
				{
					base.AddPutLink("enable", resources.Url<RulesController>((RulesController x) => "EnableRule", variable), null);
				}
				else
				{
					base.AddPutLink("disable", resources.Url<RulesController>((RulesController x) => "DisableRule", variable), null);
				}
			}
			if (resources.get_CanUpdateRule())
			{
				base.AddPutLink("update", resources.Url<RulesController>((RulesController x) => "PutRule", variable), null);
			}
			if (resources.get_CanRunRuleEvents())
			{
				base.AddPutLink("trigger", resources.Url<RulesController>((RulesController x) => "TriggerRule", variable), null);
				if (canRun && ruleRunnerService.CanRunRule(rule))
				{
					base.AddPutLink("run", resources.Url<RulesController>((RulesController x) => "PutRuleRun", variable), null);
				}
				if (canRun && ruleRunnerService.CanRunFromSnapshots(rule))
				{
					var variable1 = new { app = variable.app, id = variable.id, fromSnapshots = true };
					base.AddPutLink("run/snapshots", resources.Url<RulesController>((RulesController x) => "PutRuleRun", variable1), null);
				}
			}
			if (resources.get_CanReadRuleEvents())
			{
				base.AddGetLink("logs", resources.Url<RulesController>((RulesController x) => "GetEvents", variable), null);
			}
			if (resources.get_CanDeleteRule())
			{
				base.AddDeleteLink("delete", resources.Url<RulesController>((RulesController x) => "DeleteRule", variable), null);
			}
			return this;
		}

		public static RuleDto FromDomain(IEnrichedRuleEntity rule, bool canRun, IRuleRunnerService ruleRunnerService, Resources resources)
		{
			RuleDto ruleDto = new RuleDto();
			SimpleMapper.Map<IEnrichedRuleEntity, RuleDto>(rule, ruleDto);
			SimpleMapper.Map<Rule, RuleDto>(rule.get_RuleDef(), ruleDto);
			if (rule.get_RuleDef().get_Trigger() != null)
			{
				ruleDto.Trigger = RuleTriggerDtoFactory.Create(rule.get_RuleDef().get_Trigger());
			}
			return ruleDto.CreateLinks(resources, rule, canRun, ruleRunnerService);
		}
	}
}