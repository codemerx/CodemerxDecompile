using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.Frontend.Middlewares
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class EmbedMiddleware
	{
		private readonly RequestDelegate next;

		public EmbedMiddleware(RequestDelegate next)
		{
			this.next = next;
		}

		public Task InvokeAsync(HttpContext context)
		{
			PathString pathString = new PathString();
			HttpRequest request = context.get_Request();
			if (request.get_Path().StartsWithSegments("/embed", StringComparison.Ordinal, ref pathString))
			{
				request.set_Path(pathString);
				OptionsFeature optionsFeature = new OptionsFeature();
				optionsFeature.Options["embedded"] = true;
				optionsFeature.Options["embedPath"] = "/embed";
				context.get_Features().Set<OptionsFeature>(optionsFeature);
			}
			return this.next.Invoke(context);
		}
	}
}