using Squidex.Domain.Apps.Entities.Billing;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Plans.Models
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class PlanDto
	{
		public string ConfirmText
		{
			get;
			set;
		}

		[LocalizedRequired]
		[Nullable(1)]
		public string Costs
		{
			[NullableContext(1)]
			get;
			[NullableContext(1)]
			set;
		}

		[LocalizedRequired]
		[Nullable(1)]
		public string Id
		{
			[NullableContext(1)]
			get;
			[NullableContext(1)]
			set;
		}

		public long MaxApiBytes
		{
			get;
			set;
		}

		public long MaxApiCalls
		{
			get;
			set;
		}

		public long MaxAssetSize
		{
			get;
			set;
		}

		public int MaxContributors
		{
			get;
			set;
		}

		[LocalizedRequired]
		[Nullable(1)]
		public string Name
		{
			[NullableContext(1)]
			get;
			[NullableContext(1)]
			set;
		}

		public string YearlyConfirmText
		{
			get;
			set;
		}

		public string YearlyCosts
		{
			get;
			set;
		}

		public string YearlyId
		{
			get;
			set;
		}

		public PlanDto()
		{
		}

		[NullableContext(1)]
		public static PlanDto FromDomain(Plan plan)
		{
			return SimpleMapper.Map<Plan, PlanDto>(plan, new PlanDto());
		}
	}
}