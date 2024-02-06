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
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class ImportContentsDto
	{
		[LocalizedRequired]
		public List<ContentData> Datas
		{
			get;
			set;
		}

		public bool DoNotScript { get; set; } = true;

		public bool OptimizeValidation { get; set; } = true;

		[Obsolete("Use bulk endpoint now.")]
		public bool Publish
		{
			get;
			set;
		}

		public ImportContentsDto()
		{
		}

		public BulkUpdateContents ToCommand()
		{
			BulkUpdateJob[] array;
			BulkUpdateContents bulkUpdateContent = SimpleMapper.Map<ImportContentsDto, BulkUpdateContents>(this, new BulkUpdateContents());
			BulkUpdateContents bulkUpdateContent1 = bulkUpdateContent;
			List<ContentData> datas = this.Datas;
			if (datas != null)
			{
				array = datas.Select<ContentData, BulkUpdateJob>((ContentData x) => {
					BulkUpdateJob bulkUpdateJob = new BulkUpdateJob();
					bulkUpdateJob.set_Type(2);
					bulkUpdateJob.set_Data(x);
					BulkUpdateJob bulkUpdateJob1 = bulkUpdateJob;
					if (this.Publish)
					{
						bulkUpdateJob1.set_Status(new Status?(Status.Published));
					}
					return bulkUpdateJob1;
				}).ToArray<BulkUpdateJob>();
			}
			else
			{
				array = null;
			}
			bulkUpdateContent1.set_Jobs(array);
			return bulkUpdateContent;
		}
	}
}