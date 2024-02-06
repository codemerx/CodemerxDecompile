using System;
using System.Runtime.CompilerServices;

namespace Squidex.Pipeline.Robots
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class RobotsTxtOptions
	{
		public string Text
		{
			get;
			set;
		}

		public RobotsTxtOptions()
		{
		}
	}
}