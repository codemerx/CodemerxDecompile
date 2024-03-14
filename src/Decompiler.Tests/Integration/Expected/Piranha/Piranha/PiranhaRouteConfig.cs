using System;
using System.Runtime.CompilerServices;

namespace Piranha
{
	public class PiranhaRouteConfig
	{
		public string LoginUrl { get; set; } = "/login";

		public bool UseAliasRouting { get; set; } = true;

		public bool UseArchiveRouting { get; set; } = true;

		public bool UsePageRouting { get; set; } = true;

		public bool UsePostRouting { get; set; } = true;

		public bool UseSitemapRouting { get; set; } = true;

		public bool UseSiteRouting { get; set; } = true;

		public bool UseStartpageRouting { get; set; } = true;

		public PiranhaRouteConfig()
		{
		}
	}
}