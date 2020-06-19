using System;
using System.Collections.Generic;
using System.Linq;

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

		public bool ContainsKey(Key key)
		{
			return cacheStore.ContainsKey(key);
		}

		public void Add(Key key, Value value)
		{
			while (this.Count > this.maxSize)
			{
				Key oldestInCache = keysQueue.Dequeue();
				Delete(oldestInCache);
			}

			cacheStore.Add(key, value);
			keysQueue.Enqueue(key);
		}

		private void Delete(Key key)
		{
			cacheStore.Remove(key);
		}

        public void Remove(Key key)
        {
            this.Delete(key);
            Queue<Key> newKeysQueue = new Queue<Key>();
            foreach (Key keyFromQueue in this.keysQueue)
            {
                if (!keyFromQueue.Equals(key))
                {
                    newKeysQueue.Enqueue(keyFromQueue);
                }
            }

            this.keysQueue = newKeysQueue;
        }

		public Value Get(Key key)
		{
			Value value;

			if (!cacheStore.TryGetValue(key, out value))
			{
				throw new KeyNotFoundException("Key not found in cache.");
			}

			return value;
		}

		public void Clear()
		{
			cacheStore.Clear();
		}
	}
}
