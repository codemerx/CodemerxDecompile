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
			JObject jObject;
			if (!File.Exists(this._tenants))
			{
				jObject = new JObject();
			}
			else
			{
				using (StreamReader streamReader = File.OpenText(this._tenants))
				{
					using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
					{
						jObject = await JObject.LoadAsync(jsonTextReader, new CancellationToken());
					}
					jsonTextReader = null;
				}
				streamReader = null;
			}
			JObject value = jObject.GetValue(tenant) as JObject;
			if (value == null)
			{
				value = new JObject();
			}
			JObject jObject1 = value;
			foreach (string key in data.Keys)
			{
				if (data[key] == null)
				{
					jObject1.Remove(key);
				}
				else
				{
					jObject1.set_Item(key, data[key]);
				}
			}
			jObject.set_Item(tenant, jObject1);
			using (StreamWriter streamWriter = File.CreateText(this._tenants))
			{
				JsonTextWriter jsonTextWriter = new JsonTextWriter(streamWriter);
				jsonTextWriter.set_Formatting(1);
				using (JsonTextWriter jsonTextWriter1 = jsonTextWriter)
				{
					await jObject.WriteToAsync(jsonTextWriter1, Array.Empty<JsonConverter>());
				}
				jsonTextWriter1 = null;
			}
			streamWriter = null;
		}
	}
}