using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;

namespace OrchardCore.Environment.Shell
{
	public class ShellOptionsSetup : IConfigureOptions<ShellOptions>
	{
		private const string OrchardAppData = "ORCHARD_APP_DATA";

		private const string DefaultAppDataPath = "App_Data";

		private const string DefaultSitesPath = "Sites";

		private readonly IHostEnvironment _hostingEnvironment;

		public ShellOptionsSetup(IHostEnvironment hostingEnvironment)
		{
			base();
			this._hostingEnvironment = hostingEnvironment;
			return;
		}

		public void Configure(ShellOptions options)
		{
			V_0 = Environment.GetEnvironmentVariable("ORCHARD_APP_DATA");
			if (string.IsNullOrEmpty(V_0))
			{
				options.set_ShellsApplicationDataPath(Path.Combine(this._hostingEnvironment.get_ContentRootPath(), "App_Data"));
			}
			else
			{
				options.set_ShellsApplicationDataPath(Path.Combine(this._hostingEnvironment.get_ContentRootPath(), V_0));
			}
			options.set_ShellsContainerName("Sites");
			return;
		}
	}
}