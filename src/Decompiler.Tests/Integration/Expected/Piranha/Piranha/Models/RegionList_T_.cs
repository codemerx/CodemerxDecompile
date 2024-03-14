using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	[Serializable]
	public class RegionList<T> : List<T>, IRegionList
	{
		public IDynamicContent Model
		{
			get;
			set;
		}

		public string RegionId
		{
			get;
			set;
		}

		public string TypeId
		{
			get;
			set;
		}

		public RegionList()
		{
		}

		public void Add(object item)
		{
			if (item.GetType() != typeof(T))
			{
				throw new ArgumentException("Item type does not match the list");
			}
			base.Add((T)item);
		}
	}
}