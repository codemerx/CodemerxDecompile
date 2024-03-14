using NodaTime;
using Squidex.Infrastructure.UsageTracking;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Statistics.Models
{
	public sealed class CallsUsagePerDateDto
	{
		public double AverageElapsedMs
		{
			get;
			set;
		}

		public LocalDate Date
		{
			get;
			set;
		}

		public long TotalBytes
		{
			get;
			set;
		}

		public long TotalCalls
		{
			get;
			set;
		}

		public CallsUsagePerDateDto()
		{
		}

		[NullableContext(1)]
		public static CallsUsagePerDateDto FromDomain(ApiStats stats)
		{
			return new CallsUsagePerDateDto()
			{
				Date = LocalDate.FromDateTime(DateTime.SpecifyKind(stats.get_Date(), DateTimeKind.Utc)),
				TotalBytes = stats.get_TotalBytes(),
				TotalCalls = stats.get_TotalCalls(),
				AverageElapsedMs = stats.get_AverageElapsedMs()
			};
		}
	}
}