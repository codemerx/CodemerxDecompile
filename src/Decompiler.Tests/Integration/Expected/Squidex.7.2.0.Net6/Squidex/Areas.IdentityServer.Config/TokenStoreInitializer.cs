using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OpenIddict.Abstractions;
using OpenIddict.MongoDb;
using OpenIddict.MongoDb.Models;
using Squidex.Hosting;
using Squidex.Infrastructure.Timers;
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace Squidex.Areas.IdentityServer.Config
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class TokenStoreInitializer : IInitializable, ISystem, IBackgroundProcess
	{
		private readonly OpenIddictMongoDbOptions options;

		private readonly IServiceProvider serviceProvider;

		private CompletionTimer timer;

		public TokenStoreInitializer(IOptions<OpenIddictMongoDbOptions> options, IServiceProvider serviceProvider)
		{
			this.options = options.get_Value();
			this.serviceProvider = serviceProvider;
		}

		public async Task InitializeAsync(CancellationToken ct)
		{
			await this.SetupIndexAsync(ct);
		}

		private async Task PruneAsync(CancellationToken ct)
		{
			ValueTask valueTask;
			object obj;
			AsyncServiceScope asyncServiceScope = ServiceProviderServiceExtensions.CreateAsyncScope(this.serviceProvider);
			object obj1 = null;
			try
			{
				IOpenIddictTokenManager requiredService = ServiceProviderServiceExtensions.GetRequiredService<IOpenIddictTokenManager>(asyncServiceScope.get_ServiceProvider());
				DateTimeOffset utcNow = DateTimeOffset.UtcNow;
				valueTask = requiredService.PruneAsync(utcNow.AddDays(-40), ct);
				await valueTask;
			}
			catch
			{
				obj = obj2;
				obj1 = obj;
			}
			valueTask = asyncServiceScope.DisposeAsync();
			await valueTask;
			obj = obj1;
			if (obj != null)
			{
				Exception exception = obj as Exception;
				if (exception == null)
				{
					throw obj;
				}
				ExceptionDispatchInfo.Capture(exception).Throw();
			}
			obj1 = null;
			asyncServiceScope = new AsyncServiceScope();
		}

		private async Task SetupIndexAsync(CancellationToken ct)
		{
			object obj;
			AsyncServiceScope asyncServiceScope = ServiceProviderServiceExtensions.CreateAsyncScope(this.serviceProvider);
			object obj1 = null;
			try
			{
				ValueTask<IMongoDatabase> databaseAsync = ServiceProviderServiceExtensions.GetRequiredService<IOpenIddictMongoDbContext>(asyncServiceScope.get_ServiceProvider()).GetDatabaseAsync(ct);
				IMongoDatabase mongoDatabase = await databaseAsync;
				IMongoCollection<OpenIddictMongoDbToken<string>> collection = mongoDatabase.GetCollection<OpenIddictMongoDbToken<string>>(this.options.get_TokensCollectionName(), null);
				IMongoIndexManager<OpenIddictMongoDbToken<string>> indexes = collection.get_Indexes();
				IndexKeysDefinitionBuilder<OpenIddictMongoDbToken<string>> indexKeys = Builders<OpenIddictMongoDbToken<string>>.get_IndexKeys();
				ParameterExpression parameterExpression = Expression.Parameter(typeof(OpenIddictMongoDbToken<string>), "x");
				MemberExpression memberExpression = Expression.Property(parameterExpression, (MethodInfo)MethodBase.GetMethodFromHandle(typeof(OpenIddictMongoDbToken<string>).GetMethod("get_ReferenceId").MethodHandle, typeof(OpenIddictMongoDbToken<string>).TypeHandle));
				ParameterExpression[] parameterExpressionArray = new ParameterExpression[] { parameterExpression };
				await indexes.CreateOneAsync(new CreateIndexModel<OpenIddictMongoDbToken<string>>(indexKeys.Ascending(Expression.Lambda<Func<OpenIddictMongoDbToken<string>, object>>(memberExpression, parameterExpressionArray)), null), null, ct);
			}
			catch
			{
				obj = obj2;
				obj1 = obj;
			}
			await asyncServiceScope.DisposeAsync();
			obj = obj1;
			if (obj != null)
			{
				Exception exception = obj as Exception;
				if (exception == null)
				{
					throw obj;
				}
				ExceptionDispatchInfo.Capture(exception).Throw();
			}
			obj1 = null;
			asyncServiceScope = new AsyncServiceScope();
		}

		public async Task StartAsync(CancellationToken ct)
		{
			TimeSpan timeSpan = TimeSpan.FromHours(6);
			this.timer = new CompletionTimer((int)timeSpan.TotalMilliseconds, new Func<CancellationToken, Task>(this.PruneAsync), 0);
			await this.PruneAsync(ct);
		}

		public Task StopAsync(CancellationToken ct)
		{
			object completedTask;
			CompletionTimer completionTimer = this.timer;
			if (completionTimer != null)
			{
				completedTask = completionTimer.StopAsync();
			}
			else
			{
				completedTask = null;
			}
			if (completedTask == null)
			{
				completedTask = Task.CompletedTask;
			}
			return completedTask;
		}
	}
}