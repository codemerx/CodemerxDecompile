using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Piranha.Runtime
{
	public abstract class AppDataList<T, TItem> : IEnumerable<TItem>, IEnumerable
	where TItem : AppDataItem
	{
		protected readonly List<TItem> _items;

		protected AppDataList()
		{
			this._items = new List<TItem>();
			base();
			return;
		}

		public virtual TItem GetByType(Type type)
		{
			V_0 = new AppDataList<T, TItem>.u003cu003ec__DisplayClass3_0();
			V_0.type = type;
			return this._items.SingleOrDefault<TItem>(new Func<TItem, bool>(V_0.u003cGetByTypeu003eb__0));
		}

		public virtual TItem GetByType(string typeName)
		{
			V_0 = new AppDataList<T, TItem>.u003cu003ec__DisplayClass4_0();
			V_0.typeName = typeName;
			return this._items.SingleOrDefault<TItem>(new Func<TItem, bool>(V_0.u003cGetByTypeu003eb__0));
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
			V_0 = new AppDataList<T, TItem>.u003cu003ec__DisplayClass1_0<TValue>();
			V_0.type = Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Piranha.Runtime.AppDataList`2::Register()
			// Exception in: System.Void Register()
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com


		IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this._items.GetEnumerator();
		}

		public virtual void UnRegister<TValue>()
		where TValue : T
		{
			stackVariable1 = this._items;
			stackVariable2 = AppDataList<T, TItem>.u003cu003ec__2<TValue>.u003cu003e9__2_0;
			if (stackVariable2 == null)
			{
				dummyVar0 = stackVariable2;
				stackVariable2 = new Func<TItem, bool>(AppDataList<T, TItem>.u003cu003ec__2<TValue>.u003cu003e9.u003cUnRegisteru003eb__2_0);
				AppDataList<T, TItem>.u003cu003ec__2<TValue>.u003cu003e9__2_0 = stackVariable2;
			}
			V_0 = stackVariable1.SingleOrDefault<TItem>(stackVariable2);
			if (V_0 != null)
			{
				dummyVar1 = this._items.Remove(V_0);
			}
			return;
		}
	}
}