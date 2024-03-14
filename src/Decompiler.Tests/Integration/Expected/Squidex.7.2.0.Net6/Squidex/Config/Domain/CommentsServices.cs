using Microsoft.Extensions.DependencyInjection;
using Squidex.Domain.Apps.Entities.Comments;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Config.Domain
{
	public static class CommentsServices
	{
		[NullableContext(1)]
		public static void AddSquidexComments(IServiceCollection services)
		{
			DependencyInjectionExtensions.AddSingletonAs<CommentsLoader>(services).As<ICommentsLoader>();
			DependencyInjectionExtensions.AddSingletonAs<WatchingService>(services).As<IWatchingService>();
		}
	}
}