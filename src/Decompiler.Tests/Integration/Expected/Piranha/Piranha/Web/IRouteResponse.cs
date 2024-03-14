using Piranha.Models;
using System;

namespace Piranha.Web
{
	public interface IRouteResponse
	{
		HttpCacheInfo CacheInfo
		{
			get;
			set;
		}

		bool IsPublished
		{
			get;
			set;
		}

		Guid PageId
		{
			get;
			set;
		}

		string QueryString
		{
			get;
			set;
		}

		Piranha.Models.RedirectType RedirectType
		{
			get;
			set;
		}

		string RedirectUrl
		{
			get;
			set;
		}

		string Route
		{
			get;
			set;
		}
	}
}