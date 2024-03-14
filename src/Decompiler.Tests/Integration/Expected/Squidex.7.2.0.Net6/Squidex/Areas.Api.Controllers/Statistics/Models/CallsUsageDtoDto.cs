using Squidex.Domain.Apps.Entities.Billing;
using Squidex.Infrastructure.UsageTracking;
using Squidex.Infrastructure.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Statistics.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class CallsUsageDtoDto
	{
		public long AllowedBytes
		{
			get;
			set;
		}

		public long AllowedCalls
		{
			get;
			set;
		}

		public double AverageElapsedMs
		{
			get;
			set;
		}

		public long BlockingApiCalls
		{
			get;
			set;
		}

		[LocalizedRequired]
		public Dictionary<string, CallsUsagePerDateDto[]> Details
		{
			get;
			set;
		}

		public long MonthBytes
		{
			get;
			set;
		}

		public long MonthCalls
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

		public CallsUsageDtoDto()
		{
		}

		public static CallsUsageDtoDto FromDomain(Plan plan, ApiStatsSummary summary, Dictionary<string, List<ApiStats>> details)
		{
			return new CallsUsageDtoDto()
			{
				AverageElapsedMs = summary.get_AverageElapsedMs(),
				BlockingApiCalls = plan.get_BlockingApiCalls(),
				AllowedBytes = plan.get_MaxApiBytes(),
				AllowedCalls = plan.get_MaxApiCalls(),
				TotalBytes = summary.get_TotalBytes(),
				TotalCalls = summary.get_TotalCalls(),
				MonthBytes = summary.get_MonthBytes(),
				MonthCalls = summary.get_MonthCalls(),
				Details = details.ToDictionary<KeyValuePair<string, List<ApiStats>>, string, CallsUsagePerDateDto[]>((KeyValuePair<string, List<ApiStats>> x) => x.Key, (KeyValuePair<string, List<ApiStats>> x) => x.Value.Select<ApiStats, CallsUsagePerDateDto>(new Func<ApiStats, CallsUsagePerDateDto>(CallsUsagePerDateDto.FromDomain)).ToArray<CallsUsagePerDateDto>())
			};
		}
	}
}