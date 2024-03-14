using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Squidex.Areas.Api.Controllers.UI;
using Squidex.Domain.Apps.Core.Apps;
using Squidex.Domain.Apps.Entities;
using Squidex.Domain.Apps.Entities.Apps;
using Squidex.Infrastructure.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Squidex.Config.Domain
{
	public static class AppsServices
	{
		[NullableContext(1)]
		public static void AddSquidexApps(IServiceCollection services, IConfiguration config)
		{
			if (ConfigurationBinder.GetValue<bool>(config, "apps:deletePermanent"))
			{
				DependencyInjectionExtensions.AddSingletonAs<AppPermanentDeleter>(services).As<IEventConsumer>();
			}
			DependencyInjectionExtensions.AddSingletonAs<RolePermissionsProvider>(services).AsSelf();
			DependencyInjectionExtensions.AddSingletonAs<AppEventDeleter>(services).As<IDeleter>();
			DependencyInjectionExtensions.AddSingletonAs<AppUsageDeleter>(services).As<IDeleter>();
			DependencyInjectionExtensions.AddSingletonAs<DefaultAppLogStore>(services).As<IAppLogStore>().As<IDeleter>();
			DependencyInjectionExtensions.AddSingletonAs<AppHistoryEventsCreator>(services).As<IHistoryEventsCreator>();
			DependencyInjectionExtensions.AddSingletonAs<DefaultAppImageStore>(services).As<IAppImageStore>();
			DependencyInjectionExtensions.AddSingletonAs<AppProvider>(services).As<IAppProvider>();
			DependencyInjectionExtensions.AddSingletonAs<AppUISettings>(services).As<IAppUISettings>().As<IDeleter>();
			DependencyInjectionExtensions.AddSingletonAs<AppSettingsSearchSource>(services).As<ISearchSource>();
			ServiceCollectionServiceExtensions.AddSingleton<InitialSettings>(services, (IServiceProvider c) => {
				string str;
				string str1;
				MyUIOptions value = ServiceProviderServiceExtensions.GetRequiredService<IOptions<MyUIOptions>>(c).get_Value();
				List<Pattern> patterns = new List<Pattern>();
				if (value.RegexSuggestions != null)
				{
					foreach (KeyValuePair<string, string> regexSuggestion in value.RegexSuggestions)
					{
						regexSuggestion.Deconstruct(out str, out str1);
						string str2 = str;
						string str3 = str1;
						if (string.IsNullOrWhiteSpace(str2) || string.IsNullOrWhiteSpace(str3))
						{
							continue;
						}
						patterns.Add(new Pattern(str2, str3));
					}
				}
				InitialSettings initialSetting = new InitialSettings();
				AppSettings appSetting = new AppSettings();
				appSetting.set_Patterns(ReadonlyList.ToReadonlyList<Pattern>(patterns));
				initialSetting.set_Settings(appSetting);
				return initialSetting;
			});
		}
	}
}