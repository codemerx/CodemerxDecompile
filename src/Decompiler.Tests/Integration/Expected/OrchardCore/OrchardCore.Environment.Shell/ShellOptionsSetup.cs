using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.IO;

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
			this._hostingEnvironment = hostingEnvironment;
		}

		public void Configure(ShellOptions options)
		{
			string environmentVariable = Environment.GetEnvironmentVariable("ORCHARD_APP_DATA");
			if (string.IsNullOrEmpty(environmentVariable))
			{
				options.set_ShellsApplicationDataPath(Path.Combine(this._hostingEnvironment.get_ContentRootPath(), "App_Data"));
			}
			else
			{
				options.set_ShellsApplicationDataPath(Path.Combine(this._hostingEnvironment.get_ContentRootPath(), environmentVariable));
			}
			options.set_ShellsContainerName("Sites");
		}
	}
}