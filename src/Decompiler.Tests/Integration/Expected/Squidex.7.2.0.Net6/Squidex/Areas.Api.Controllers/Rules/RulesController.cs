using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using NJsonSchema;
using NodaTime;
using NSwag.Annotations;
using Squidex.Areas.Api.Controllers.Rules.Models;
using Squidex.Domain.Apps.Core.HandleRules;
using Squidex.Domain.Apps.Core.Rules;
using Squidex.Domain.Apps.Core.Scripting;
using Squidex.Domain.Apps.Entities;
using Squidex.Domain.Apps.Entities.Apps;
using Squidex.Domain.Apps.Entities.Rules;
using Squidex.Domain.Apps.Entities.Rules.Commands;
using Squidex.Domain.Apps.Entities.Rules.Repositories;
using Squidex.Domain.Apps.Entities.Rules.Runner;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Commands;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.Rules
{
	[ApiExplorerSettings(GroupName="Rules")]
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class RulesController : ApiController
	{
		private readonly EventJsonSchemaGenerator eventJsonSchemaGenerator;

		private readonly IAppProvider appProvider;

		private readonly IRuleEventRepository ruleEventsRepository;

		private readonly IRuleQueryService ruleQuery;

		private readonly IRuleRunnerService ruleRunnerService;

		private readonly RuleTypeProvider ruleRegistry;

		public RulesController(ICommandBus commandBus, IAppProvider appProvider, IRuleEventRepository ruleEventsRepository, IRuleQueryService ruleQuery, IRuleRunnerService ruleRunnerService, RuleTypeProvider ruleRegistry, EventJsonSchemaGenerator eventJsonSchemaGenerator) : base(commandBus)
		{
			this.appProvider = appProvider;
			this.ruleEventsRepository = ruleEventsRepository;
			this.ruleQuery = ruleQuery;
			this.ruleRunnerService = ruleRunnerService;
			this.ruleRegistry = ruleRegistry;
			this.eventJsonSchemaGenerator = eventJsonSchemaGenerator;
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.rules.events.delete" })]
		[HttpDelete]
		[Route("apps/{app}/rules/events/{id}/")]
		public async Task<IActionResult> DeleteEvent(string app, DomainId id)
		{
			IActionResult actionResult;
			if (await this.ruleEventsRepository.FindAsync(id, base.get_HttpContext().get_RequestAborted()) != null)
			{
				await this.ruleEventsRepository.CancelByRuleAsync(id, base.get_HttpContext().get_RequestAborted());
				actionResult = this.NoContent();
			}
			else
			{
				actionResult = this.NotFound();
			}
			return actionResult;
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.rules.events.delete" })]
		[HttpDelete]
		[ProducesResponseType(204)]
		[Route("apps/{app}/rules/events/")]
		public async Task<IActionResult> DeleteEvents(string app)
		{
			await this.ruleEventsRepository.CancelByAppAsync(base.get_App().get_Id(), base.get_HttpContext().get_RequestAborted());
			return this.NoContent();
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.rules.delete" })]
		[HttpDelete]
		[Route("apps/{app}/rules/{id}/")]
		public async Task<IActionResult> DeleteRule(string app, DomainId id)
		{
			DeleteRule deleteRule = new DeleteRule();
			deleteRule.set_RuleId(id);
			DeleteRule deleteRule1 = deleteRule;
			await base.get_CommandBus().PublishAsync(deleteRule1, base.get_HttpContext().get_RequestAborted());
			return this.NoContent();
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.rules.events.delete" })]
		[HttpDelete]
		[ProducesResponseType(204)]
		[Route("apps/{app}/rules/{id}/events/")]
		public async Task<IActionResult> DeleteRuleEvents(string app, DomainId id)
		{
			await this.ruleEventsRepository.CancelByRuleAsync(id, base.get_HttpContext().get_RequestAborted());
			return this.NoContent();
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.rules.events.update" })]
		[HttpDelete]
		[ProducesResponseType(204)]
		[Route("apps/{app}/rules/run")]
		public async Task<IActionResult> DeleteRuleRun(string app)
		{
			await this.ruleRunnerService.CancelAsync(base.get_App().get_Id(), base.get_HttpContext().get_RequestAborted());
			return this.NoContent();
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.rules.disable" })]
		[HttpPut]
		[ProducesResponseType(typeof(RuleDto), 200)]
		[Route("apps/{app}/rules/{id}/disable/")]
		public async Task<IActionResult> DisableRule(string app, DomainId id)
		{
			DisableRule disableRule = new DisableRule();
			disableRule.set_RuleId(id);
			return this.Ok(await this.InvokeCommandAsync(disableRule));
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.rules.disable" })]
		[HttpPut]
		[ProducesResponseType(typeof(RuleDto), 200)]
		[Route("apps/{app}/rules/{id}/enable/")]
		public async Task<IActionResult> EnableRule(string app, DomainId id)
		{
			EnableRule enableRule = new EnableRule();
			enableRule.set_RuleId(id);
			return this.Ok(await this.InvokeCommandAsync(enableRule));
		}

		[ApiCosts(0)]
		[ApiPermission(new string[] {  })]
		[HttpGet]
		[ProducesResponseType(typeof(Dictionary<string, RuleElementDto>), 200)]
		[Route("rules/actions/")]
		public IActionResult GetActions()
		{
			string sha256Base64 = RandomHash.ToSha256Base64(string.Concat(
				from x in this.ruleRegistry.get_Actions()
				select x.Key));
			Deferred deferred = Deferred.Response(() => this.ruleRegistry.get_Actions().ToDictionary<KeyValuePair<string, RuleActionDefinition>, string, RuleElementDto>((KeyValuePair<string, RuleActionDefinition> x) => x.Key, (KeyValuePair<string, RuleActionDefinition> x) => RuleElementDto.FromDomain(x.Value)));
			base.get_Response().get_Headers().set_Item(HeaderNames.ETag, sha256Base64);
			return this.Ok(deferred);
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.rules.events.read" })]
		[HttpGet]
		[ProducesResponseType(typeof(RuleEventsDto), 200)]
		[Route("apps/{app}/rules/events/")]
		public async Task<IActionResult> GetEvents(string app, [FromQuery] DomainId? ruleId = null, [FromQuery] int skip = 0, [FromQuery] int take = 20)
		{
			IResultList<IRuleEventEntity> resultList = await this.ruleEventsRepository.QueryByAppAsync(base.get_AppId(), ruleId, skip, take, base.get_HttpContext().get_RequestAborted());
			RuleEventsDto ruleEventsDto = RuleEventsDto.FromDomain(resultList, base.get_Resources(), ruleId);
			return this.Ok(ruleEventsDto);
		}

		[AllowAnonymous]
		[HttpGet]
		[ProducesResponseType(typeof(object), 200)]
		[Route("rules/eventtypes/{type}")]
		public IActionResult GetEventSchema(string type)
		{
			JsonSchema schema = this.eventJsonSchemaGenerator.GetSchema(type);
			if (schema == null)
			{
				return this.NotFound();
			}
			return this.Content(schema.ToJson(), "application/json");
		}

		[AllowAnonymous]
		[HttpGet]
		[ProducesResponseType(typeof(string[]), 200)]
		[Route("rules/eventtypes")]
		public IActionResult GetEventTypes()
		{
			return this.Ok(this.eventJsonSchemaGenerator.get_AllTypes());
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.rules.read" })]
		[HttpGet]
		[ProducesResponseType(typeof(RulesDto), 200)]
		[Route("apps/{app}/rules/")]
		public async Task<IActionResult> GetRules(string app)
		{
			IReadOnlyList<IEnrichedRuleEntity> enrichedRuleEntities = await this.ruleQuery.QueryAsync(base.get_Context(), base.get_HttpContext().get_RequestAborted());
			IReadOnlyList<IEnrichedRuleEntity> enrichedRuleEntities1 = enrichedRuleEntities;
			Deferred deferred = Deferred.AsyncResponse<RulesDto>(() => RulesDto.FromRulesAsync(enrichedRuleEntities1, this.ruleRunnerService, base.get_Resources()));
			IActionResult actionResult = this.Ok(deferred);
			return actionResult;
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] {  })]
		[HttpGet]
		[OpenApiIgnore]
		[Route("apps/{app}/rules/completion/{triggerType}")]
		public IActionResult GetScriptCompletion(string app, string triggerType, [FromServices] ScriptingCompleter completer)
		{
			return this.Ok(completer.Trigger(triggerType));
		}

		private async Task<RuleDto> InvokeCommandAsync(ICommand command)
		{
			CommandContext commandContext = await base.get_CommandBus().PublishAsync(command, base.get_HttpContext().get_RequestAborted());
			DomainId? runningRuleIdAsync = await this.ruleRunnerService.GetRunningRuleIdAsync(base.get_Context().get_App().get_Id(), base.get_HttpContext().get_RequestAborted());
			IEnrichedRuleEntity enrichedRuleEntity = commandContext.Result<IEnrichedRuleEntity>();
			RuleDto ruleDto = RuleDto.FromDomain(enrichedRuleEntity, !runningRuleIdAsync.HasValue, this.ruleRunnerService, base.get_Resources());
			RuleDto ruleDto1 = ruleDto;
			commandContext = null;
			return ruleDto1;
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.rules.create" })]
		[HttpPost]
		[ProducesResponseType(typeof(RuleDto), 201)]
		[Route("apps/{app}/rules/")]
		public async Task<IActionResult> PostRule(string app, [FromBody] CreateRuleDto request)
		{
			RuleDto ruleDto = await this.InvokeCommandAsync(request.ToCommand());
			IActionResult actionResult = this.CreatedAtAction("GetRules", new { app = app }, ruleDto);
			return actionResult;
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.rules.events.update" })]
		[HttpPut]
		[Route("apps/{app}/rules/events/{id}/")]
		public async Task<IActionResult> PutEvent(string app, DomainId id)
		{
			IActionResult actionResult;
			if (await this.ruleEventsRepository.FindAsync(id, base.get_HttpContext().get_RequestAborted()) != null)
			{
				await this.ruleEventsRepository.EnqueueAsync(id, SystemClock.get_Instance().GetCurrentInstant(), base.get_HttpContext().get_RequestAborted());
				actionResult = this.NoContent();
			}
			else
			{
				actionResult = this.NotFound();
			}
			return actionResult;
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.rules.update" })]
		[HttpPut]
		[ProducesResponseType(typeof(RuleDto), 200)]
		[Route("apps/{app}/rules/{id}/")]
		public async Task<IActionResult> PutRule(string app, DomainId id, [FromBody] UpdateRuleDto request)
		{
			UpdateRule command = request.ToCommand(id);
			return this.Ok(await this.InvokeCommandAsync(command));
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.rules.events.run" })]
		[HttpPut]
		[ProducesResponseType(204)]
		[Route("apps/{app}/rules/{id}/run")]
		public async Task<IActionResult> PutRuleRun(string app, DomainId id, [FromQuery] bool fromSnapshots = false)
		{
			await this.ruleRunnerService.RunAsync(base.get_App().get_Id(), id, fromSnapshots, base.get_HttpContext().get_RequestAborted());
			return this.NoContent();
		}

		[ApiCosts(5)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.rules.events.read" })]
		[HttpPost]
		[ProducesResponseType(typeof(SimulatedRuleEventsDto), 200)]
		[Route("apps/{app}/rules/simulate/")]
		public async Task<IActionResult> Simulate(string app, [FromBody] CreateRuleDto request)
		{
			Rule rule = request.ToRule();
			List<SimulatedRuleEvent> simulatedRuleEvents = await this.ruleRunnerService.SimulateAsync(AppExtensions.NamedId(base.get_App()), DomainId.Empty, rule, base.get_HttpContext().get_RequestAborted());
			return this.Ok(SimulatedRuleEventsDto.FromDomain(simulatedRuleEvents));
		}

		[ApiCosts(5)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.rules.events.read" })]
		[HttpGet]
		[ProducesResponseType(typeof(SimulatedRuleEventsDto), 200)]
		[Route("apps/{app}/rules/{id}/simulate/")]
		public async Task<IActionResult> Simulate(string app, DomainId id)
		{
			IActionResult actionResult;
			IRuleEntity ruleAsync = await this.appProvider.GetRuleAsync(base.get_AppId(), id, base.get_HttpContext().get_RequestAborted());
			if (ruleAsync != null)
			{
				List<SimulatedRuleEvent> simulatedRuleEvents = await this.ruleRunnerService.SimulateAsync(ruleAsync, base.get_HttpContext().get_RequestAborted());
				actionResult = this.Ok(SimulatedRuleEventsDto.FromDomain(simulatedRuleEvents));
			}
			else
			{
				actionResult = this.NotFound();
			}
			return actionResult;
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.rules.events.run" })]
		[HttpPut]
		[Route("apps/{app}/rules/{id}/trigger/")]
		public async Task<IActionResult> TriggerRule(string app, DomainId id)
		{
			TriggerRule triggerRule = new TriggerRule();
			triggerRule.set_RuleId(id);
			TriggerRule triggerRule1 = triggerRule;
			await base.get_CommandBus().PublishAsync(triggerRule1, base.get_HttpContext().get_RequestAborted());
			return this.NoContent();
		}
	}
}