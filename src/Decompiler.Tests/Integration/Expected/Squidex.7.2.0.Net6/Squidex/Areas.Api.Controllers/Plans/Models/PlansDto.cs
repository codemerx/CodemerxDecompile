using Squidex.Domain.Apps.Entities.Billing;
using Squidex.Infrastructure.Validation;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Plans.Models
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class PlansDto
	{
		public string CurrentPlanId
		{
			get;
			set;
		}

		public PlansLockedReason Locked
		{
			get;
			set;
		}

		public string PlanOwner
		{
			get;
			set;
		}

		[LocalizedRequired]
		[Nullable(1)]
		public PlanDto[] Plans
		{
			[NullableContext(1)]
			get;
			[NullableContext(1)]
			set;
		}

		public Uri PortalLink
		{
			get;
			set;
		}

		public ReferralInfo Referral
		{
			get;
			set;
		}

		public PlansDto()
		{
		}

		[return: Nullable(1)]
		public static PlansDto FromDomain([Nullable(1)] Plan[] plans, string owner, string planId, ReferralInfo referral, Uri link, PlansLockedReason locked)
		{
			PlansDto plansDto = new PlansDto()
			{
				Locked = locked,
				CurrentPlanId = planId,
				Plans = plans.Select<Plan, PlanDto>(new Func<Plan, PlanDto>(PlanDto.FromDomain)).ToArray<PlanDto>(),
				PlanOwner = owner,
				Referral = referral
			};
			PlansDto plansDto1 = plansDto;
			if (locked == PlansLockedReason.None)
			{
				plansDto1.PortalLink = link;
			}
			return plansDto1;
		}
	}
}