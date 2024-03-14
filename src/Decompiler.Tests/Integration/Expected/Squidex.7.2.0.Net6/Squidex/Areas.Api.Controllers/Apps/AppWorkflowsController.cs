using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Squidex.Areas.Api.Controllers.Apps.Models;
using Squidex.Domain.Apps.Entities.Apps;
using Squidex.Domain.Apps.Entities.Apps.Commands;
using Squidex.Domain.Apps.Entities.Contents;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Commands;
using Squidex.Web;
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.Apps
{
	[ApiExplorerSettings(GroupName="Apps")]
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class AppWorkflowsController : ApiController
	{
		private readonly IWorkflowsValidator workflowsValidator;

		public AppWorkflowsController(ICommandBus commandBus, IWorkflowsValidator workflowsValidator) : base(commandBus)
		{
			this.workflowsValidator = workflowsValidator;
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.workflows.update" })]
		[HttpDelete]
		[ProducesResponseType(typeof(WorkflowsDto), 200)]
		[Route("apps/{app}/workflows/{id}")]
		public async Task<IActionResult> DeleteWorkflow(string app, DomainId id)
		{
			DeleteWorkflow deleteWorkflow = new DeleteWorkflow();
			deleteWorkflow.set_WorkflowId(id);
			return this.Ok(await this.InvokeCommandAsync(deleteWorkflow));
		}

		private async Task<WorkflowsDto> GetResponse(IAppEntity result)
		{
			WorkflowsDto workflowsDto = await WorkflowsDto.FromAppAsync(this.workflowsValidator, result, base.get_Resources());
			return workflowsDto;
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.workflows.read" })]
		[HttpGet]
		[ProducesResponseType(typeof(WorkflowsDto), 200)]
		[Route("apps/{app}/workflows/")]
		public IActionResult GetWorkflows(string app)
		{
			Deferred deferred = Deferred.AsyncResponse<WorkflowsDto>(() => this.GetResponse(base.get_App()));
			base.get_Response().get_Headers().set_Item(HeaderNames.ETag, ETagExtensions.ToEtag<IAppEntity>(base.get_App()));
			return this.Ok(deferred);
		}

		private async Task<WorkflowsDto> InvokeCommandAsync(ICommand command)
		{
			CommandContext commandContext = await base.get_CommandBus().PublishAsync(command, base.get_HttpContext().get_RequestAborted());
			return await this.GetResponse(commandContext.Result<IAppEntity>());
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.workflows.update" })]
		[HttpPost]
		[ProducesResponseType(typeof(WorkflowsDto), 200)]
		[Route("apps/{app}/workflows/")]
		public async Task<IActionResult> PostWorkflow(string app, [FromBody] AddWorkflowDto request)
		{
			AddWorkflow command = request.ToCommand();
			return this.Ok(await this.InvokeCommandAsync(command));
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.workflows.update" })]
		[HttpPut]
		[ProducesResponseType(typeof(WorkflowsDto), 200)]
		[Route("apps/{app}/workflows/{id}")]
		public async Task<IActionResult> PutWorkflow(string app, DomainId id, [FromBody] UpdateWorkflowDto request)
		{
			UpdateWorkflow command = request.ToCommand(id);
			return this.Ok(await this.InvokeCommandAsync(command));
		}
	}
}