using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OrchardCore.Environment.Shell
{
	public class SingleShellSettingsManager : IShellSettingsManager
	{
		public SingleShellSettingsManager()
		{
			base();
			return;
		}

		public ShellSettings CreateDefaultSettings()
		{
			stackVariable0 = new ShellSettings();
			stackVariable0.set_Name("Default");
			stackVariable0.set_State(2);
			return stackVariable0;
		}

		public Task<IEnumerable<ShellSettings>> LoadSettingsAsync()
		{
			stackVariable1 = new ShellSettings[1];
			stackVariable1[0] = this.CreateDefaultSettings();
			return Task.FromResult<IEnumerable<ShellSettings>>(stackVariable1.AsEnumerable<ShellSettings>());
		}

		public Task<ShellSettings> LoadSettingsAsync(string tenant)
		{
			return Task.FromResult<ShellSettings>(this.CreateDefaultSettings());
		}

		public Task SaveSettingsAsync(ShellSettings shellSettings)
		{
			return Task.get_CompletedTask();
		}
	}
}