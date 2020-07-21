using System;

namespace Piranha.Models
{
	[Serializable]
	public class Page<T> : GenericPage<T>
	where T : Page<T>
	{
		public Page()
		{
			base();
			return;
		}
	}
}