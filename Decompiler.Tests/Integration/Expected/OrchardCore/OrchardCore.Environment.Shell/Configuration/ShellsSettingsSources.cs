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
			base();
			this._tenants = Path.Combine(shellOptions.get_Value().get_ShellsApplicationDataPath(), "tenants.json");
			return;
		}

		public Task AddSourcesAsync(IConfigurationBuilder builder)
		{
			dummyVar0 = JsonConfigurationExtensions.AddJsonFile(builder, this._tenants, true);
			return Task.get_CompletedTask();
		}

		public async Task SaveAsync(string tenant, IDictionary<string, string> data)
		{
			V_0.u003cu003e4__this = this;
			V_0.tenant = tenant;
			V_0.data = data;
			V_0.u003cu003et__builder = AsyncTaskMethodBuilder.Create();
			V_0.u003cu003e1__state = -1;
			V_0.u003cu003et__builder.Start<ShellsSettingsSources.u003cSaveAsyncu003ed__3>(ref V_0);
			return V_0.u003cu003et__builder.get_Task();
		}
	}
}