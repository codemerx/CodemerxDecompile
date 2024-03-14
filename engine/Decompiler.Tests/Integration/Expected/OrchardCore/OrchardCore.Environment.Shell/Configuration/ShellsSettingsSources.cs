using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrchardCore.Environment.Shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace OrchardCore.Environment.Shell.Configuration
{
	public class ShellsSettingsSources : IShellsSettingsSources
	{
		private readonly string _tenants;

		public ShellsSettingsSources(IOptions<ShellOptions> shellOptions)
		{
			this._tenants = Path.Combine(shellOptions.get_Value().get_ShellsApplicationDataPath(), "tenants.json");
		}

		public Task AddSourcesAsync(IConfigurationBuilder builder)
		{
			JsonConfigurationExtensions.AddJsonFile(builder, this._tenants, true);
			return Task.CompletedTask;
		}

		public async Task SaveAsync(string tenant, IDictionary<string, string> data)
		{
			ShellsSettingsSources.u003cSaveAsyncu003ed__3 variable = new ShellsSettingsSources.u003cSaveAsyncu003ed__3();
			variable.u003cu003e4__this = this;
			variable.tenant = tenant;
			variable.data = data;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<ShellsSettingsSources.u003cSaveAsyncu003ed__3>(ref variable);
			return variable.u003cu003et__builder.Task;
		}
	}
}