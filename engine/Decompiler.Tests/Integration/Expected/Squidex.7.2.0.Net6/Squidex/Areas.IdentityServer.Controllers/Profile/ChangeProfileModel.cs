using Squidex.Domain.Users;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.IdentityServer.Controllers.Profile
{
	[Nullable(0)]
	[NullableContext(1)]
	public class ChangeProfileModel
	{
		[LocalizedRequired]
		public string DisplayName
		{
			get;
			set;
		}

		[LocalizedRequired]
		public string Email
		{
			get;
			set;
		}

		public bool IsHidden
		{
			get;
			set;
		}

		public ChangeProfileModel()
		{
		}

		public UserValues ToValues()
		{
			UserValues userValue = new UserValues();
			userValue.set_Email(this.Email);
			userValue.set_DisplayName(this.DisplayName);
			userValue.set_Hidden(new bool?(this.IsHidden));
			return userValue;
		}
	}
}