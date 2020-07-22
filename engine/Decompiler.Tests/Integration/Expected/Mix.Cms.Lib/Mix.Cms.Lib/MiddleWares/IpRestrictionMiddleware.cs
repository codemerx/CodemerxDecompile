using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
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
			base();
			this.Next = next;
			this.IpSecuritySettings = ipSecuritySettings.get_Value();
			return;
		}

		public async Task Invoke(HttpContext context)
		{
			V_0.u003cu003e4__this = this;
			V_0.context = context;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<IpRestrictionMiddleware.u003cInvokeu003ed__3>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}