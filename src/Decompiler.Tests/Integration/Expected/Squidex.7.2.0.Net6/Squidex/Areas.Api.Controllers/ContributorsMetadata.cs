using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class ContributorsMetadata
	{
		public string IsInvited
		{
			get;
			set;
		}

		public ContributorsMetadata()
		{
		}
	}
}