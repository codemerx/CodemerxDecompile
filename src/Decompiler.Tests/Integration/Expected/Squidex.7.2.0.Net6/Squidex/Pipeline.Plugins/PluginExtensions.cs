using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Squidex.Domain.Apps.Core;
using Squidex.Domain.Apps.Entities;
using Squidex.Domain.Apps.Events;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Plugins;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Squidex.Pipeline.Plugins
{
	[Nullable(0)]
	[NullableContext(1)]
	public static class PluginExtensions
	{
		private readonly static AssemblyName[] SharedAssemblies;

		static PluginExtensions()
		{
			PluginExtensions.SharedAssemblies = (
				from x in (IEnumerable<Type>)(new Type[] { typeof(IPlugin), typeof(SquidexCoreModel), typeof(SquidexCoreOperations), typeof(SquidexEntities), typeof(SquidexEvents), typeof(SquidexInfrastructure), typeof(SquidexWeb) })
				select x.Assembly.GetName()).ToArray<AssemblyName>();
		}

		public static IMvcBuilder AddSquidexPlugins(this IMvcBuilder mvcBuilder, IConfiguration config)
		{
			PluginManager pluginManager = new PluginManager();
			PluginOptions pluginOption = ConfigurationBinder.Get<PluginOptions>(config);
			if (pluginOption.get_Plugins() != null)
			{
				string[] plugins = pluginOption.get_Plugins();
				for (int i = 0; i < (int)plugins.Length; i++)
				{
					Assembly assembly = pluginManager.Load(plugins[i], PluginExtensions.SharedAssemblies);
					if (assembly != null)
					{
						assembly.AddParts(mvcBuilder);
					}
				}
			}
			pluginManager.ConfigureServices(mvcBuilder.get_Services(), config);
			ServiceCollectionServiceExtensions.AddSingleton<PluginManager>(mvcBuilder.get_Services(), pluginManager);
			return mvcBuilder;
		}

		public static void UsePlugins(this IApplicationBuilder app)
		{
			PluginManager requiredService = ServiceProviderServiceExtensions.GetRequiredService<PluginManager>(app.get_ApplicationServices());
			requiredService.Log(ServiceProviderServiceExtensions.GetRequiredService<ISemanticLog>(app.get_ApplicationServices()));
		}
	}
}