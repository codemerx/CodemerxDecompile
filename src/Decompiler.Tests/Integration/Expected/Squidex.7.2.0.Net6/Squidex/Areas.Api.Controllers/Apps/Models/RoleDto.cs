using Squidex.Areas.Api.Controllers.Apps;
using Squidex.Domain.Apps.Core.Apps;
using Squidex.Domain.Apps.Entities.Apps;
using Squidex.Infrastructure.Json.Objects;
using Squidex.Infrastructure.Security;
using Squidex.Infrastructure.Validation;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Apps.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class RoleDto : Resource
	{
		public bool IsDefaultRole
		{
			get;
			set;
		}

		[LocalizedRequired]
		public string Name
		{
			get;
			set;
		}

		public int NumClients
		{
			get;
			set;
		}

		public int NumContributors
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

		[LocalizedRequired]
		public JsonObject Properties
		{
			get;
			set;
		}

		public RoleDto()
		{
		}

		public RoleDto CreateLinks(Resources resources)
		{
			var variable = new { app = resources.get_App(), roleName = this.Name };
			if (!this.IsDefaultRole)
			{
				if (resources.get_CanUpdateRole())
				{
					base.AddPutLink("update", resources.Url<AppRolesController>((AppRolesController x) => "PutRole", variable), null);
				}
				if (resources.get_CanDeleteRole() && this.NumClients == 0 && this.NumContributors == 0)
				{
					base.AddDeleteLink("delete", resources.Url<AppRolesController>((AppRolesController x) => "DeleteRole", variable), null);
				}
			}
			return this;
		}

		public static RoleDto FromDomain(Role role, IAppEntity app)
		{
			return new RoleDto()
			{
				Name = role.get_Name(),
				NumClients = RoleDto.GetNumClients(role, app),
				NumContributors = RoleDto.GetNumContributors(role, app),
				Permissions = role.get_Permissions().ToIds().ToArray<string>(),
				Properties = role.get_Properties(),
				IsDefaultRole = role.get_IsDefault()
			};
		}

		private static int GetNumClients(Role role, IAppEntity app)
		{
			return app.get_Clients().Count<KeyValuePair<string, AppClient>>((KeyValuePair<string, AppClient> x) => role.Equals(x.Value.get_Role()));
		}

		private static int GetNumContributors(Role role, IAppEntity app)
		{
			return app.get_Contributors().Count<KeyValuePair<string, string>>((KeyValuePair<string, string> x) => role.Equals(x.Value));
		}
	}
}