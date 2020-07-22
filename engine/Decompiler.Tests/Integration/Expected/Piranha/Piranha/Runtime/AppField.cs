using System;
using System.Runtime.CompilerServices;

namespace Piranha.Runtime
{
	public sealed class AppField : AppDataItem
	{
		public string Component
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public string Shorthand
		{
			get;
			set;
		}

		public AppField()
		{
			base();
			return;
		}
	}
}