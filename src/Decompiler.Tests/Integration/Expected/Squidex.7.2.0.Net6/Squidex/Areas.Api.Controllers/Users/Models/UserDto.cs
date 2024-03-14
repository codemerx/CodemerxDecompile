using Squidex.Areas.Api.Controllers.Users;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Security;
using Squidex.Infrastructure.Validation;
using Squidex.Shared.Identity;
using Squidex.Shared.Users;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Users.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class UserDto : Resource
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

		[LocalizedRequired]
		public string Id
		{
			get;
			set;
		}

		[LocalizedRequired]
		public bool IsLocked
		{
			get;
			set;
		}

		[LocalizedRequired]
		public IEnumerable<string> Permissions
		{
			get;
			set;
		}

		public UserDto()
		{
		}

		private UserDto CreateLinks(Resources resources)
		{
			var variable = new { id = this.Id };
			if (!(resources.get_Controller() is UserManagementController))
			{
				base.AddSelfLink(resources.Url<UsersController>((UsersController c) => "GetUser", variable));
			}
			else
			{
				base.AddSelfLink(resources.Url<UserManagementController>((UserManagementController c) => "GetUser", variable));
			}
			if (!Squidex.Web.Extensions.IsUser(resources.get_Controller(), this.Id))
			{
				if (resources.get_CanLockUser() && !this.IsLocked)
				{
					base.AddPutLink("lock", resources.Url<UserManagementController>((UserManagementController c) => "LockUser", variable), null);
				}
				if (resources.get_CanUnlockUser() && this.IsLocked)
				{
					base.AddPutLink("unlock", resources.Url<UserManagementController>((UserManagementController c) => "UnlockUser", variable), null);
				}
				base.AddDeleteLink("delete", resources.Url<UserManagementController>((UserManagementController x) => "DeleteUser", variable), null);
			}
			if (resources.get_CanUpdateUser())
			{
				base.AddPutLink("update", resources.Url<UserManagementController>((UserManagementController c) => "PutUser", variable), null);
			}
			base.AddGetLink("picture", resources.Url<UsersController>((UsersController c) => "GetUserPicture", variable), null);
			return this;
		}

		public static UserDto FromDomain(IUser user, Resources resources)
		{
			UserDto ids = SimpleMapper.Map<IUser, UserDto>(user, new UserDto());
			ids.DisplayName = SquidexClaimsExtensions.DisplayName(user.get_Claims());
			ids.Permissions = SquidexClaimsExtensions.Permissions(user.get_Claims()).ToIds();
			return ids.CreateLinks(resources);
		}
	}
}