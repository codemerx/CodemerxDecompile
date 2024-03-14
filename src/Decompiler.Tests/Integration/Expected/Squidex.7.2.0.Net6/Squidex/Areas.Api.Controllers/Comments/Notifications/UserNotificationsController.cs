using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Squidex.Areas.Api.Controllers.Comments.Models;
using Squidex.Domain.Apps.Entities.Comments;
using Squidex.Domain.Apps.Entities.Comments.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Commands;
using Squidex.Infrastructure.Security;
using Squidex.Infrastructure.Translations;
using Squidex.Web;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Squidex.Areas.Api.Controllers.Comments.Notifications
{
	[ApiExplorerSettings(GroupName="Notifications")]
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class UserNotificationsController : ApiController
	{
		private readonly ICommentsLoader commentsLoader;

		public UserNotificationsController(ICommandBus commandBus, ICommentsLoader commentsLoader) : base(commandBus)
		{
			this.commentsLoader = commentsLoader;
		}

		private void CheckPermissions(DomainId userId)
		{
			if (!string.Equals(userId.ToString(), Squidex.Infrastructure.Security.Extensions.OpenIdSubject(base.get_User()), StringComparison.Ordinal))
			{
				throw new DomainForbiddenException(T.Get("comments.noPermissions", null), null);
			}
		}

		[ApiPermission(new string[] {  })]
		[HttpDelete]
		[Route("users/{userId}/notifications/{commentId}")]
		public async Task<IActionResult> DeleteComment(DomainId userId, DomainId commentId)
		{
			this.CheckPermissions(userId);
			DeleteComment deleteComment = new DeleteComment();
			deleteComment.set_AppId(CommentsCommand.NoApp);
			deleteComment.set_CommentsId(userId);
			deleteComment.set_CommentId(commentId);
			DeleteComment deleteComment1 = deleteComment;
			await base.get_CommandBus().PublishAsync(deleteComment1, base.get_HttpContext().get_RequestAborted());
			return this.NoContent();
		}

		[ApiPermission(new string[] {  })]
		[HttpGet]
		[ProducesResponseType(typeof(CommentsDto), 200)]
		[Route("users/{userId}/notifications")]
		public async Task<IActionResult> GetNotifications(DomainId userId, [FromQuery] long version = -2L)
		{
			this.CheckPermissions(userId);
			CommentsResult commentsAsync = await this.commentsLoader.GetCommentsAsync(userId, version, base.get_HttpContext().get_RequestAborted());
			CommentsResult commentsResult = commentsAsync;
			Deferred deferred = Deferred.Response(() => CommentsDto.FromDomain(commentsResult));
			IHeaderDictionary headers = base.get_Response().get_Headers();
			headers.set_Item(HeaderNames.ETag, commentsResult.get_Version().ToString(CultureInfo.InvariantCulture));
			IActionResult actionResult = this.Ok(deferred);
			return actionResult;
		}
	}
}