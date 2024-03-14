using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.IdentityServer.Controllers.Profile
{
	[Nullable(0)]
	[NullableContext(1)]
	public class SetPasswordModel
	{
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

		public SetPasswordModel()
		{
		}
	}
}