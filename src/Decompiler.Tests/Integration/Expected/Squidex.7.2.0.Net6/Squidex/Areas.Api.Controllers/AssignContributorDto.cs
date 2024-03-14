using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class AssignContributorDto
	{
		[LocalizedRequired]
		public string ContributorId
		{
			get;
			set;
		}

		public bool Invite
		{
			get;
			set;
		}

		[Nullable(2)]
		public string Role { [NullableContext(2)]
		get; [NullableContext(2)]
		set; } = "Developer";

		public AssignContributorDto()
		{
		}
	}
}