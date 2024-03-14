using Squidex.Domain.Apps.Entities.Apps.Commands;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Apps.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class AddRoleDto
	{
		[LocalizedRequired]
		public string Name
		{
			get;
			set;
		}

		public AddRoleDto()
		{
		}

		public AddRole ToCommand()
		{
			AddRole addRole = new AddRole();
			addRole.set_Name(this.Name);
			return addRole;
		}
	}
}