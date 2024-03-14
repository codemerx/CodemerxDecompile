using System;

namespace Piranha.Models
{
	public interface IRegionList
	{
		IDynamicContent Model
		{
			get;
			set;
		}

		string RegionId
		{
			get;
			set;
		}

		string TypeId
		{
			get;
			set;
		}

		void Add(object item);

		void Clear();
	}
}