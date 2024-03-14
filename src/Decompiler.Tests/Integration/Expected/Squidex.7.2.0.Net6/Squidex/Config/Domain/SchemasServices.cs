using Microsoft.Extensions.DependencyInjection;
using Squidex.Domain.Apps.Entities.Schemas;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Config.Domain
{
	public static class SchemasServices
	{
		[NullableContext(1)]
		public static void AddSquidexSchemas(IServiceCollection services)
		{
			DependencyInjectionExtensions.AddTransientAs<SchemasSearchSource>(services).As<ISearchSource>();
			DependencyInjectionExtensions.AddSingletonAs<SchemaHistoryEventsCreator>(services).As<IHistoryEventsCreator>();
		}
	}
}