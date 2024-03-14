using Piranha.Extend;
using System;
using System.Runtime.CompilerServices;

namespace Piranha.Runtime
{
	public sealed class AppModule : AppDataItem
	{
		public IModule Instance
		{
			get;
			set;
		}

		public AppModule()
		{
		}
	}
}