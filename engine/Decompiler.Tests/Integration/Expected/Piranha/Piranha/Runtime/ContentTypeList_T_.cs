using Piranha;
using Piranha.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Piranha.Runtime
{
	public sealed class ContentTypeList<T> : List<T>
	where T : ContentTypeBase
	{
		public ContentTypeList()
		{
		}

		public T GetById(string id)
		{
			return this.FirstOrDefault<T>((T t) => t.Id == id);
		}

		public void Init(IEnumerable<T> types)
		{
			foreach (T type in types)
			{
				base.Add(type);
			}
			App.Hooks.RegisterOnAfterSave<T>((T model) => {
				T t1 = this.FirstOrDefault<T>((T t) => t.Id == model.Id);
				if (t1 != null)
				{
					base.Remove(t1);
				}
				base.Add(model);
			});
		}
	}
}