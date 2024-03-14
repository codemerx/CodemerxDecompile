using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.IdentityServer.Controllers.Account
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class LoginModel
	{
		[LocalizedRequired]
		public string Email
		{
			get;
			set;
		}

		[LocalizedRequired]
		public string Password
		{
			get;
			set;
		}

		public LoginModel()
		{
		}
	}
}