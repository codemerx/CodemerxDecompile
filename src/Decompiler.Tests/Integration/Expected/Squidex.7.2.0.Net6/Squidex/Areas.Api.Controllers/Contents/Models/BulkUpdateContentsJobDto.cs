using NodaTime;
using Squidex.Areas.Api.Controllers;
using Squidex.Domain.Apps.Core.Contents;
using Squidex.Domain.Apps.Entities.Contents.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Reflection;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Contents.Models
{
	[Nullable(0)]
	[NullableContext(2)]
	public class BulkUpdateContentsJobDto
	{
		public ContentData Data
		{
			get;
			set;
		}

		public Instant? DueTime
		{
			get;
			set;
		}

		public long ExpectedCount { get; set; } = (long)1;

		public long ExpectedVersion { get; set; } = (long)-2;

		public DomainId? Id
		{
			get;
			set;
		}

		public bool Patch
		{
			get;
			set;
		}

		public bool Permanent
		{
			get;
			set;
		}

		public QueryJsonDto Query
		{
			get;
			set;
		}

		public string Schema
		{
			get;
			set;
		}

		public Squidex.Domain.Apps.Core.Contents.Status? Status
		{
			get;
			set;
		}

		public BulkUpdateContentType Type
		{
			get;
			set;
		}

		public BulkUpdateContentsJobDto()
		{
		}

		[NullableContext(1)]
		public BulkUpdateJob ToJob()
		{
			BulkUpdateJob bulkUpdateJob = new BulkUpdateJob();
			bulkUpdateJob.set_Query(this.Query);
			return SimpleMapper.Map<BulkUpdateContentsJobDto, BulkUpdateJob>(this, bulkUpdateJob);
		}
	}
}