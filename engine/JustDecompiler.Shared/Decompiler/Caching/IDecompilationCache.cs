using System;
using System.Linq;

namespace Telerik.JustDecompiler.Decompiler.Caching
{
	public interface IDecompilationCache<Key, Value>
	{
		bool ContainsKey(Key key);
		void Add(Key key, Value value);
		Value Get(Key key);
        void Remove(Key key);
		void Clear();
	}
}
