using System;
using System.Runtime.CompilerServices;

namespace Piranha
{
	public class PiranhaRouteConfig
	{
		public string LoginUrl
		{
			get;
			set;
		}

		public bool UseAliasRouting
		{
			get;
			set;
		}

		public bool UseArchiveRouting
		{
			get;
			set;
		}

		public bool UsePageRouting
		{
			get;
			set;
		}

		public bool UsePostRouting
		{
			get;
			set;
		}

		public bool UseSitemapRouting
		{
			get;
			set;
		}

		public bool UseSiteRouting
		{
			get;
			set;
		}

		public bool UseStartpageRouting
		{
			get;
			set;
		}

		public PiranhaRouteConfig()
		{
			this.u003cLoginUrlu003ek__BackingField = "/login";
			this.u003cUseAliasRoutingu003ek__BackingField = true;
			this.u003cUseArchiveRoutingu003ek__BackingField = true;
			this.u003cUsePageRoutingu003ek__BackingField = true;
			this.u003cUsePostRoutingu003ek__BackingField = true;
			this.u003cUseSiteRoutingu003ek__BackingField = true;
			this.u003cUseSitemapRoutingu003ek__BackingField = true;
			this.u003cUseStartpageRoutingu003ek__BackingField = true;
			base();
			return;
		}
	}
}