using Microsoft.AspNetCore.Http;
using OrchardCore.Environment.Shell;
using OrchardCore.Environment.Shell.Builders;
using System;
using System.Runtime.CompilerServices;

namespace OrchardCore.Modules
{
	internal static class ShellExtensions
	{
		public static HttpContext CreateHttpContext(this ShellContext shell)
		{
			stackVariable2 = shell.get_Settings().CreateHttpContext();
			stackVariable3 = stackVariable2.get_Features();
			stackVariable4 = new ShellContextFeature();
			stackVariable4.set_ShellContext(shell);
			stackVariable4.set_OriginalPathBase(PathString.op_Implicit(string.Empty));
			stackVariable4.set_OriginalPath(PathString.op_Implicit("/"));
			stackVariable3.Set<ShellContextFeature>(stackVariable4);
			return stackVariable2;
		}

		public static HttpContext CreateHttpContext(this ShellSettings settings)
		{
			V_0 = HttpContextExtensions.UseShellScopeServices(new DefaultHttpContext());
			stackVariable3 = settings.get_RequestUrlHost();
			if (stackVariable3 != null)
			{
				stackVariable7 = stackVariable3.Split('/', 1).FirstOrDefault<string>();
			}
			else
			{
				dummyVar0 = stackVariable3;
				stackVariable7 = null;
			}
			V_1 = stackVariable7;
			stackVariable9 = V_0.get_Request();
			stackVariable10 = V_1;
			if (stackVariable10 == null)
			{
				dummyVar1 = stackVariable10;
				stackVariable10 = "localhost";
			}
			stackVariable9.set_Host(new HostString(stackVariable10));
			if (!string.IsNullOrWhiteSpace(settings.get_RequestUrlPrefix()))
			{
				V_0.get_Request().set_PathBase(PathString.op_Implicit(string.Concat("/", settings.get_RequestUrlPrefix())));
			}
			V_0.get_Request().set_Path(PathString.op_Implicit("/"));
			V_0.get_Items().set_Item("IsBackground", true);
			return V_0;
		}
	}
}