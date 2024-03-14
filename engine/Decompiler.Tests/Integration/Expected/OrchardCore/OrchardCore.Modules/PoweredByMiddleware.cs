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
			this._next = next;
			this._options = options;
		}

		public Task Invoke(HttpContext httpContext)
		{
			if (this._options.Enabled)
			{
				httpContext.get_Response().get_Headers().set_Item(this._options.HeaderName, this._options.HeaderValue);
			}
			return this._next.Invoke(httpContext);
		}
	}
}