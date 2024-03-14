using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Squidex.Areas.Api.Controllers.Plans.Models;
using Squidex.Domain.Apps.Entities.Billing;
using Squidex.Domain.Apps.Entities.Teams.Commands;
using Squidex.Infrastructure.Commands;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Tasks;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.Plans
{
	[ApiExplorerSettings(GroupName="Plans")]
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class TeamPlansController : ApiController
	{
		private readonly IUsageGate appUsageGate;

		private readonly IBillingPlans billingPlans;

		private readonly IBillingManager billingManager;

		public TeamPlansController(ICommandBus commandBus, IUsageGate appUsageGate, IBillingPlans billingPlans, IBillingManager billingManager) : base(commandBus)
		{
			this.appUsageGate = appUsageGate;
			this.billingPlans = billingPlans;
			this.billingManager = billingManager;
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.teams.{team}.plans.read" })]
		[HttpGet]
		[ProducesResponseType(typeof(PlansDto), 200)]
		[Route("teams/{team}/plans/")]
		public IActionResult GetTeamPlans(string team)
		{
			Deferred deferred = Deferred.AsyncResponse<PlansDto>(async () => {
				IEnumerable<Plan> availablePlans = this.billingPlans.GetAvailablePlans();
				ValueTuple<ValueTuple<Plan, string>, Uri, ReferralInfo> valueTuple = await AsyncHelper.WhenAll<ValueTuple<Plan, string>, Uri, ReferralInfo>(this.appUsageGate.GetPlanForTeamAsync(base.get_Team(), base.get_HttpContext().get_RequestAborted()), this.billingManager.GetPortalLinkAsync(base.get_UserId(), base.get_Team(), base.get_HttpContext().get_RequestAborted()), this.billingManager.GetReferralInfoAsync(base.get_UserId(), base.get_Team(), base.get_HttpContext().get_RequestAborted()));
				ValueTuple<Plan, string> item1 = valueTuple.Item1;
				Uri item2 = valueTuple.Item2;
				ReferralInfo item3 = valueTuple.Item3;
				PlansDto plansDto = PlansDto.FromDomain(availablePlans.ToArray<Plan>(), null, item1.Item2, item3, item2, this.u003cGetTeamPlansu003eg__GetLockedu007c4_1());
				availablePlans = null;
				return plansDto;
			});
			base.get_Response().get_Headers().set_Item(HeaderNames.ETag, ETagExtensions.ToEtag<ITeamEntity>(base.get_Team()));
			return this.Ok(deferred);
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.teams.{team}.plans.change" })]
		[HttpPut]
		[ProducesResponseType(typeof(PlanChangedDto), 200)]
		[Route("teams/{team}/plan/")]
		public async Task<IActionResult> PutTeamPlan(string team, [FromBody] ChangePlanDto request)
		{
			string str;
			ChangePlan changePlan = SimpleMapper.Map<ChangePlanDto, ChangePlan>(request, new ChangePlan());
			CommandContext commandContext = await base.get_CommandBus().PublishAsync(changePlan, base.get_HttpContext().get_RequestAborted());
			string str1 = null;
			PlanChangedResult plainResult = commandContext.get_PlainResult() as PlanChangedResult;
			if (plainResult != null)
			{
				Uri redirectUri = plainResult.get_RedirectUri();
				if (redirectUri != null)
				{
					str = redirectUri.ToString();
				}
				else
				{
					str = null;
				}
				str1 = str;
			}
			return this.Ok(new PlanChangedDto()
			{
				RedirectUri = str1
			});
		}
	}
}