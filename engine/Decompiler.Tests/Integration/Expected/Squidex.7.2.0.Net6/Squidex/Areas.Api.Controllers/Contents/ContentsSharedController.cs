using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Squidex.Areas.Api.Controllers;
using Squidex.Areas.Api.Controllers.Contents.Models;
using Squidex.Domain.Apps.Entities;
using Squidex.Domain.Apps.Entities.Contents;
using Squidex.Domain.Apps.Entities.Contents.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Commands;
using Squidex.Web;
using Squidex.Web.GraphQL;
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
	public sealed class ContentsSharedController : ApiController
	{
		private readonly IContentQueryService contentQuery;

		private readonly IContentWorkflow contentWorkflow;

		private readonly GraphQLRunner graphQLRunner;

		public ContentsSharedController(ICommandBus commandBus, IContentQueryService contentQuery, IContentWorkflow contentWorkflow, GraphQLRunner graphQLRunner) : base(commandBus)
		{
			this.contentQuery = contentQuery;
			this.contentWorkflow = contentWorkflow;
			this.graphQLRunner = graphQLRunner;
		}

		[ApiCosts(5)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.contents.{schema}.read.own" })]
		[HttpPost]
		[ProducesResponseType(typeof(BulkResultDto[]), 200)]
		[Route("content/{app}/bulk")]
		public async Task<IActionResult> BulkUpdateContents(string app, string schema, [FromBody] BulkUpdateContentsDto request)
		{
			BulkUpdateContents command = request.ToCommand(true);
			BulkUpdateResult bulkUpdateResult = await base.get_CommandBus().PublishAsync(command, base.get_HttpContext().get_RequestAborted()).Result<BulkUpdateResult>();
			BulkResultDto[] array = (
				from x in bulkUpdateResult
				select BulkResultDto.FromDomain(x, base.get_HttpContext())).ToArray<BulkResultDto>();
			return this.Ok(array);
		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] {  })]
		[HttpGet]
		[ProducesResponseType(typeof(ContentsDto), 200)]
		[Route("content/{app}/")]
		public async Task<IActionResult> GetAllContents(string app, AllContentsByGetDto query)
		{
			// 
			// Current member / type: System.Threading.Tasks.Task`1<Microsoft.AspNetCore.Mvc.IActionResult> Squidex.Areas.Api.Controllers.Contents.ContentsSharedController::GetAllContents(System.String,Squidex.Areas.Api.Controllers.Contents.Models.AllContentsByGetDto)
			// Exception in: System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetAllContents(System.String,Squidex.Areas.Api.Controllers.Contents.Models.AllContentsByGetDto)
			// Object reference not set to an instance of an object.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		[ApiCosts(1)]
		[ApiPermissionOrAnonymous(new string[] {  })]
		[HttpPost]
		[ProducesResponseType(typeof(ContentsDto), 200)]
		[Route("content/{app}/")]
		public async Task<IActionResult> GetAllContentsPost(string app, [FromBody] AllContentsByPostDto query)
		{
			// 
			// Current member / type: System.Threading.Tasks.Task`1<Microsoft.AspNetCore.Mvc.IActionResult> Squidex.Areas.Api.Controllers.Contents.ContentsSharedController::GetAllContentsPost(System.String,Squidex.Areas.Api.Controllers.Contents.Models.AllContentsByPostDto)
			// Exception in: System.Threading.Tasks.Task<Microsoft.AspNetCore.Mvc.IActionResult> GetAllContentsPost(System.String,Squidex.Areas.Api.Controllers.Contents.Models.AllContentsByPostDto)
			// Object reference not set to an instance of an object.
			// 
			// mailto: JustDecompilePublicFeedback@telerik.com

		}

		[ApiCosts(2)]
		[ApiPermissionOrAnonymous(new string[] {  })]
		[HttpGet]
		[HttpPost]
		[Route("content/{app}/graphql/")]
		[Route("content/{app}/graphql/batch")]
		public Task GetGraphQL(string app)
		{
			return this.graphQLRunner.InvokeAsync(base.get_HttpContext());
		}
	}
}