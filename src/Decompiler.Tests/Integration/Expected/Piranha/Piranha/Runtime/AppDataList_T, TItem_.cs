using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Piranha.Runtime
{
	public abstract class AppDataList<T, TItem> : IEnumerable<TItem>, IEnumerable
	where TItem : AppDataItem
	{
		protected readonly List<TItem> _items;

		protected AppDataList()
		{
		}

		public virtual TItem GetByType(Type type)
		{
			return this._items.SingleOrDefault<TItem>((TItem i) => i.Type == type);
		}

		public virtual TItem GetByType(string typeName)
		{
			return this._items.SingleOrDefault<TItem>((TItem i) => i.TypeName == typeName);
		}

		public IEnumerator<TItem> GetEnumerator()
		{
			return this._items.GetEnumerator();
		}

		protected virtual TItem OnRegister<TValue>(TItem item)
		where TValue : T
		{
			return item;
		}

		public virtual void Register<TValue>()
		where TValue : T
		{
			Type type = typeof(TValue);
			if ((
				from  in this._items
				where i.Type == type
				select ).Count<TItem>() == 0)
			{
				TItem fullName = Activator.CreateInstance<TItem>();
				fullName.Type = type;
				fullName.TypeName = type.FullName;
				this._items.Add(this.OnRegister<TValue>(fullName));
			}
		}

		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this._items.GetEnumerator();
		}

		public virtual void UnRegister<TValue>()
		where TValue : T
		{
			TItem tItem = this._items.SingleOrDefault<TItem>((TItem i) => i.Type == typeof(TValue));
			if (tItem != null)
			{
				this._items.Remove(tItem);
			}
		}
	}
}