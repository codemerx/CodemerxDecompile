using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Squidex.Domain.Apps.Entities.History;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.Frontend.Middlewares
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class NotifoMiddleware
	{
		private readonly RequestDelegate next;

		[Nullable(2)]
		private readonly string workerUrl;

		public NotifoMiddleware(RequestDelegate next, IOptions<NotifoOptions> options)
		{
			this.next = next;
			this.workerUrl = NotifoMiddleware.GetUrl(options.get_Value());
		}

		[return: Nullable(2)]
		private static string GetUrl(NotifoOptions options)
		{
			if (!options.IsConfigured())
			{
				return null;
			}
			if (options.get_ApiUrl().Contains("localhost:5002", StringComparison.Ordinal))
			{
				return "https://localhost:3002/notifo-sdk-worker.js";
			}
			return string.Concat(options.get_ApiUrl(), "/build/notifo-sdk-worker.js");
		}

		public async Task InvokeAsync(HttpContext context)
		{
			if (!context.get_Request().get_Path().Equals("/notifo-sw.js", StringComparison.Ordinal) || this.workerUrl == null)
			{
				await this.next.Invoke(context);
			}
			else
			{
				context.get_Response().get_Headers().set_Item(HeaderNames.ContentType, "text/javascript");
				string str = string.Concat("importScripts('", this.workerUrl, "')");
				await HttpResponseWritingExtensions.WriteAsync(context.get_Response(), str, context.get_RequestAborted());
			}
		}
	}
}