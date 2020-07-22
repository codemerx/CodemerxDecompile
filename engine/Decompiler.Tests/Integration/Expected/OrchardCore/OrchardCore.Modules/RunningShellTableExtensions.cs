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
			V_0 = httpContext.get_Request();
			return table.Match(V_0.get_Host(), V_0.get_Path(), true);
		}
	}
}