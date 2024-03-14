using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class DocsVM
	{
		public string Specification
		{
			get;
			set;
		}

		public DocsVM()
		{
		}
	}
}