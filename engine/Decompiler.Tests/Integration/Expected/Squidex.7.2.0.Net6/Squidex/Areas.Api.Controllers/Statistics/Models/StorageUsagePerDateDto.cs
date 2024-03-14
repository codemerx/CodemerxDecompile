using NodaTime;
using Squidex.Domain.Apps.Entities.Assets;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Statistics.Models
{
	public sealed class StorageUsagePerDateDto
	{
		public LocalDate Date
		{
			get;
			set;
		}

		public long TotalCount
		{
			get;
			set;
		}

		public long TotalSize
		{
			get;
			set;
		}

		public StorageUsagePerDateDto()
		{
		}

		[NullableContext(1)]
		public static StorageUsagePerDateDto FromDomain(AssetStats stats)
		{
			return new StorageUsagePerDateDto()
			{
				Date = LocalDate.FromDateTime(DateTime.SpecifyKind(stats.get_Date(), DateTimeKind.Utc)),
				TotalCount = stats.get_TotalCount(),
				TotalSize = stats.get_TotalSize()
			};
		}
	}
}