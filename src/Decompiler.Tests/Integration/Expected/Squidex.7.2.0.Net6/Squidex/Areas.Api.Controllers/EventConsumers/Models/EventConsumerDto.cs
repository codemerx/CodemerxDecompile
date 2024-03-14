using Squidex.Areas.Api.Controllers.EventConsumers;
using Squidex.Infrastructure.EventSourcing;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using Squidex.Web;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.EventConsumers.Models
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class EventConsumerDto : Resource
	{
		public int Count
		{
			get;
			set;
		}

		public string Error
		{
			get;
			set;
		}

		public bool IsResetting
		{
			get;
			set;
		}

		public bool IsStopped
		{
			get;
			set;
		}

		[LocalizedRequired]
		[Nullable(1)]
		public string Name
		{
			[NullableContext(1)]
			get;
			[NullableContext(1)]
			set;
		}

		public string Position
		{
			get;
			set;
		}

		public EventConsumerDto()
		{
		}

		[NullableContext(1)]
		private EventConsumerDto CreateLinks(Resources resources)
		{
			if (resources.get_CanManageEvents())
			{
				var variable = new { consumerName = this.Name };
				if (!this.IsResetting)
				{
					base.AddPutLink("reset", resources.Url<EventConsumersController>((EventConsumersController x) => "ResetEventConsumer", variable), null);
				}
				if (!this.IsStopped)
				{
					base.AddPutLink("stop", resources.Url<EventConsumersController>((EventConsumersController x) => "StopEventConsumer", variable), null);
				}
				else
				{
					base.AddPutLink("start", resources.Url<EventConsumersController>((EventConsumersController x) => "StartEventConsumer", variable), null);
				}
			}
			return this;
		}

		[NullableContext(1)]
		public static EventConsumerDto FromDomain(EventConsumerInfo eventConsumerInfo, Resources resources)
		{
			return SimpleMapper.Map<EventConsumerInfo, EventConsumerDto>(eventConsumerInfo, new EventConsumerDto()).CreateLinks(resources);
		}
	}
}