using OpenIddict.Abstractions;
using Squidex.Domain.Users.InMemory;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.IdentityServer.Config
{
	public static class IdentityServerConfiguration
	{
		public sealed class Scopes : InMemoryScopeStore
		{
			public Scopes() : base(IdentityServerConfiguration.Scopes.BuildScopes())
			{
			}

			[return: Nullable(new byte[] { 1, 0, 1, 1 })]
			private static IEnumerable<ValueTuple<string, OpenIddictScopeDescriptor>> BuildScopes()
			{
				OpenIddictScopeDescriptor openIddictScopeDescriptor = new OpenIddictScopeDescriptor();
				openIddictScopeDescriptor.set_Name("squidex-api");
				openIddictScopeDescriptor.get_Resources().Add("scp:squidex-api");
				yield return new ValueTuple<string, OpenIddictScopeDescriptor>("squidex-api", openIddictScopeDescriptor);
			}
		}
	}
}