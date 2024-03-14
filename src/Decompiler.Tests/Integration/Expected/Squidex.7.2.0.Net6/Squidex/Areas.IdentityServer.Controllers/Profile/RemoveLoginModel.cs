using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.IdentityServer.Controllers.Profile
{
	[Nullable(0)]
	[NullableContext(1)]
	public class RemoveLoginModel
	{
		[LocalizedRequired]
		public string LoginProvider
		{
			get;
			set;
		}

		[LocalizedRequired]
		public string ProviderKey
		{
			get;
			set;
		}

		public RemoveLoginModel()
		{
		}
	}
}