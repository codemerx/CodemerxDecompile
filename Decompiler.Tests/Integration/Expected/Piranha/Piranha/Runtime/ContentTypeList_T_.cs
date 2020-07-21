using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Piranha.Runtime
{
	public sealed class ContentTypeList<T> : List<T>
	where T : ContentTypeBase
	{
		public ContentTypeList()
		{
			base();
			return;
		}

		public T GetById(string id)
		{
			V_0 = new ContentTypeList<T>.u003cu003ec__DisplayClass1_0();
			V_0.id = id;
			return this.FirstOrDefault<T>(new Func<T, bool>(V_0.u003cGetByIdu003eb__0));
		}

		public void Init(IEnumerable<T> types)
		{
			V_0 = types.GetEnumerator();
			try
			{
				while (V_0.MoveNext())
				{
					V_1 = V_0.get_Current();
					this.Add(V_1);
				}
			}
			finally
			{
				if (V_0 != null)
				{
					V_0.Dispose();
				}
			}
			App.get_Hooks().RegisterOnAfterSave<T>(new HookManager.ModelDelegate<T>(this.u003cInitu003eb__0_0));
			return;
		}
	}
}