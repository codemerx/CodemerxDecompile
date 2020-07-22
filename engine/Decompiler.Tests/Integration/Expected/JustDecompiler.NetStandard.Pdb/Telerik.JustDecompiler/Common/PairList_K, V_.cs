using System;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Common
{
	public class PairList<K, V> : List<KeyValuePair<K, V>>
	{
		public PairList()
		{
			base();
			return;
		}

		public PairList(IEnumerable<KeyValuePair<K, V>> collection)
		{
			base(collection);
			return;
		}

		public void Add(K key, V value)
		{
			this.Add(new KeyValuePair<K, V>(key, value));
			return;
		}
	}
}