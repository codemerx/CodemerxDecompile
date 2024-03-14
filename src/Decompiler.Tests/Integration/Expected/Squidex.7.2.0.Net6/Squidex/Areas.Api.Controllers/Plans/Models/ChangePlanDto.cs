using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Plans.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class ChangePlanDto
	{
		[LocalizedRequired]
		public string PlanId
		{
			get;
			set;
		}

		public ChangePlanDto()
		{
		}
	}
}