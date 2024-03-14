using NodaTime;
using Squidex.Domain.Apps.Entities.History;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.History.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class HistoryEventDto
	{
		[LocalizedRequired]
		public string Actor
		{
			get;
			set;
		}

		public Instant Created
		{
			get;
			set;
		}

		public DomainId EventId
		{
			get;
			set;
		}

		[LocalizedRequired]
		public string EventType
		{
			get;
			set;
		}

		[LocalizedRequired]
		public string Message
		{
			get;
			set;
		}

		public long Version
		{
			get;
			set;
		}

		public HistoryEventDto()
		{
		}

		public static HistoryEventDto FromDomain(ParsedHistoryEvent historyEvent)
		{
			return SimpleMapper.Map<ParsedHistoryEvent, HistoryEventDto>(historyEvent, new HistoryEventDto()
			{
				EventId = historyEvent.get_Id()
			});
		}
	}
}