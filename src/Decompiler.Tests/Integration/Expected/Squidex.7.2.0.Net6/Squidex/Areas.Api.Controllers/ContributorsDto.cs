using Squidex.Areas.Api.Controllers.Apps;
using Squidex.Areas.Api.Controllers.Teams;
using Squidex.Domain.Apps.Core;
using Squidex.Domain.Apps.Entities.Apps;
using Squidex.Domain.Apps.Entities.Billing;
using Squidex.Domain.Apps.Entities.Teams;
using Squidex.Infrastructure.Collections;
using Squidex.Infrastructure.Validation;
using Squidex.Shared.Users;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class ContributorsDto : Resource
	{
		[LocalizedRequired]
		public ContributorDto[] Items
		{
			get;
			set;
		}

		public long MaxContributors
		{
			get;
			set;
		}

		[JsonPropertyName("_meta")]
		[Nullable(2)]
		public ContributorsMetadata Metadata
		{
			[NullableContext(2)]
			get;
			[NullableContext(2)]
			set;
		}

		public ContributorsDto()
		{
		}

		private ContributorsDto CreateAppLinks(Resources resources)
		{
			var variable = new { app = resources.get_App() };
			base.AddSelfLink(resources.Url<AppContributorsController>((AppContributorsController x) => "GetContributors", variable));
			if (resources.get_CanAssignContributor() && (this.MaxContributors < (long)0 || (long)((int)this.Items.Length) < this.MaxContributors))
			{
				base.AddPostLink("create", resources.Url<AppContributorsController>((AppContributorsController x) => "PostContributor", variable), null);
			}
			return this;
		}

		private void CreateInvited(bool isInvited)
		{
			if (isInvited)
			{
				this.Metadata = new ContributorsMetadata()
				{
					IsInvited = "true"
				};
			}
		}

		private void CreatePlan(Plan plan)
		{
			this.MaxContributors = plan.get_MaxContributors();
		}

		private ContributorsDto CreateTeamLinks(Resources resources)
		{
			var variable = new { team = resources.get_Team() };
			base.AddSelfLink(resources.Url<TeamContributorsController>((TeamContributorsController x) => "GetContributors", variable));
			if (resources.get_CanAssignTeamContributor())
			{
				base.AddPostLink("create", resources.Url<TeamContributorsController>((TeamContributorsController x) => "PostContributor", variable), null);
			}
			return this;
		}

		public static async Task<ContributorsDto> FromDomainAsync(IAppEntity app, Resources resources, IUserResolver userResolver, Plan plan, bool invited)
		{
			IUserResolver userResolver1 = userResolver;
			string[] array = app.get_Contributors().get_Keys().ToArray<string>();
			Dictionary<string, IUser> strs = await userResolver1.QueryManyAsync(array, new CancellationToken());
			ContributorsDto contributorsDto = new ContributorsDto();
			Contributors contributors = app.get_Contributors();
			IEnumerable<ContributorDto> contributorDtos = 
				from x in contributors
				select ContributorDto.FromDomain(x.Key, x.Value) into x
				select x.CreateUser(strs) into x
				select x.CreateAppLinks(resources);
			contributorsDto.Items = (
				from x in contributorDtos
				orderby x.ContributorName
				select x).ToArray<ContributorDto>();
			ContributorsDto contributorsDto1 = contributorsDto;
			contributorsDto1.CreateInvited(invited);
			contributorsDto1.CreatePlan(plan);
			ContributorsDto contributorsDto2 = contributorsDto1.CreateAppLinks(resources);
			return contributorsDto2;
		}

		public static async Task<ContributorsDto> FromDomainAsync(ITeamEntity team, Resources resources, IUserResolver userResolver, bool invited)
		{
			IUserResolver userResolver1 = userResolver;
			string[] array = team.get_Contributors().get_Keys().ToArray<string>();
			Dictionary<string, IUser> strs = await userResolver1.QueryManyAsync(array, new CancellationToken());
			ContributorsDto contributorsDto = new ContributorsDto();
			Contributors contributors = team.get_Contributors();
			IEnumerable<ContributorDto> contributorDtos = 
				from x in contributors
				select ContributorDto.FromDomain(x.Key, x.Value) into x
				select x.CreateUser(strs) into x
				select x.CreateTeamLinks(resources);
			contributorsDto.Items = (
				from x in contributorDtos
				orderby x.ContributorName
				select x).ToArray<ContributorDto>();
			ContributorsDto contributorsDto1 = contributorsDto;
			contributorsDto1.CreateInvited(invited);
			ContributorsDto contributorsDto2 = contributorsDto1.CreateTeamLinks(resources);
			return contributorsDto2;
		}
	}
}