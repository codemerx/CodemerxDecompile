using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Squidex.Areas.Api.Controllers.History.Models;
using Squidex.Domain.Apps.Entities.History;
using Squidex.Infrastructure.Commands;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.History
{
	[ApiExplorerSettings(GroupName="History")]
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class HistoryController : ApiController
	{
		private readonly IHistoryService historyService;

		public HistoryController(ICommandBus commandBus, IHistoryService historyService) : base(commandBus)
		{
			this.historyService = historyService;
		}

		[ApiCosts(0.1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.history" })]
		[HttpGet]
		[ProducesResponseType(typeof(HistoryEventDto[]), 200)]
		[Route("apps/{app}/history/")]
		public async Task<IActionResult> GetAppHistory(string app, string channel)
		{
			IReadOnlyList<ParsedHistoryEvent> parsedHistoryEvents = await this.historyService.QueryByChannelAsync(base.get_AppId(), channel, 100, base.get_HttpContext().get_RequestAborted());
			IEnumerable<HistoryEventDto> historyEventDtos = parsedHistoryEvents.Select<ParsedHistoryEvent, HistoryEventDto>(new Func<ParsedHistoryEvent, HistoryEventDto>(HistoryEventDto.FromDomain));
			HistoryEventDto[] array = (
				from x in historyEventDtos
				where x.Message != null
				select x).ToArray<HistoryEventDto>();
			return this.Ok(array);
		}

		[ApiCosts(0.1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.teams.{team}.history" })]
		[HttpGet]
		[ProducesResponseType(typeof(HistoryEventDto[]), 200)]
		[Route("teams/{team}/history/")]
		public async Task<IActionResult> GetTeamHistory(string team, string channel)
		{
			IReadOnlyList<ParsedHistoryEvent> parsedHistoryEvents = await this.historyService.QueryByChannelAsync(base.get_TeamId(), channel, 100, base.get_HttpContext().get_RequestAborted());
			IEnumerable<HistoryEventDto> historyEventDtos = parsedHistoryEvents.Select<ParsedHistoryEvent, HistoryEventDto>(new Func<ParsedHistoryEvent, HistoryEventDto>(HistoryEventDto.FromDomain));
			HistoryEventDto[] array = (
				from x in historyEventDtos
				where x.Message != null
				select x).ToArray<HistoryEventDto>();
			return this.Ok(array);
		}
	}
}