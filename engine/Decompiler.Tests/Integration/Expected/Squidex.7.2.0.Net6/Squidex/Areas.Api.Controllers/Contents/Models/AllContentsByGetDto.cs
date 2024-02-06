using Microsoft.AspNetCore.Mvc;
using NodaTime;
using Squidex.Domain.Apps.Entities;
using Squidex.Infrastructure.Translations;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Contents.Models
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class AllContentsByGetDto
	{
		[FromQuery(Name="ids")]
		public string Ids
		{
			get;
			set;
		}

		[FromQuery]
		public Instant? ScheduledFrom
		{
			get;
			set;
		}

		[FromQuery]
		public Instant? ScheduledTo
		{
			get;
			set;
		}

		public AllContentsByGetDto()
		{
		}

		[NullableContext(1)]
		public Q ToQuery()
		{
			if (!string.IsNullOrWhiteSpace(this.Ids))
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