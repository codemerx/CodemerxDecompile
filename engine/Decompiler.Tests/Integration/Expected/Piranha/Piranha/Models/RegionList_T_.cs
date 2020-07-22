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
			base();
			return;
		}

		public void Add(object item)
		{
			if (!Type.op_Equality(item.GetType(), Type.GetTypeFromHandle(// 
			// Current member / type: System.Void Piranha.Models.RegionList`1::Add(System.Object)
			// Exception in: System.Void Add(System.Object)
			// Specified method is not supported.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

	}
}