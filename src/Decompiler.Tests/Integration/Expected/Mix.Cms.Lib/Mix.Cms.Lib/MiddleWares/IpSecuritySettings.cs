using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Mix.Cms.Lib.MiddleWares
{
	public class IpSecuritySettings
	{
		public string AllowedIps
		{
			get;
			set;
		}

		public List<string> AllowedIPsList
		{
			get
			{
				if (string.IsNullOrEmpty(this.AllowedIps))
				{
					return new List<string>();
				}
				return this.AllowedIps.Split(',', StringSplitOptions.None).ToList<string>();
			}
		}

		public string AllowedPortalIps
		{
			get;
			set;
		}

		public string ExceptIps
		{
			get;
			set;
		}

		public bool IsRetrictIp
		{
			get;
			set;
		}

		public IpSecuritySettings()
		{
		}
	}
}