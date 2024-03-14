using Microsoft.Extensions.Configuration;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Config.Domain
{
	public static class ConfigurationExtensions
	{
		[NullableContext(1)]
		public static void ConfigureForSquidex(IConfigurationBuilder builder)
		{
			JsonConfigurationExtensions.AddJsonFile(builder, "appsettings.Custom.json", true);
		}
	}
}