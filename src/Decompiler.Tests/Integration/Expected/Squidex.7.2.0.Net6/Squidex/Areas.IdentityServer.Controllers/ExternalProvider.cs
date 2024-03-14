using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.IdentityServer.Controllers
{
	[Nullable(0)]
	[NullableContext(1)]
	public class ExternalProvider
	{
		public string AuthenticationScheme
		{
			get;
		}

		public string DisplayName
		{
			get;
		}

		public ExternalProvider(string authenticationSchema, string displayName)
		{
			this.AuthenticationScheme = authenticationSchema;
			this.DisplayName = displayName;
		}
	}
}