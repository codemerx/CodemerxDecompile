using Squidex.Domain.Apps.Entities.Apps.Commands;
using Squidex.Infrastructure.Json.Objects;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Apps.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class UpdateRoleDto
	{
		[LocalizedRequired]
		public string[] Permissions
		{
			get;
			set;
		}

		public JsonObject Properties
		{
			get;
			set;
		}

		public UpdateRoleDto()
		{
		}

		public UpdateRole ToCommand(string name)
		{
			UpdateRole updateRole = new UpdateRole();
			updateRole.set_Name(name);
			return SimpleMapper.Map<UpdateRoleDto, UpdateRole>(this, updateRole);
		}
	}
}