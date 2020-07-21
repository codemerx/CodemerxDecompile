using Piranha.Models;
using System;
using System.Runtime.CompilerServices;

namespace Piranha.Web
{
	public class RouteResponse : IRouteResponse
	{
		public HttpCacheInfo CacheInfo
		{
			get;
			set;
		}

		public bool IsPublished
		{
			get;
			set;
		}

		public Guid PageId
		{
			get;
			set;
		}

		public string QueryString
		{
			get;
			set;
		}

		public Piranha.Models.RedirectType RedirectType
		{
			get;
			set;
		}

		public string RedirectUrl
		{
			get;
			set;
		}

		public string Route
		{
			get;
			set;
		}

		public RouteResponse()
		{
			base();
			return;
		}
	}
}