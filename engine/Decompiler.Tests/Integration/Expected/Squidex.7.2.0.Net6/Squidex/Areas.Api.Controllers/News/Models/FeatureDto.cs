using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.News.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class FeatureDto
	{
		[LocalizedRequired]
		public string Name
		{
			get;
			set;
		}

		[LocalizedRequired]
		public string Text
		{
			get;
			set;
		}

		public FeatureDto()
		{
		}
	}
}