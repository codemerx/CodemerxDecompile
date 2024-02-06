using Squidex.Domain.Apps.Entities.Assets.Commands;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Assets.Models
{
	public sealed class BulkUpdateAssetsDto
	{
		public bool CheckReferrers
		{
			get;
			set;
		}

		public bool DoNotScript { get; set; } = true;

		[LocalizedRequired]
		[Nullable(new byte[] { 2, 1 })]
		public BulkUpdateAssetsJobDto[] Jobs
		{
			[return: Nullable(new byte[] { 2, 1 })]
			get;
			set;
		}

		public bool OptimizeValidation { get; set; } = true;

		public BulkUpdateAssetsDto()
		{
		}

		[NullableContext(1)]
		public BulkUpdateAssets ToCommand()
		{
			BulkUpdateJob[] array;
			BulkUpdateAssets bulkUpdateAsset = SimpleMapper.Map<BulkUpdateAssetsDto, BulkUpdateAssets>(this, new BulkUpdateAssets());
			BulkUpdateAssets bulkUpdateAsset1 = bulkUpdateAsset;
			BulkUpdateAssetsJobDto[] jobs = this.Jobs;
			if (jobs != null)
			{
				IEnumerable<BulkUpdateJob> job = 
					from x in (IEnumerable<BulkUpdateAssetsJobDto>)jobs
					select x.ToJob();
				if (job != null)
				{
					array = job.ToArray<BulkUpdateJob>();
				}
				else
				{
					array = null;
				}
			}
			else
			{
				array = null;
			}
			bulkUpdateAsset1.set_Jobs(array);
			return bulkUpdateAsset;
		}
	}
}