using System;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Common
{
	public class PairList<K, V> : List<KeyValuePair<K, V>>
	{
		public PairList()
		{
		}

		public PairList(IEnumerable<KeyValuePair<K, V>> collection) : base(collection)
		{
		}

		public void Add(K key, V value)
		{
			base.Add(new KeyValuePair<K, V>(key, value));
		}
	}
}