using Squidex.Infrastructure.Validation;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.News.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public class FeaturesDto
	{
		[LocalizedRequired]
		public List<FeatureDto> Features { get; } = new List<FeatureDto>();

		public int Version
		{
			get;
			set;
		}

		public FeaturesDto()
		{
		}
	}
}