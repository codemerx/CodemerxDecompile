using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace OrchardCore.Modules
{
	public class PoweredByMiddleware
	{
		private readonly RequestDelegate _next;

		private readonly IPoweredByMiddlewareOptions _options;

		public PoweredByMiddleware(RequestDelegate next, IPoweredByMiddlewareOptions options)
		{
			base();
			this._next = next;
			this._options = options;
			return;
		}

		public Task Invoke(HttpContext httpContext)
		{
			if (this._options.get_Enabled())
			{
				httpContext.get_Response().get_Headers().set_Item(this._options.get_HeaderName(), StringValues.op_Implicit(this._options.get_HeaderValue()));
			}
			return this._next.Invoke(httpContext);
		}
	}
}