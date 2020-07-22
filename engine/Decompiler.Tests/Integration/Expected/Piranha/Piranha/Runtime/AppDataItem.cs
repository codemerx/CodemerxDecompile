using System;
using System.Runtime.CompilerServices;

namespace Piranha.Runtime
{
	public class AppDataItem
	{
		public System.Type Type
		{
			get;
			set;
		}

		public string TypeName
		{
			get;
			set;
		}

		public AppDataItem()
		{
			base();
			return;
		}
	}
}