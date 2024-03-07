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
			JObject jObject;
			string str = Path.Combine(this._container, tenant);
			string str1 = Path.Combine(str, "appsettings.json");
			if (!File.Exists(str1))
			{
				jObject = new JObject();
			}
			else
			{
				using (StreamReader streamReader = File.OpenText(str1))
				{
					using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
					{
						jObject = await JObject.LoadAsync(jsonTextReader, new CancellationToken());
					}
					jsonTextReader = null;
				}
				streamReader = null;
			}
			foreach (string key in data.Keys)
			{
				if (data[key] == null)
				{
					jObject.Remove(key);
				}
				else
				{
					jObject.set_Item(key, data[key]);
				}
			}
			Directory.CreateDirectory(str);
			using (StreamWriter streamWriter = File.CreateText(str1))
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