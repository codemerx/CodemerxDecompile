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
				return this.cacheStore.Count;
			}
		}

		public DecompilationCache(int maxSize)
		{
			this.maxSize = maxSize;
			this.cacheStore = new Dictionary<Key, Value>();
			this.keysQueue = new Queue<Key>();
		}

		public void Add(Key key, Value value)
		{
			while (this.Count > this.maxSize)
			{
				this.Delete(this.keysQueue.Dequeue());
			}
			this.cacheStore.Add(key, value);
			this.keysQueue.Enqueue(key);
		}

		public void Clear()
		{
			this.cacheStore.Clear();
		}

		public bool ContainsKey(Key key)
		{
			return this.cacheStore.ContainsKey(key);
		}

		private void Delete(Key key)
		{
			this.cacheStore.Remove(key);
		}

		public Value Get(Key key)
		{
			Value value;
			if (!this.cacheStore.TryGetValue(key, out value))
			{
				throw new KeyNotFoundException("Key not found in cache.");
			}
			return value;
		}

		public void Remove(Key key)
		{
			this.Delete(key);
			Queue<Key> keys = new Queue<Key>();
			foreach (Key key1 in this.keysQueue)
			{
				if (key1.Equals(key))
				{
					continue;
				}
				keys.Enqueue(key1);
			}
			this.keysQueue = keys;
		}
	}
}