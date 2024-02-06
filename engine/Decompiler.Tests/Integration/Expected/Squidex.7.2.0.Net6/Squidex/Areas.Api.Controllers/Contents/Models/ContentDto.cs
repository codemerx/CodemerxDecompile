using NodaTime;
using Squidex.Areas.Api.Controllers.Contents;
using Squidex.Areas.Api.Controllers.Schemas.Models;
using Squidex.Domain.Apps.Core.Contents;
using Squidex.Domain.Apps.Core.ConvertContent;
using Squidex.Domain.Apps.Core.Schemas;
using Squidex.Domain.Apps.Entities.Contents;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using Squidex.Web;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Contents.Models
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class ContentDto : Resource
	{
		public Instant Created
		{
			get;
			set;
		}

		[LocalizedRequired]
		[Nullable(1)]
		public RefToken CreatedBy
		{
			[NullableContext(1)]
			get;
			[NullableContext(1)]
			set;
		}

		[LocalizedRequired]
		[Nullable(1)]
		public object Data
		{
			[NullableContext(1)]
			get;
			[NullableContext(1)]
			set;
		}

		public string EditToken
		{
			get;
			set;
		}

		public DomainId Id
		{
			get;
			set;
		}

		public bool IsDeleted
		{
			get;
			set;
		}

		public Instant LastModified
		{
			get;
			set;
		}

		[LocalizedRequired]
		[Nullable(1)]
		public RefToken LastModifiedBy
		{
			[NullableContext(1)]
			get;
			[NullableContext(1)]
			set;
		}

		public Squidex.Domain.Apps.Core.Contents.Status? NewStatus
		{
			get;
			set;
		}

		public string NewStatusColor
		{
			get;
			set;
		}

		public ContentData ReferenceData
		{
			get;
			set;
		}

		[Nullable(new byte[] { 2, 1 })]
		public FieldDto[] ReferenceFields
		{
			[return: Nullable(new byte[] { 2, 1 })]
			get;
			set;
		}

		public ScheduleJobDto ScheduleJob
		{
			get;
			set;
		}

		public string SchemaDisplayName
		{
			get;
			set;
		}

		public DomainId SchemaId
		{
			get;
			set;
		}

		public string SchemaName
		{
			get;
			set;
		}

		public Squidex.Domain.Apps.Core.Contents.Status Status
		{
			get;
			set;
		}

		[Nullable(1)]
		public string StatusColor
		{
			[NullableContext(1)]
			get;
			[NullableContext(1)]
			set;
		}

		public long Version
		{
			get;
			set;
		}

		public ContentDto()
		{
		}

		[NullableContext(1)]
		private ContentDto CreateLinksAsync(IEnrichedContentEntity content, Resources resources, string schema)
		{
			string app = resources.get_App();
			var variable = new { app = app, schema = schema, id = this.Id };
			base.AddSelfLink(resources.Url<ContentsController>((ContentsController x) => "GetContent", variable));
			if (this.Version > (long)0)
			{
				var variable1 = new { app = app, schema = schema, id = variable.id, version = this.Version - (long)1 };
				base.AddGetLink("previous", resources.Url<ContentsController>((ContentsController x) => "GetContentVersion", variable1), null);
			}
			if (this.NewStatus.HasValue)
			{
				if (resources.CanDeleteContentVersion(schema))
				{
					base.AddDeleteLink("draft/delete", resources.Url<ContentsController>((ContentsController x) => "DeleteVersion", variable), null);
				}
			}
			else if (this.Status == Squidex.Domain.Apps.Core.Contents.Status.Published && resources.CanCreateContentVersion(schema))
			{
				base.AddPostLink("draft/create", resources.Url<ContentsController>((ContentsController x) => "CreateDraft", variable), null);
			}
			if (content.get_NextStatuses() != null && resources.CanChangeStatus(schema))
			{
				StatusInfo[] nextStatuses = content.get_NextStatuses();
				for (int i = 0; i < (int)nextStatuses.Length; i++)
				{
					StatusInfo statusInfo = nextStatuses[i];
					DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(7, 1);
					defaultInterpolatedStringHandler.AppendLiteral("status/");
					defaultInterpolatedStringHandler.AppendFormatted<Squidex.Domain.Apps.Core.Contents.Status>(statusInfo.get_Status());
					base.AddPutLink(defaultInterpolatedStringHandler.ToStringAndClear(), resources.Url<ContentsController>((ContentsController x) => "PutContentStatus", variable), statusInfo.get_Color());
				}
			}
			if (content.get_ScheduleJob() != null && resources.CanCancelContentStatus(schema))
			{
				base.AddDeleteLink("cancel", resources.Url<ContentsController>((ContentsController x) => "DeleteContentStatus", variable), null);
			}
			if (!content.get_IsSingleton() && resources.CanDeleteContent(schema))
			{
				base.AddDeleteLink("delete", resources.Url<ContentsController>((ContentsController x) => "DeleteContent", variable), null);
			}
			if (content.get_CanUpdate() && resources.CanUpdateContent(schema))
			{
				base.AddPatchLink("patch", resources.Url<ContentsController>((ContentsController x) => "PatchContent", variable), null);
			}
			if (content.get_CanUpdate() && resources.CanUpdateContent(schema))
			{
				base.AddPutLink("update", resources.Url<ContentsController>((ContentsController x) => "PutContent", variable), null);
			}
			return this;
		}

		[NullableContext(1)]
		public static ContentDto FromDomain(IEnrichedContentEntity content, Resources resources)
		{
			ContentDto data = SimpleMapper.Map<IEnrichedContentEntity, ContentDto>(content, new ContentDto()
			{
				SchemaId = content.get_SchemaId().get_Id(),
				SchemaName = content.get_SchemaId().get_Name()
			});
			if (!ContentExtensions.ShouldFlatten(resources.get_Context()))
			{
				data.Data = content.get_Data();
			}
			else
			{
				data.Data = ContentConverterFlat.ToFlatten(content.get_Data());
			}
			if (content.get_ReferenceFields() != null)
			{
				data.ReferenceFields = content.get_ReferenceFields().Select<RootField, FieldDto>(new Func<RootField, FieldDto>(FieldDto.FromDomain)).ToArray<FieldDto>();
			}
			if (content.get_ScheduleJob() != null)
			{
				data.ScheduleJob = new ScheduleJobDto()
				{
					Color = content.get_ScheduledStatusColor()
				};
				SimpleMapper.Map<Squidex.Domain.Apps.Entities.Contents.ScheduleJob, ScheduleJobDto>(content.get_ScheduleJob(), data.ScheduleJob);
			}
			if (data.IsDeleted)
			{
				return data;
			}
			return data.CreateLinksAsync(content, resources, content.get_SchemaId().get_Name());
		}
	}
}