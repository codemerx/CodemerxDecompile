using Squidex.Areas.Api.Controllers.Rules;
using Squidex.Domain.Apps.Entities;
using Squidex.Domain.Apps.Entities.Rules;
using Squidex.Domain.Apps.Entities.Rules.Runner;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Validation;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.Rules.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class RulesDto : Resource
	{
		[LocalizedRequired]
		public RuleDto[] Items
		{
			get;
			set;
		}

		public DomainId? RunningRuleId
		{
			get;
			set;
		}

		public RulesDto()
		{
		}

		private RulesDto CreateLinks(Resources resources, DomainId? runningRuleId)
		{
			var variable = new { app = resources.get_App() };
			base.AddSelfLink(resources.Url<RulesController>((RulesController x) => "GetRules", variable));
			if (resources.get_CanCreateRule())
			{
				base.AddPostLink("create", resources.Url<RulesController>((RulesController x) => "PostRule", variable), null);
			}
			if (resources.get_CanReadRuleEvents())
			{
				base.AddGetLink("events", resources.Url<RulesController>((RulesController x) => "GetEvents", variable), null);
			}
			if (resources.get_CanDeleteRuleEvents() && runningRuleId.HasValue)
			{
				base.AddDeleteLink("run/cancel", resources.Url<RulesController>((RulesController x) => "DeleteRuleRun", variable), null);
			}
			return this;
		}

		public static async Task<RulesDto> FromRulesAsync(IEnumerable<IEnrichedRuleEntity> items, IRuleRunnerService ruleRunnerService, Resources resources)
		{
			IRuleRunnerService ruleRunnerService1 = ruleRunnerService;
			DomainId id = resources.get_Context().get_App().get_Id();
			DomainId? runningRuleIdAsync = await ruleRunnerService1.GetRunningRuleIdAsync(id, new CancellationToken());
			bool hasValue = runningRuleIdAsync.HasValue;
			RulesDto rulesDto = new RulesDto()
			{
				Items = (
					from x in items
					select RuleDto.FromDomain(x, hasValue, ruleRunnerService, resources)).ToArray<RuleDto>()
			};
			RulesDto rulesDto1 = rulesDto;
			rulesDto1.RunningRuleId = runningRuleIdAsync;
			RulesDto rulesDto2 = rulesDto1.CreateLinks(resources, runningRuleIdAsync);
			return rulesDto2;
		}
	}
}