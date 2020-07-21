using System;
using System.Collections.Generic;
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
				if (string.IsNullOrEmpty(this.get_AllowedIps()))
				{
					return new List<string>();
				}
				return Enumerable.ToList<string>(this.get_AllowedIps().Split(',', 0));
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
			base();
			return;
		}
	}
}