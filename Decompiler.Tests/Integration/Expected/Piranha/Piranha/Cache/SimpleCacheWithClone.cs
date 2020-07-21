using System;

namespace Piranha.Cache
{
	public class SimpleCacheWithClone : SimpleCache
	{
		public SimpleCacheWithClone()
		{
			base(true);
			return;
		}
	}
}