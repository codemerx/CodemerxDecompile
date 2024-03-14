using Squidex.Areas.Api.Controllers.Apps;
using Squidex.Domain.Apps.Core.Apps;
using Squidex.Domain.Apps.Entities.Apps;
using Squidex.Infrastructure.Validation;
using Squidex.Web;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Apps.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class RolesDto : Resource
	{
		[LocalizedRequired]
		public RoleDto[] Items
		{
			get;
			set;
		}

		public RolesDto()
		{
		}

		private RolesDto CreateLinks(Resources resources)
		{
			var variable = new { app = resources.get_App() };
			base.AddSelfLink(resources.Url<AppRolesController>((AppRolesController x) => "GetRoles", variable));
			if (resources.get_CanCreateRole())
			{
				base.AddPostLink("create", resources.Url<AppRolesController>((AppRolesController x) => "PostRole", variable), null);
			}
			return this;
		}

		public static RolesDto FromDomain(IAppEntity app, Resources resources)
		{
			return (new RolesDto()
			{
				Items = (
					from x in app.get_Roles().get_All()
					select RoleDto.FromDomain(x, app) into x
					select x.CreateLinks(resources) into x
					orderby x.Name
					select x).ToArray<RoleDto>()
			}).CreateLinks(resources);
		}
	}
}