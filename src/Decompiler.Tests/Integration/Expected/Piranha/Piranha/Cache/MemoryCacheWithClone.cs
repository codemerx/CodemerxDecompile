using Microsoft.Extensions.Caching.Memory;
using System;

namespace Piranha.Cache
{
	public class MemoryCacheWithClone : MemoryCache
	{
		public MemoryCacheWithClone(IMemoryCache cache) : base(cache, true)
		{
		}
	}
}