using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OrchardCore.Environment.Shell;
using System;
using System.Threading.Tasks;

namespace OrchardCore.Environment.Shell.Configuration
{
	public class ShellsConfigurationSources : IShellsConfigurationSources
	{
		private readonly string _environment;

		private readonly string _appsettings;

		public ShellsConfigurationSources(IHostEnvironment hostingEnvironment, IOptions<ShellOptions> shellOptions)
		{
			base();
			this._environment = hostingEnvironment.get_EnvironmentName();
			this._appsettings = Path.Combine(shellOptions.get_Value().get_ShellsApplicationDataPath(), "appsettings");
			return;
		}

		public Task AddSourcesAsync(IConfigurationBuilder builder)
		{
			dummyVar0 = JsonConfigurationExtensions.AddJsonFile(JsonConfigurationExtensions.AddJsonFile(builder, string.Concat(this._appsettings, ".json"), true), string.Concat(this._appsettings, ".", this._environment, ".json"), true);
			return Task.get_CompletedTask();
		}
	}
}