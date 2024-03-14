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
				return JustDecompileGenerated_get_HttpContext();
			}
			set
			{
				JustDecompileGenerated_set_HttpContext(value);
			}
		}

		private Microsoft.AspNetCore.Http.HttpContext JustDecompileGenerated_HttpContext_k__BackingField;

		public Microsoft.AspNetCore.Http.HttpContext JustDecompileGenerated_get_HttpContext()
		{
			return this.JustDecompileGenerated_HttpContext_k__BackingField;
		}

		public void JustDecompileGenerated_set_HttpContext(Microsoft.AspNetCore.Http.HttpContext value)
		{
			this.JustDecompileGenerated_HttpContext_k__BackingField = value;
		}

		public DefaultOrchardHelper(IHttpContextAccessor httpContextAccessor)
		{
			this.HttpContext = httpContextAccessor.get_HttpContext();
		}
	}
}