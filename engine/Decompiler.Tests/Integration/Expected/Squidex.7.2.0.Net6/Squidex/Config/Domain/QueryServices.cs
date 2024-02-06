using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Squidex.Domain.Apps.Core.ExtractReferenceIds;
using Squidex.Domain.Apps.Entities.Contents.GraphQL.Types.Primitives;
using Squidex.Web.Services;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Config.Domain
{
	public static class QueryServices
	{
		[NullableContext(1)]
		public static void AddSquidexQueries(IServiceCollection services, IConfiguration config)
		{
			ConfigurationServiceExtensions.Configure<GraphQLOptions>(services, config, "graphql");
			DependencyInjectionExtensions.AddSingletonAs<StringReferenceExtractor>(services).AsSelf();
			DependencyInjectionExtensions.AddSingletonAs<UrlGenerator>(services).As<IUrlGenerator>();
			DependencyInjectionExtensions.AddSingletonAs<InstantGraphType>(services).AsSelf();
			DependencyInjectionExtensions.AddSingletonAs<JsonGraphType>(services).AsSelf();
			DependencyInjectionExtensions.AddSingletonAs<JsonNoopGraphType>(services).AsSelf();
		}
	}
}