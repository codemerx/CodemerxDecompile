using NodaTime;
using Squidex.Domain.Apps.Entities;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Translations;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Contents.Models
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class AllContentsByPostDto
	{
		public DomainId[] Ids
		{
			get;
			set;
		}

		public Instant? ScheduledFrom
		{
			get;
			set;
		}

		public Instant? ScheduledTo
		{
			get;
			set;
		}

		public AllContentsByPostDto()
		{
		}

		[NullableContext(1)]
		public Q ToQuery()
		{
			bool length;
			DomainId[] ids = this.Ids;
			if (ids != null)
			{
				length = ids.Length != 0;
			}
			else
			{
				length = false;
			}
			if (length)
			{
				return Q.get_Empty().WithIds(this.Ids);
			}
			if (!this.ScheduledFrom.HasValue || !this.ScheduledTo.HasValue)
			{
				throw new ValidationException(T.Get("contents.invalidAllQuery", null), null);
			}
			return Q.get_Empty().WithSchedule(this.ScheduledFrom.Value, this.ScheduledTo.Value);
		}
	}
}