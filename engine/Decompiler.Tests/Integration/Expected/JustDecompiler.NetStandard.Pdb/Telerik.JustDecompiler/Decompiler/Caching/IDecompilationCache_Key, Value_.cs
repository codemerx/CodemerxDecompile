using System;

namespace Telerik.JustDecompiler.Decompiler.Caching
{
	public interface IDecompilationCache<Key, Value>
	{
		void Add(Key key, Value value);

		void Clear();

		bool ContainsKey(Key key);

		Value Get(Key key);

		void Remove(Key key);
	}
}