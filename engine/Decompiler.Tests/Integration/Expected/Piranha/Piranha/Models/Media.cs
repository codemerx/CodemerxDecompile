using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	[Serializable]
	public sealed class Media : MediaBase
	{
		public IDictionary<string, string> Properties
		{
			get;
			set;
		}

		public IList<MediaVersion> Versions
		{
			get;
			set;
		}

		public Media()
		{
			this.u003cPropertiesu003ek__BackingField = new Dictionary<string, string>();
			this.u003cVersionsu003ek__BackingField = new List<MediaVersion>();
			base();
			return;
		}
	}
}