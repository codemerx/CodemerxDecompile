using Squidex.Areas.Api.Controllers.EventConsumers;
using Squidex.Infrastructure.EventSourcing;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.EventConsumers.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class EventConsumersDto : Resource
	{
		public EventConsumerDto[] Items
		{
			get;
			set;
		}

		public EventConsumersDto()
		{
		}

		private EventConsumersDto CreateLinks(Resources resources)
		{
			base.AddSelfLink(resources.Url<EventConsumersController>((EventConsumersController c) => "GetEventConsumers", null));
			return this;
		}

		public static EventConsumersDto FromDomain(IEnumerable<EventConsumerInfo> items, Resources resources)
		{
			return (new EventConsumersDto()
			{
				Items = (
					from x in items
					select EventConsumerDto.FromDomain(x, resources)).ToArray<EventConsumerDto>()
			}).CreateLinks(resources);
		}
	}
}