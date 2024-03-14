using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Squidex.Areas.Api.Controllers.Plans.Models;
using Squidex.Domain.Apps.Core;
using Squidex.Domain.Apps.Entities.Apps;
using Squidex.Domain.Apps.Entities.Apps.Commands;
using Squidex.Domain.Apps.Entities.Billing;
using Squidex.Infrastructure;
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
	public sealed class AppPlansController : ApiController
	{
		private readonly IBillingPlans billingPlans;

		private readonly IBillingManager billingManager;

		private readonly IUsageGate usageGate;

		public AppPlansController(ICommandBus commandBus, IUsageGate usageGate, IBillingPlans billingPlans, IBillingManager billingManager) : base(commandBus)
		{
			this.billingPlans = billingPlans;
			this.billingManager = billingManager;
			this.usageGate = usageGate;
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.plans.read" })]
		[HttpGet]
		[ProducesResponseType(typeof(PlansDto), 200)]
		[Route("apps/{app}/plans/")]
		public IActionResult GetPlans(string app)
		{
			Deferred deferred = Deferred.AsyncResponse<PlansDto>(async () => {
				AppPlansController.u003cu003ec__DisplayClass4_0 item1 = new AppPlansController.u003cu003ec__DisplayClass4_0();
				string identifier;
				IEnumerable<Plan> availablePlans = this.billingPlans.GetAvailablePlans();
				ValueTuple<ValueTuple<Plan, string, DomainId?>, Uri, ReferralInfo> valueTuple = await AsyncHelper.WhenAll<ValueTuple<Plan, string, DomainId?>, Uri, ReferralInfo>(this.usageGate.GetPlanForAppAsync(base.get_App(), base.get_HttpContext().get_RequestAborted()), this.billingManager.GetPortalLinkAsync(base.get_UserId(), base.get_App(), base.get_HttpContext().get_RequestAborted()), this.billingManager.GetReferralInfoAsync(base.get_UserId(), base.get_App(), base.get_HttpContext().get_RequestAborted()));
				item1.plan = valueTuple.Item1;
				Uri item2 = valueTuple.Item2;
				ReferralInfo item3 = valueTuple.Item3;
				AssignedPlan plan = base.get_App().get_Plan();
				if (plan != null)
				{
					identifier = plan.get_Owner().get_Identifier();
				}
				else
				{
					identifier = null;
				}
				item1.planOwner = identifier;
				PlansDto plansDto = PlansDto.FromDomain(availablePlans.ToArray<Plan>(), item1.planOwner, item1.plan.Item2, item3, item2, this.u003cGetPlansu003eg__GetLockedu007c4_1(ref item1));
				availablePlans = null;
				return plansDto;
			});
			base.get_Response().get_Headers().set_Item(HeaderNames.ETag, ETagExtensions.ToEtag<IAppEntity>(base.get_App()));
			return this.Ok(deferred);
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.plans.change" })]
		[HttpPut]
		[ProducesResponseType(typeof(PlanChangedDto), 200)]
		[Route("apps/{app}/plan/")]
		public async Task<IActionResult> PutPlan(string app, [FromBody] ChangePlanDto request)
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