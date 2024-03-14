using System;

namespace Squidex.Areas.Api.Controllers.Plans.Models
{
	public enum PlansLockedReason
	{
		None,
		NotOwner,
		NoPermission,
		ManagedByTeam
	}
}