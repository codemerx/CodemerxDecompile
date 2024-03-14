using Microsoft.AspNetCore.Http;
using OrchardCore.Environment.Shell;
using System;
using System.Runtime.CompilerServices;

namespace OrchardCore.Modules
{
	public static class RunningShellTableExtensions
	{
		public static ShellSettings Match(this IRunningShellTable table, HttpContext httpContext)
		{
			if (httpContext == null)
			{
				throw new ArgumentNullException("httpContext");
			}
			HttpRequest request = httpContext.get_Request();
			return table.Match(request.get_Host(), request.get_Path(), true);
		}
	}
}