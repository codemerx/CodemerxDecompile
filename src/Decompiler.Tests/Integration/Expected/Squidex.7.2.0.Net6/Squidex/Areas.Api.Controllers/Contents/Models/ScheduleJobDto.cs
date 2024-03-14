using NodaTime;
using Squidex.Domain.Apps.Core.Contents;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Contents.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class ScheduleJobDto
	{
		public string Color
		{
			get;
			set;
		}

		public Instant DueTime
		{
			get;
			set;
		}

		public DomainId Id
		{
			get;
			set;
		}

		[LocalizedRequired]
		public RefToken ScheduledBy
		{
			get;
			set;
		}

		public Squidex.Domain.Apps.Core.Contents.Status Status
		{
			get;
			set;
		}

		public ScheduleJobDto()
		{
		}
	}
}