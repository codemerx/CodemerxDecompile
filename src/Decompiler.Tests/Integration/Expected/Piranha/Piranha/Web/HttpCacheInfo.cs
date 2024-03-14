using System;
using System.Runtime.CompilerServices;

namespace Piranha.Web
{
	public class HttpCacheInfo
	{
		public string EntityTag
		{
			get;
			set;
		}

		public DateTime? LastModified
		{
			get;
			set;
		}

		public HttpCacheInfo()
		{
		}
	}
}