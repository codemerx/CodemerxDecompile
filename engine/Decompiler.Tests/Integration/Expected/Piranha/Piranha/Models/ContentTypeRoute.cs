using System;
using System.Runtime.CompilerServices;

namespace Piranha.Models
{
	[Serializable]
	public sealed class ContentTypeRoute
	{
		public string Route
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public ContentTypeRoute()
		{
			base();
			return;
		}

		public static implicit operator String(ContentTypeRoute r)
		{
			return r.get_Route();
		}
	}
}