using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Frontend.Middlewares
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class OptionsFeature
	{
		public Dictionary<string, object> Options { get; } = new Dictionary<string, object>();

		public OptionsFeature()
		{
		}
	}
}