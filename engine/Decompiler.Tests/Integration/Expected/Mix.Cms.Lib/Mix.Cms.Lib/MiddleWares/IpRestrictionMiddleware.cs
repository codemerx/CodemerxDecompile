using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Mix.Cms.Lib.Services;
using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.MiddleWares
{
	public class IpRestrictionMiddleware
	{
		public readonly RequestDelegate Next;

		public readonly Mix.Cms.Lib.MiddleWares.IpSecuritySettings IpSecuritySettings;

		public IpRestrictionMiddleware(RequestDelegate next, IOptions<Mix.Cms.Lib.MiddleWares.IpSecuritySettings> ipSecuritySettings)
		{
			this.Next = next;
			this.IpSecuritySettings = ipSecuritySettings.get_Value();
		}

		public async Task Invoke(HttpContext context)
		{
			string str;
			if (MixService.GetIpConfig<bool>("IsRetrictIp"))
			{
				IPAddress remoteIpAddress = context.get_Connection().get_RemoteIpAddress();
				if (remoteIpAddress != null)
				{
					remoteIpAddress.ToString();
				}
				else
				{
				}
				string ipConfig = MixService.GetIpConfig<string>("AllowedIps");
				string ipConfig1 = MixService.GetIpConfig<string>("ExceptIps");
				ConnectionInfo connection = context.get_Connection();
				if (connection != null)
				{
					IPAddress pAddress = connection.get_RemoteIpAddress();
					if (pAddress != null)
					{
						str = pAddress.ToString();
					}
					else
					{
						str = null;
					}
				}
				else
				{
					str = null;
				}
				string str1 = str;
				if (ipConfig != "*" && !ipConfig.Contains(str1) || ipConfig1.Contains(str1))
				{
					context.get_Response().set_StatusCode(0x193);
					return;
				}
			}
			await this.Next.Invoke(context);
		}
	}
}