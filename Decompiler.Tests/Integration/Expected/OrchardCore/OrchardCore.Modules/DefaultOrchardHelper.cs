using Microsoft.AspNetCore.Http;
using OrchardCore;
using System;
using System.Runtime.CompilerServices;

namespace OrchardCore.Modules
{
	public class DefaultOrchardHelper : IOrchardHelper
	{
		public Microsoft.AspNetCore.Http.HttpContext HttpContext
		{
			get
			{
				return get_HttpContext();
			}
			set
			{
				set_HttpContext(value);
			}
		}

		// <HttpContext>k__BackingField
		private Microsoft.AspNetCore.Http.HttpContext u003cHttpContextu003ek__BackingField;

		public Microsoft.AspNetCore.Http.HttpContext get_HttpContext()
		{
			return this.u003cHttpContextu003ek__BackingField;
		}

		public void set_HttpContext(Microsoft.AspNetCore.Http.HttpContext value)
		{
			this.u003cHttpContextu003ek__BackingField = value;
			return;
		}

		public DefaultOrchardHelper(IHttpContextAccessor httpContextAccessor)
		{
			base();
			this.set_HttpContext(httpContextAccessor.get_HttpContext());
			return;
		}
	}
}