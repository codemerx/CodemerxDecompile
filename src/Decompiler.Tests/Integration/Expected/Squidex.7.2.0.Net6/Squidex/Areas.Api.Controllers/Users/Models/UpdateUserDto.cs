using Squidex.Domain.Users;
using Squidex.Infrastructure.Security;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Users.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class UpdateUserDto
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

		[Nullable(2)]
		public string Password
		{
			[NullableContext(2)]
			get;
			[NullableContext(2)]
			set;
		}

		[LocalizedRequired]
		public string[] Permissions
		{
			get;
			set;
		}

		public UpdateUserDto()
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