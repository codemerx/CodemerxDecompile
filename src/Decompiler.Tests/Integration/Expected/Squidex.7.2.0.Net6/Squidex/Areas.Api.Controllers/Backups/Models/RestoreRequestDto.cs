using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Backups.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class RestoreRequestDto
	{
		[LocalizedRegularExpression("^[a-z0-9]+(\\-[a-z0-9]+)*$")]
		[Nullable(2)]
		public string Name
		{
			[NullableContext(2)]
			get;
			[NullableContext(2)]
			set;
		}

		[LocalizedRequired]
		public Uri Url
		{
			get;
			set;
		}

		public RestoreRequestDto()
		{
		}
	}
}