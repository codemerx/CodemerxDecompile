using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Squidex.Areas.IdentityServer.Config
{
	[Nullable(new byte[] { 0, 1 })]
	[NullableContext(1)]
	public sealed class ApplicationManager<T> : OpenIddictApplicationManager<T>
	where T : class
	{
		public ApplicationManager(IOptionsMonitor<OpenIddictCoreOptions> options, IOpenIddictApplicationCache<T> cache, IOpenIddictApplicationStoreResolver resolver, ILogger<OpenIddictApplicationManager<T>> logger) : base(cache, logger, options, resolver)
		{
		}

		[return: Nullable(0)]
		protected override ValueTask<bool> ValidateClientSecretAsync(string secret, string comparand, CancellationToken cancellationToken = null)
		{
			return new ValueTask<bool>(string.Equals(secret, comparand, StringComparison.Ordinal));
		}
	}
}