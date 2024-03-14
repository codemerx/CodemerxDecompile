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
	public class ShellConfigurationSources : IShellConfigurationSources
	{
		private readonly string _container;

		public ShellConfigurationSources(IOptions<ShellOptions> shellOptions)
		{
			this._container = Path.Combine(shellOptions.get_Value().get_ShellsApplicationDataPath(), shellOptions.get_Value().get_ShellsContainerName());
			Directory.CreateDirectory(this._container);
		}

		public Task AddSourcesAsync(string tenant, IConfigurationBuilder builder)
		{
			JsonConfigurationExtensions.AddJsonFile(builder, Path.Combine(this._container, tenant, "appsettings.json"), true);
			return Task.CompletedTask;
		}

		public async Task SaveAsync(string tenant, IDictionary<string, string> data)
		{
			ShellConfigurationSources.u003cSaveAsyncu003ed__3 variable = new ShellConfigurationSources.u003cSaveAsyncu003ed__3();
			variable.u003cu003e4__this = this;
			variable.tenant = tenant;
			variable.data = data;
			variable.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			variable.u003cu003e1__state = -1;
			variable.u003cu003et__builder.Start<ShellConfigurationSources.u003cSaveAsyncu003ed__3>(ref variable);
			return variable.u003cu003et__builder.Task;
		}
	}
}