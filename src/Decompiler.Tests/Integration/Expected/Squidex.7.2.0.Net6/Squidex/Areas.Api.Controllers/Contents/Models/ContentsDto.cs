using Squidex.Areas.Api.Controllers.Contents;
using Squidex.Domain.Apps.Core.Contents;
using Squidex.Domain.Apps.Core.Schemas;
using Squidex.Domain.Apps.Entities;
using Squidex.Domain.Apps.Entities.Contents;
using Squidex.Domain.Apps.Entities.Schemas;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Validation;
using Squidex.Web;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.Contents.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class ContentsDto : Resource
	{
		[LocalizedRequired]
		public ContentDto[] Items
		{
			get;
			set;
		}

		[LocalizedRequired]
		public StatusInfoDto[] Statuses
		{
			get;
			set;
		}

		public long Total
		{
			get;
			set;
		}

		public ContentsDto()
		{
		}

		private async Task AssignStatusesAsync(IContentWorkflow workflow, ISchemaEntity schema)
		{
			StatusInfo[] allAsync = await workflow.GetAllAsync(schema);
			this.Statuses = allAsync.Select<StatusInfo, StatusInfoDto>(new Func<StatusInfo, StatusInfoDto>(StatusInfoDto.FromDomain)).ToArray<StatusInfoDto>();
		}

		private async Task CreateLinksAsync(Resources resources, IContentWorkflow workflow, ISchemaEntity schema)
		{
			var variable = new { app = resources.get_App(), schema = schema.get_SchemaDef().get_Name() };
			ContentsDto contentsDto = this;
			Resources resource = resources;
			((Resource)contentsDto).AddSelfLink(resource.Url<ContentsController>((ContentsController x) => "GetContents", variable));
			if (resources.CanCreateContent(variable.schema))
			{
				ContentsDto contentsDto1 = this;
				Resources resource1 = resources;
				((Resource)contentsDto1).AddPostLink("create", resource1.Url<ContentsController>((ContentsController x) => "PostContent", variable), null);
				bool flag = resources.CanChangeStatus(variable.schema);
				if (flag)
				{
					ValueTask<bool> valueTask = workflow.CanPublishInitialAsync(schema, resources.get_Context().get_UserPrincipal());
					flag = await valueTask;
				}
				if (flag)
				{
					var variable1 = new { app = variable.app, schema = variable.schema, publish = true };
					ContentsDto contentsDto2 = this;
					Resources resource2 = resources;
					((Resource)contentsDto2).AddPostLink("create/publish", resource2.Url<ContentsController>((ContentsController x) => "PostContent", variable1), null);
				}
			}
			variable = null;
		}

		public static async Task<ContentsDto> FromContentsAsync(IResultList<IEnrichedContentEntity> contents, Resources resources, [Nullable(2)] ISchemaEntity schema, IContentWorkflow workflow)
		{
			ContentsDto contentsDto = new ContentsDto()
			{
				Total = contents.get_Total(),
				Items = (
					from x in contents
					select ContentDto.FromDomain(x, resources)).ToArray<ContentDto>()
			};
			ContentsDto contentsDto1 = contentsDto;
			if (schema != null)
			{
				await contentsDto1.AssignStatusesAsync(workflow, schema);
				await contentsDto1.CreateLinksAsync(resources, workflow, schema);
			}
			ContentsDto contentsDto2 = contentsDto1;
			contentsDto1 = null;
			return contentsDto2;
		}
	}
}