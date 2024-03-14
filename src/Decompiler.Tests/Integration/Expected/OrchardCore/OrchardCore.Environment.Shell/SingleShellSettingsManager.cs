using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OrchardCore.Environment.Shell
{
	public class SingleShellSettingsManager : IShellSettingsManager
	{
		public SingleShellSettingsManager()
		{
		}

		public ShellSettings CreateDefaultSettings()
		{
			ShellSettings shellSetting = new ShellSettings();
			shellSetting.set_Name("Default");
			shellSetting.set_State(2);
			return shellSetting;
		}

		public Task<IEnumerable<ShellSettings>> LoadSettingsAsync()
		{
			return Task.FromResult<IEnumerable<ShellSettings>>((new ShellSettings[] { this.CreateDefaultSettings() }).AsEnumerable<ShellSettings>());
		}

		public Task<ShellSettings> LoadSettingsAsync(string tenant)
		{
			return Task.FromResult<ShellSettings>(this.CreateDefaultSettings());
		}

		public Task SaveSettingsAsync(ShellSettings shellSettings)
		{
			return Task.CompletedTask;
		}
	}
}