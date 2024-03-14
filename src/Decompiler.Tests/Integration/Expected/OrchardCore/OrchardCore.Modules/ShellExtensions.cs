using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace OrchardCore.Modules
{
	internal static class ShellExtensions
	{
		public static HttpContext CreateHttpContext(this ShellContext shell)
		{
			HttpContext httpContext = shell.get_Settings().CreateHttpContext();
			IFeatureCollection features = httpContext.get_Features();
			ShellContextFeature shellContextFeature = new ShellContextFeature();
			shellContextFeature.set_ShellContext(shell);
			shellContextFeature.set_OriginalPathBase(string.Empty);
			shellContextFeature.set_OriginalPath("/");
			features.Set<ShellContextFeature>(shellContextFeature);
			return httpContext;
		}

		public static HttpContext CreateHttpContext(this ShellSettings settings)
		{
			string str;
			HttpContext httpContext = HttpContextExtensions.UseShellScopeServices(new DefaultHttpContext());
			string requestUrlHost = settings.get_RequestUrlHost();
			if (requestUrlHost != null)
			{
				str = requestUrlHost.Split('/', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault<string>();
			}
			else
			{
				str = null;
			}
			string str1 = str;
			httpContext.get_Request().set_Host(new HostString(str1 ?? "localhost"));
			if (!string.IsNullOrWhiteSpace(settings.get_RequestUrlPrefix()))
			{
				httpContext.get_Request().set_PathBase(string.Concat("/", settings.get_RequestUrlPrefix()));
			}
			httpContext.get_Request().set_Path("/");
			httpContext.get_Items()["IsBackground"] = true;
			return httpContext;
		}
	}
}