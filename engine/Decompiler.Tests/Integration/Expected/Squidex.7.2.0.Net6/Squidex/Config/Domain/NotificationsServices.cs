using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Squidex.Domain.Apps.Entities.Invitation;
using Squidex.Domain.Apps.Entities.Notifications;
using Squidex.Infrastructure.Email;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Config.Domain
{
	public static class NotificationsServices
	{
		[NullableContext(1)]
		public static void AddSquidexNotifications(IServiceCollection services, IConfiguration config)
		{
			SmtpOptions smtpOption = ConfigurationBinder.Get<SmtpOptions>(config.GetSection("email:smtp")) ?? new SmtpOptions();
			if (!smtpOption.IsConfigured())
			{
				DependencyInjectionExtensions.AddSingletonAs<NoopUserNotifications>(services).AsOptional<IUserNotifications>();
			}
			else
			{
				ServiceCollectionServiceExtensions.AddSingleton<IOptions<SmtpOptions>>(services, Options.Create<SmtpOptions>(smtpOption));
				ConfigurationServiceExtensions.Configure<EmailUserNotificationOptions>(services, config, "email:notifications");
				DependencyInjectionExtensions.AddSingletonAs<SmtpEmailSender>(services).As<IEmailSender>();
				DependencyInjectionExtensions.AddSingletonAs<EmailUserNotifications>(services).AsOptional<IUserNotifications>();
			}
			DependencyInjectionExtensions.AddSingletonAs<InvitationEventConsumer>(services).As<IEventConsumer>();
		}
	}
}