using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Squidex.Areas.Api.Controllers.EventConsumers.Models;
using Squidex.Infrastructure.Commands;
using Squidex.Infrastructure.EventSourcing;
using Squidex.Infrastructure.EventSourcing.Consume;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.EventConsumers
{
	[ApiExplorerSettings(GroupName="EventConsumers")]
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class EventConsumersController : ApiController
	{
		private readonly IEventConsumerManager eventConsumerManager;

		public EventConsumersController(ICommandBus commandBus, IEventConsumerManager eventConsumerManager) : base(commandBus)
		{
			this.eventConsumerManager = eventConsumerManager;
		}

		[ApiPermission(new string[] { "squidex.admin.events.read" })]
		[HttpGet]
		[ProducesResponseType(typeof(EventConsumersDto), 200)]
		[Route("event-consumers/")]
		public async Task<IActionResult> GetEventConsumers()
		{
			List<EventConsumerInfo> consumersAsync = await this.eventConsumerManager.GetConsumersAsync(base.get_HttpContext().get_RequestAborted());
			EventConsumersDto eventConsumersDto = EventConsumersDto.FromDomain(consumersAsync, base.get_Resources());
			return this.Ok(eventConsumersDto);
		}

		[ApiPermission(new string[] { "squidex.admin.events.manage" })]
		[HttpPut]
		[ProducesResponseType(typeof(EventConsumerDto), 200)]
		[Route("event-consumers/{consumerName}/reset/")]
		public async Task<IActionResult> ResetEventConsumer(string consumerName)
		{
			EventConsumerInfo eventConsumerInfo = await this.eventConsumerManager.ResetAsync(consumerName, base.get_HttpContext().get_RequestAborted());
			EventConsumerDto eventConsumerDto = EventConsumerDto.FromDomain(eventConsumerInfo, base.get_Resources());
			return this.Ok(eventConsumerDto);
		}

		[ApiPermission(new string[] { "squidex.admin.events.manage" })]
		[HttpPut]
		[ProducesResponseType(typeof(EventConsumerDto), 200)]
		[Route("event-consumers/{consumerName}/start/")]
		public async Task<IActionResult> StartEventConsumer(string consumerName)
		{
			EventConsumerInfo eventConsumerInfo = await this.eventConsumerManager.StartAsync(consumerName, base.get_HttpContext().get_RequestAborted());
			EventConsumerDto eventConsumerDto = EventConsumerDto.FromDomain(eventConsumerInfo, base.get_Resources());
			return this.Ok(eventConsumerDto);
		}

		[ApiPermission(new string[] { "squidex.admin.events.manage" })]
		[HttpPut]
		[ProducesResponseType(typeof(EventConsumerDto), 200)]
		[Route("event-consumers/{consumerName}/stop/")]
		public async Task<IActionResult> StopEventConsumer(string consumerName)
		{
			EventConsumerInfo eventConsumerInfo = await this.eventConsumerManager.StopAsync(consumerName, base.get_HttpContext().get_RequestAborted());
			EventConsumerDto eventConsumerDto = EventConsumerDto.FromDomain(eventConsumerInfo, base.get_Resources());
			return this.Ok(eventConsumerDto);
		}
	}
}