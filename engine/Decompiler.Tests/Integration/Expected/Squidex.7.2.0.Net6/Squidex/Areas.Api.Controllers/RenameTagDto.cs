using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class RenameTagDto
	{
		[LocalizedRequired]
		public string TagName
		{
			get;
			set;
		}

		public RenameTagDto()
		{
		}
	}
}