using Squidex.Areas.Api.Controllers.Apps;
using Squidex.Areas.Api.Controllers.Teams;
using Squidex.Infrastructure.Translations;
using Squidex.Infrastructure.Validation;
using Squidex.Shared.Identity;
using Squidex.Shared.Users;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class ContributorDto : Resource
	{
		[LocalizedRequired]
		public string ContributorEmail
		{
			get;
			set;
		}

		[LocalizedRequired]
		public string ContributorId
		{
			get;
			set;
		}

		[LocalizedRequired]
		public string ContributorName
		{
			get;
			set;
		}

		[Nullable(2)]
		public string Role
		{
			[NullableContext(2)]
			get;
			[NullableContext(2)]
			set;
		}

		public ContributorDto()
		{
		}

		public ContributorDto CreateAppLinks(Resources resources)
		{
			if (resources.IsUser(this.ContributorId))
			{
				return this;
			}
			string app = resources.get_App();
			if (resources.get_CanAssignContributor())
			{
				var variable = new { app = app };
				base.AddPostLink("update", resources.Url<AppContributorsController>((AppContributorsController x) => "PostContributor", variable), null);
			}
			if (resources.get_CanRevokeContributor())
			{
				var variable1 = new { app = app, id = this.ContributorId };
				base.AddDeleteLink("delete", resources.Url<AppContributorsController>((AppContributorsController x) => "DeleteContributor", variable1), null);
			}
			return this;
		}

		public ContributorDto CreateTeamLinks(Resources resources)
		{
			if (resources.IsUser(this.ContributorId))
			{
				return this;
			}
			string team = resources.get_Team();
			if (resources.get_CanAssignTeamContributor())
			{
				var variable = new { team = team };
				base.AddPostLink("update", resources.Url<TeamContributorsController>((TeamContributorsController x) => "PostContributor", variable), null);
			}
			if (resources.get_CanRevokeTeamContributor())
			{
				var variable1 = new { team = team, id = this.ContributorId };
				base.AddDeleteLink("delete", resources.Url<TeamContributorsController>((TeamContributorsController x) => "DeleteContributor", variable1), null);
			}
			return this;
		}

		public ContributorDto CreateUser(IDictionary<string, IUser> users)
		{
			IUser user;
			if (!users.TryGetValue(this.ContributorId, out user))
			{
				this.ContributorName = T.Get("common.notFoundValue", null);
			}
			else
			{
				this.ContributorName = SquidexClaimsExtensions.DisplayName(user.get_Claims());
				this.ContributorEmail = user.get_Email();
			}
			return this;
		}

		public static ContributorDto FromDomain(string id, string role)
		{
			return new ContributorDto()
			{
				ContributorId = id,
				Role = role
			};
		}
	}
}