using Squidex.Domain.Users;
using Squidex.Infrastructure.Security;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Users.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class CreateUserDto
	{
		[LocalizedRequired]
		public string DisplayName
		{
			get;
			set;
		}

		[LocalizedEmailAddress]
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
		public string[] Permissions
		{
			get;
			set;
		}

		public CreateUserDto()
		{
		}

		public UserValues ToValues()
		{
			UserValues userValue = new UserValues();
			userValue.set_Email(this.Email);
			userValue.set_DisplayName(this.DisplayName);
			userValue.set_Password(this.Password);
			userValue.set_Permissions(new PermissionSet(this.Permissions));
			return userValue;
		}
	}
}