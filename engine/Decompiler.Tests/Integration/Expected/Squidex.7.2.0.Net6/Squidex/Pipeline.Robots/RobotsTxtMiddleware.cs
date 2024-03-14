using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Pipeline.Robots
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class RobotsTxtMiddleware
	{
		private readonly RequestDelegate next;

		public RobotsTxtMiddleware(RequestDelegate next)
		{
			this.next = next;
		}

		private static bool CanServeRequest(HttpRequest request)
		{
			if (!HttpMethods.IsGet(request.get_Method()))
			{
				return false;
			}
			return string.IsNullOrEmpty(request.get_Path());
		}

		public async Task InvokeAsync(HttpContext context, IOptions<RobotsTxtOptions> robotsTxtOptions)
		{
			string text = robotsTxtOptions.get_Value().Text;
			if (!RobotsTxtMiddleware.CanServeRequest(context.get_Request()) || string.IsNullOrWhiteSpace(text))
			{
				await this.next.Invoke(context);
			}
			else
			{
				context.get_Response().set_ContentType("text/plain");
				context.get_Response().set_StatusCode(200);
				await HttpResponseWritingExtensions.WriteAsync(context.get_Response(), text, context.get_RequestAborted());
			}
		}
	}
}