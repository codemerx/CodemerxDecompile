using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Squidex.Domain.Users;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Config.Authentication
{
	public static class IdentityServices
	{
		[NullableContext(1)]
		public static void AddSquidexIdentity(IServiceCollection services, IConfiguration config)
		{
			ConfigurationServiceExtensions.Configure<MyIdentityOptions>(services, config, "identity");
			DependencyInjectionExtensions.AddSingletonAs<DefaultUserResolver>(services).AsOptional<IUserResolver>();
			DependencyInjectionExtensions.AddSingletonAs<DefaultUserPictureStore>(services).AsOptional<IUserPictureStore>();
		}
	}
}