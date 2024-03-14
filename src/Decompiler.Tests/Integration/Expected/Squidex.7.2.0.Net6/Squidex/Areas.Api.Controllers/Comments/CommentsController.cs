using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Squidex.Areas.Api.Controllers.Comments.Models;
using Squidex.Domain.Apps.Entities.Comments;
using Squidex.Domain.Apps.Entities.Comments.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Commands;
using Squidex.Web;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.Comments
{
	[ApiExplorerSettings(GroupName="Comments")]
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class CommentsController : ApiController
	{
		private readonly ICommentsLoader commentsLoader;

		private readonly IWatchingService watchingService;

		public CommentsController(ICommandBus commandBus, ICommentsLoader commentsLoader, IWatchingService watchingService) : base(commandBus)
		{
			this.commentsLoader = commentsLoader;
			this.watchingService = watchingService;
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.comments.delete" })]
		[HttpDelete]
		[Route("apps/{app}/comments/{commentsId}/{commentId}")]
		public async Task<IActionResult> DeleteComment(string app, DomainId commentsId, DomainId commentId)
		{
			DeleteComment deleteComment = new DeleteComment();
			deleteComment.set_CommentsId(commentsId);
			deleteComment.set_CommentId(commentId);
			DeleteComment deleteComment1 = deleteComment;
			await base.get_CommandBus().PublishAsync(deleteComment1, base.get_HttpContext().get_RequestAborted());
			return this.NoContent();
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.comments.read" })]
		[HttpGet]
		[ProducesResponseType(typeof(CommentsDto), 200)]
		[Route("apps/{app}/comments/{commentsId}")]
		public async Task<IActionResult> GetComments(string app, DomainId commentsId, [FromQuery] long version = -2L)
		{
			CommentsResult commentsAsync = await this.commentsLoader.GetCommentsAsync(this.Id(commentsId), version, base.get_HttpContext().get_RequestAborted());
			CommentsResult commentsResult = commentsAsync;
			Deferred deferred = Deferred.Response(() => CommentsDto.FromDomain(commentsResult));
			IHeaderDictionary headers = base.get_Response().get_Headers();
			headers.set_Item(HeaderNames.ETag, commentsResult.get_Version().ToString(CultureInfo.InvariantCulture));
			IActionResult actionResult = this.Ok(deferred);
			return actionResult;
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] {  })]
		[HttpGet]
		[ProducesResponseType(typeof(string[]), 200)]
		[Route("apps/{app}/watching/{*resource}")]
		public async Task<IActionResult> GetWatchingUsers(string app, [Nullable(2)] string resource = null)
		{
			IActionResult actionResult = this.Ok(await this.watchingService.GetWatchingUsersAsync(base.get_App().get_Id(), resource, base.get_UserId(), base.get_HttpContext().get_RequestAborted()));
			return actionResult;
		}

		private DomainId Id(DomainId commentsId)
		{
			return DomainId.Combine(base.get_App().get_Id(), commentsId);
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.comments.create" })]
		[HttpPost]
		[ProducesResponseType(typeof(CommentDto), 201)]
		[Route("apps/{app}/comments/{commentsId}")]
		public async Task<IActionResult> PostComment(string app, DomainId commentsId, [FromBody] UpsertCommentDto request)
		{
			CreateComment createCommand = request.ToCreateCommand(commentsId);
			await base.get_CommandBus().PublishAsync(createCommand, base.get_HttpContext().get_RequestAborted());
			CommentDto commentDto = CommentDto.FromDomain(createCommand);
			IActionResult actionResult = this.CreatedAtAction("GetComments", new { app = app, commentsId = commentsId }, commentDto);
			createCommand = null;
			return actionResult;
		}

		[ApiCosts(0)]
		[ApiPermissionOrAnonymous(new string[] { "squidex.apps.{app}.comments.update" })]
		[HttpPut]
		[Route("apps/{app}/comments/{commentsId}/{commentId}")]
		public async Task<IActionResult> PutComment(string app, DomainId commentsId, DomainId commentId, [FromBody] UpsertCommentDto request)
		{
			UpdateComment updateComment = request.ToUpdateComment(commentsId, commentId);
			await base.get_CommandBus().PublishAsync(updateComment, base.get_HttpContext().get_RequestAborted());
			return this.NoContent();
		}
	}
}