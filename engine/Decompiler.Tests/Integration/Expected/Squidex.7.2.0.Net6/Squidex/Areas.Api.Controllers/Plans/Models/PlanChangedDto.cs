using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Plans.Models
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class PlanChangedDto
	{
		public string RedirectUri
		{
			get;
			set;
		}

		public PlanChangedDto()
		{
		}
	}
}