using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Squidex.Areas.Api.Controllers;
using Squidex.Areas.Api.Controllers.Contents.Models;
using Squidex.Domain.Apps.Core.Contents;
using Squidex.Domain.Apps.Entities;
using Squidex.Domain.Apps.Entities.Contents;
using Squidex.Domain.Apps.Entities.Contents.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Commands;
using Squidex.Web;
using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.Contents
{
	[Nullable(0)]
	[NullableContext(1)]
	[SchemaMustBePublished]
	public sealed class ContentsController : ApiController
	{
		private readonly IContentQueryService contentQuery;

		private readonly IContentWorkflow contentWorkflow;

		public ContentsController(ICommandBus commandBus, IContentQueryService contentQuery, IContentWorkflow contentWorkflow) : base(commandBus)
		{
			this.contentQuery = contentQuery;
			this.contentWorkflow = contentWorkflow;
		}

		[ApiCosts(5)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.contents.{schema}.read.own" })]
		[HttpPost]
		[ProducesResponseType(typeof(BulkResultDto[]), 200)]
		[Route("content/{app}/{schema}/bulk")]
		public async Task<IActionResult> BulkUpdateContents(string app, string schema, [FromBody] BulkUpdateContentsDto request)
		{
			BulkUpdateContents command = request.ToCommand(false);
			BulkUpdateResult bulkUpdateResult = await base.get_CommandBus().PublishAsync(command, base.get_HttpContext().get_RequestAborted()).Result<BulkUpdateResult>();
			BulkResultDto[] array = (
				from x in bulkUpdateResult
				select BulkResultDto.FromDomain(x, base.get_HttpContext())).ToArray<BulkResultDto>();
			return this.Ok(array);
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.contents.{schema}.version.create.own" })]
		[HttpPost]
		[ProducesResponseType(typeof(ContentsDto), 200)]
		[Route("content/{app}/{schema}/{id}/draft/")]
		public async Task<IActionResult> CreateDraft(string app, string schema, DomainId id)
		{
			CreateContentDraft createContentDraft = new CreateContentDraft();
			createContentDraft.set_ContentId(id);
			return this.Ok(await this.InvokeCommandAsync(createContentDraft));
		}

		[NullableContext(2)]
		[return: Nullable(1)]
		private Q CreateQuery(string ids, string q)
		{
			Q q1 = Q.get_Empty().WithIds(ids).WithJsonQuery(q);
			QueryString queryString = base.get_Request().get_QueryString();
			return q1.WithODataQuery(queryString.ToString());
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.contents.{schema}.delete.own" })]
		[HttpDelete]
		[Route("content/{app}/{schema}/{id}/")]
		public async Task<IActionResult> DeleteContent(string app, string schema, DomainId id, DeleteContentDto request)
		{
			DeleteContent command = request.ToCommand(id);
			await base.get_CommandBus().PublishAsync(command, base.get_HttpContext().get_RequestAborted());
			return this.NoContent();
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.contents.{schema}.changestatus.own" })]
		[HttpDelete]
		[ProducesResponseType(typeof(ContentsDto), 200)]
		[Route("content/{app}/{schema}/{id}/status/")]
		public async Task<IActionResult> DeleteContentStatus(string app, string schema, DomainId id)
		{
			CancelContentSchedule cancelContentSchedule = new CancelContentSchedule();
			cancelContentSchedule.set_ContentId(id);
			return this.Ok(await this.InvokeCommandAsync(cancelContentSchedule));
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.contents.{schema}.version.delete.own" })]
		[HttpDelete]
		[ProducesResponseType(typeof(ContentsDto), 200)]
		[Route("content/{app}/{schema}/{id}/draft/")]
		public async Task<IActionResult> DeleteVersion(string app, string schema, DomainId id)
		{
			DeleteContentDraft deleteContentDraft = new DeleteContentDraft();
			deleteContentDraft.set_ContentId(id);
			return this.Ok(await this.InvokeCommandAsync(deleteContentDraft));
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] {  })]
		[HttpGet]
		[ProducesResponseType(typeof(ContentDto), 200)]
		[Route("content/{app}/{schema}/{id}/")]
		public async Task<IActionResult> GetContent(string app, string schema, DomainId id, long version = -2L)
		{
			IActionResult actionResult;
			IEnrichedContentEntity enrichedContentEntity = await this.contentQuery.FindAsync(base.get_Context(), schema, id, version, base.get_HttpContext().get_RequestAborted());
			IEnrichedContentEntity enrichedContentEntity1 = enrichedContentEntity;
			if (enrichedContentEntity1 != null)
			{
				Deferred deferred = Deferred.Response(() => ContentDto.FromDomain(enrichedContentEntity1, base.get_Resources()));
				actionResult = this.Ok(deferred);
			}
			else
			{
				actionResult = this.NotFound();
			}
			return actionResult;
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] {  })]
		[HttpGet]
		[ProducesResponseType(typeof(ContentsDto), 200)]
		[Route("content/{app}/{schema}/")]
		public async Task<IActionResult> GetContents(string app, string schema, [Nullable(2)][FromQuery] string ids = null, [Nullable(2)][FromQuery] string q = null)
		{
			IResultList<IEnrichedContentEntity> resultList = await this.contentQuery.QueryAsync(base.get_Context(), schema, this.CreateQuery(ids, q), base.get_HttpContext().get_RequestAborted());
			IResultList<IEnrichedContentEntity> resultList1 = resultList;
			Deferred deferred = Deferred.AsyncResponse<ContentsDto>(() => ContentsDto.FromContentsAsync(resultList1, base.get_Resources(), base.get_Schema(), this.contentWorkflow));
			IActionResult actionResult = this.Ok(deferred);
			return actionResult;
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] {  })]
		[HttpPost]
		[ProducesResponseType(typeof(ContentsDto), 200)]
		[Route("content/{app}/{schema}/query")]
		public async Task<IActionResult> GetContentsPost(string app, string schema, [FromBody] QueryDto query)
		{
			// 
			// Current member / type: System.Threading.Tasks.Task`1<Microsoft.AspNetCore.Mvc.IActionResult> Squidex.Areas.Api.Controllers.Contents.ContentsController::GetContentsPost(System.String,System.String,Squidex.Areas.Api.Controllers.QueryDto)
			// Exception in: System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetContentsPost(System.String,System.String,Squidex.Areas.Api.Controllers.QueryDto)
			// Object reference not set to an instance of an object.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] {  })]
		[HttpGet]
		[Route("content/{app}/{schema}/{id}/validity")]
		public async Task<IActionResult> GetContentValidity(string app, string schema, DomainId id)
		{
			ValidateContent validateContent = new ValidateContent();
			validateContent.set_ContentId(id);
			ValidateContent validateContent1 = validateContent;
			await base.get_CommandBus().PublishAsync(validateContent1, base.get_HttpContext().get_RequestAborted());
			return this.NoContent();
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.contents.{schema}.read.own" })]
		[HttpGet]
		[Obsolete("Use ID endpoint with version query.")]
		[Route("content/{app}/{schema}/{id}/{version}/")]
		public async Task<IActionResult> GetContentVersion(string app, string schema, DomainId id, int version)
		{
			IActionResult actionResult;
			IEnrichedContentEntity enrichedContentEntity = await this.contentQuery.FindAsync(base.get_Context(), schema, id, (long)version, base.get_HttpContext().get_RequestAborted());
			IEnrichedContentEntity enrichedContentEntity1 = enrichedContentEntity;
			if (enrichedContentEntity1 != null)
			{
				Deferred deferred = Deferred.Response(() => ContentDto.FromDomain(enrichedContentEntity1, base.get_Resources()).Data);
				actionResult = this.Ok(deferred);
			}
			else
			{
				actionResult = this.NotFound();
			}
			return actionResult;
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] {  })]
		[HttpGet]
		[ProducesResponseType(typeof(ContentsDto), 200)]
		[Route("content/{app}/{schema}/{id}/references")]
		public async Task<IActionResult> GetReferences(string app, string schema, DomainId id, [Nullable(2)][FromQuery] string q = null)
		{
			IResultList<IEnrichedContentEntity> resultList = await this.contentQuery.QueryAsync(base.get_Context(), this.CreateQuery(null, q).WithReferencing(id), base.get_HttpContext().get_RequestAborted());
			IResultList<IEnrichedContentEntity> resultList1 = resultList;
			Deferred deferred = Deferred.AsyncResponse<ContentsDto>(() => ContentsDto.FromContentsAsync(resultList1, base.get_Resources(), null, this.contentWorkflow));
			IActionResult actionResult = this.Ok(deferred);
			return actionResult;
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] {  })]
		[HttpGet]
		[ProducesResponseType(typeof(ContentsDto), 200)]
		[Route("content/{app}/{schema}/{id}/referencing")]
		public async Task<IActionResult> GetReferencing(string app, string schema, DomainId id, [Nullable(2)][FromQuery] string q = null)
		{
			IResultList<IEnrichedContentEntity> resultList = await this.contentQuery.QueryAsync(base.get_Context(), this.CreateQuery(null, q).WithReference(id), base.get_HttpContext().get_RequestAborted());
			IResultList<IEnrichedContentEntity> resultList1 = resultList;
			Deferred deferred = Deferred.AsyncResponse<ContentsDto>(() => ContentsDto.FromContentsAsync(resultList1, base.get_Resources(), null, this.contentWorkflow));
			IActionResult actionResult = this.Ok(deferred);
			return actionResult;
		}

		private async Task<ContentDto> InvokeCommandAsync(ICommand command)
		{
			IEnrichedContentEntity enrichedContentEntity = await base.get_CommandBus().PublishAsync(command, base.get_HttpContext().get_RequestAborted()).Result<IEnrichedContentEntity>();
			return ContentDto.FromDomain(enrichedContentEntity, base.get_Resources());
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.contents.{schema}.update.own" })]
		[HttpPatch]
		[ProducesResponseType(typeof(ContentsDto), 200)]
		[Route("content/{app}/{schema}/{id}/")]
		public async Task<IActionResult> PatchContent(string app, string schema, DomainId id, [FromBody] ContentData request)
		{
			PatchContent patchContent = new PatchContent();
			patchContent.set_ContentId(id);
			patchContent.set_Data(request);
			return this.Ok(await this.InvokeCommandAsync(patchContent));
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.contents.{schema}.create" })]
		[HttpPost]
		[ProducesResponseType(typeof(ContentsDto), 201)]
		[Route("content/{app}/{schema}/")]
		public async Task<IActionResult> PostContent(string app, string schema, CreateContentDto request)
		{
			CreateContent command = request.ToCommand();
			ContentDto contentDto = await this.InvokeCommandAsync(command);
			IActionResult actionResult = this.CreatedAtAction("GetContent", new { app = app, schema = schema, id = command.get_ContentId() }, contentDto);
			command = null;
			return actionResult;
		}

		[ApiCosts(5)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.contents.{schema}.create" })]
		[HttpPost]
		[Obsolete("Use bulk endpoint now.")]
		[ProducesResponseType(typeof(BulkResultDto[]), 200)]
		[Route("content/{app}/{schema}/import")]
		public async Task<IActionResult> PostContents(string app, string schema, [FromBody] ImportContentsDto request)
		{
			BulkUpdateContents command = request.ToCommand();
			BulkUpdateResult bulkUpdateResult = await base.get_CommandBus().PublishAsync(command, base.get_HttpContext().get_RequestAborted()).Result<BulkUpdateResult>();
			BulkResultDto[] array = (
				from x in bulkUpdateResult
				select BulkResultDto.FromDomain(x, base.get_HttpContext())).ToArray<BulkResultDto>();
			return this.Ok(array);
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.contents.{schema}.upsert" })]
		[HttpPost]
		[ProducesResponseType(typeof(ContentsDto), 200)]
		[Route("content/{app}/{schema}/{id}/")]
		public async Task<IActionResult> PostUpsertContent(string app, string schema, DomainId id, UpsertContentDto request)
		{
			UpsertContent command = request.ToCommand(id);
			return this.Ok(await this.InvokeCommandAsync(command));
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.contents.{schema}.update.own" })]
		[HttpPut]
		[ProducesResponseType(typeof(ContentsDto), 200)]
		[Route("content/{app}/{schema}/{id}/")]
		public async Task<IActionResult> PutContent(string app, string schema, DomainId id, [FromBody] ContentData request)
		{
			UpdateContent updateContent = new UpdateContent();
			updateContent.set_ContentId(id);
			updateContent.set_Data(request);
			return this.Ok(await this.InvokeCommandAsync(updateContent));
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.contents.{schema}.changestatus.own" })]
		[HttpPut]
		[ProducesResponseType(typeof(ContentsDto), 200)]
		[Route("content/{app}/{schema}/{id}/status/")]
		public async Task<IActionResult> PutContentStatus(string app, string schema, DomainId id, [FromBody] ChangeStatusDto request)
		{
			ChangeContentStatus command = request.ToCommand(id);
			return this.Ok(await this.InvokeCommandAsync(command));
		}
	}
}