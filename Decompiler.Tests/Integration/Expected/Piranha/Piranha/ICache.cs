using System;

namespace Piranha
{
	public interface ICache
	{
		T Get<T>(string key);

		void Remove(string key);

		void Set<T>(string key, T value);
	}
}