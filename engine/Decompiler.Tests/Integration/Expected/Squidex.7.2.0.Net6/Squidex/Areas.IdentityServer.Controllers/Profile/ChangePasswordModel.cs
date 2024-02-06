using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.IdentityServer.Controllers.Profile
{
	[Nullable(0)]
	[NullableContext(1)]
	public class ChangePasswordModel
	{
		[LocalizedRequired]
		public string OldPassword
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

		[LocalizedCompare("Password")]
		public string PasswordConfirm
		{
			get;
			set;
		}

		public ChangePasswordModel()
		{
		}
	}
}