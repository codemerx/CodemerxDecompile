using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Squidex.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Squidex.Config.Startup
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class LogConfigurationHost : IHostedService
	{
		private readonly IConfiguration configuration;

		private readonly ISemanticLog log;

		public LogConfigurationHost(IConfiguration configuration, ISemanticLog log)
		{
			this.configuration = configuration;
			this.log = log;
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			SemanticLogExtensions.LogInformation(this.log, new LogFormatter(this, (IObjectWriter w) => w.WriteProperty("message", "Application started").WriteObject("environment", (IObjectWriter c) => {
				string str;
				string str1;
				HashSet<string> strs = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
				foreach (KeyValuePair<string, string> keyValuePair in (
					from kvp in ConfigurationExtensions.AsEnumerable(this.configuration)
					where kvp.Value != null
					select kvp).OrderBy<KeyValuePair<string, string>, string>((KeyValuePair<string, string> x) => x.Key, StringComparer.OrdinalIgnoreCase))
				{
					keyValuePair.Deconstruct(out str, out str1);
					string str2 = str;
					string str3 = str1;
					if (!strs.Add(str2))
					{
						continue;
					}
					c.WriteProperty(str2.ToLowerInvariant(), str3);
				}
			})));
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}