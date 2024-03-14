using Microsoft.Extensions.DependencyInjection;
using Squidex.Domain.Apps.Entities.Search;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Config.Domain
{
	public static class SearchServices
	{
		[NullableContext(1)]
		public static void AddSquidexSearch(IServiceCollection services)
		{
			DependencyInjectionExtensions.AddSingletonAs<SearchManager>(services).As<ISearchManager>();
		}
	}
}