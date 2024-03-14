using NodaTime;
using Squidex.Domain.Apps.Entities.Backup;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Backups.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class RestoreJobDto
	{
		[LocalizedRequired]
		public List<string> Log
		{
			get;
			set;
		}

		public Instant Started
		{
			get;
			set;
		}

		public JobStatus Status
		{
			get;
			set;
		}

		public Instant? Stopped
		{
			get;
			set;
		}

		[LocalizedRequired]
		public Uri Url
		{
			get;
			set;
		}

		public RestoreJobDto()
		{
		}

		public static RestoreJobDto FromDomain(IRestoreJob job)
		{
			return SimpleMapper.Map<IRestoreJob, RestoreJobDto>(job, new RestoreJobDto());
		}
	}
}