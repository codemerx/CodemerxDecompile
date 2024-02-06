using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Squidex.Domain.Apps.Entities.History;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Config.Domain
{
	public static class HistoryServices
	{
		[NullableContext(1)]
		public static void AddSquidexHistory(IServiceCollection services, IConfiguration config)
		{
			OptionsConfigurationServiceCollectionExtensions.Configure<NotifoOptions>(services, config.GetSection("notifo"));
			DependencyInjectionExtensions.AddSingletonAs<NotifoService>(services).AsSelf().As<IUserEvents>();
			DependencyInjectionExtensions.AddSingletonAs<HistoryService>(services).As<IEventConsumer>().As<IHistoryService>();
		}
	}
}