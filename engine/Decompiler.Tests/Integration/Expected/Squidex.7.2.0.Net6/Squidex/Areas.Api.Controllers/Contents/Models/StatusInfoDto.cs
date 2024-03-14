using Squidex.Domain.Apps.Core.Contents;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Contents.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class StatusInfoDto
	{
		[LocalizedRequired]
		public string Color
		{
			get;
			set;
		}

		[LocalizedRequired]
		public Squidex.Domain.Apps.Core.Contents.Status Status
		{
			get;
			set;
		}

		public StatusInfoDto()
		{
		}

		public static StatusInfoDto FromDomain(StatusInfo statusInfo)
		{
			return new StatusInfoDto()
			{
				Status = statusInfo.get_Status(),
				Color = statusInfo.get_Color()
			};
		}
	}
}