using Squidex.Areas.Api.Controllers.Users;
using Squidex.Infrastructure.Validation;
using Squidex.Shared.Users;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Users.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class UsersDto : Resource
	{
		[LocalizedRequired]
		public UserDto[] Items
		{
			get;
			set;
		}

		public long Total
		{
			get;
			set;
		}

		public UsersDto()
		{
		}

		private UsersDto CreateLinks(Resources context)
		{
			base.AddSelfLink(context.Url<UserManagementController>((UserManagementController c) => "GetUsers", null));
			if (context.get_CanCreateUser())
			{
				base.AddPostLink("create", context.Url<UserManagementController>((UserManagementController c) => "PostUser", null), null);
			}
			return this;
		}

		public static UsersDto FromDomain(IEnumerable<IUser> items, long total, Resources resources)
		{
			return (new UsersDto()
			{
				Total = total,
				Items = (
					from x in items
					select UserDto.FromDomain(x, resources)).ToArray<UserDto>()
			}).CreateLinks(resources);
		}
	}
}