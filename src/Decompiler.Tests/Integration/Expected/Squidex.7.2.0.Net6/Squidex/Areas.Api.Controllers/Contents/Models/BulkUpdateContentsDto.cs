using Squidex.Domain.Apps.Core.Contents;
using Squidex.Domain.Apps.Entities.Contents.Commands;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Contents.Models
{
	public sealed class BulkUpdateContentsDto
	{
		public bool CheckReferrers
		{
			get;
			set;
		}

		public bool DoNotScript { get; set; } = true;

		public bool DoNotValidate
		{
			get;
			set;
		}

		public bool DoNotValidateWorkflow
		{
			get;
			set;
		}

		[LocalizedRequired]
		[Nullable(new byte[] { 2, 1 })]
		public BulkUpdateContentsJobDto[] Jobs
		{
			[return: Nullable(new byte[] { 2, 1 })]
			get;
			set;
		}

		public bool OptimizeValidation { get; set; } = true;

		[Obsolete("Use 'jobs.status' fields now.")]
		public bool Publish
		{
			get;
			set;
		}

		public BulkUpdateContentsDto()
		{
		}

		[NullableContext(1)]
		public BulkUpdateContents ToCommand(bool setSchema)
		{
			BulkUpdateJob[] array;
			BulkUpdateContents bulkUpdateContent = SimpleMapper.Map<BulkUpdateContentsDto, BulkUpdateContents>(this, new BulkUpdateContents());
			BulkUpdateContents bulkUpdateContent1 = bulkUpdateContent;
			BulkUpdateContentsJobDto[] jobs = this.Jobs;
			if (jobs != null)
			{
				array = ((IEnumerable<BulkUpdateContentsJobDto>)jobs).Select<BulkUpdateContentsJobDto, BulkUpdateJob>((BulkUpdateContentsJobDto x) => {
					BulkUpdateJob job = x.ToJob();
					if (this.Publish)
					{
						job.set_Status(new Status?(Status.Published));
					}
					return job;
				}).ToArray<BulkUpdateJob>();
			}
			else
			{
				array = null;
			}
			bulkUpdateContent1.set_Jobs(array);
			if (setSchema)
			{
				bulkUpdateContent.set_SchemaId(BulkUpdateContents.NoSchema);
			}
			return bulkUpdateContent;
		}
	}
}