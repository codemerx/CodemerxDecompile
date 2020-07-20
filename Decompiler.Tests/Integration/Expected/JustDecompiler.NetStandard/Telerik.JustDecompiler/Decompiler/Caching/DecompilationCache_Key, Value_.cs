using System;
using System.Collections.Generic;

namespace Telerik.JustDecompiler.Decompiler.Caching
{
	public class DecompilationCache<Key, Value> : IDecompilationCache<Key, Value>
	{
		protected Dictionary<Key, Value> cacheStore;

		protected Queue<Key> keysQueue;

		protected readonly int maxSize;

		public int Count
		{
			get
			{
				return this.cacheStore.get_Count();
			}
		}

		public DecompilationCache(int maxSize)
		{
			base();
			this.maxSize = maxSize;
			this.cacheStore = new Dictionary<Key, Value>();
			this.keysQueue = new Queue<Key>();
			return;
		}

		public void Add(Key key, Value value)
		{
			while (this.get_Count() > this.maxSize)
			{
				this.Delete(this.keysQueue.Dequeue());
			}
			this.cacheStore.Add(key, value);
			this.keysQueue.Enqueue(key);
			return;
		}

		public void Clear()
		{
			this.cacheStore.Clear();
			return;
		}

		public bool ContainsKey(Key key)
		{
			return this.cacheStore.ContainsKey(key);
		}

		private void Delete(Key key)
		{
			dummyVar0 = this.cacheStore.Remove(key);
			return;
		}

		public Value Get(Key key)
		{
			if (!this.cacheStore.TryGetValue(key, out V_0))
			{
				throw new KeyNotFoundException("Key not found in cache.");
			}
			return V_0;
		}

		public void Remove(Key key)
		{
			this.Delete(key);
			V_0 = new Queue<Key>();
			V_1 = this.keysQueue.GetEnumerator();
			try
			{
				while (V_1.MoveNext())
				{
					V_2 = V_1.get_Current();
					if (V_2.Equals(key))
					{
						continue;
					}
					V_0.Enqueue(V_2);
				}
			}
			finally
			{
				((IDisposable)V_1).Dispose();
			}
			this.keysQueue = V_0;
			return;
		}
	}
}