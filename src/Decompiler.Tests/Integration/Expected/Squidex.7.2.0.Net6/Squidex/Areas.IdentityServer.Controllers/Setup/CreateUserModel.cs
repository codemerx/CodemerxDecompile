using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.IdentityServer.Controllers.Setup
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class CreateUserModel
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

		[LocalizedRequired]
		public string PasswordConfirm
		{
			get;
			set;
		}

		public CreateUserModel()
		{
		}
	}
}