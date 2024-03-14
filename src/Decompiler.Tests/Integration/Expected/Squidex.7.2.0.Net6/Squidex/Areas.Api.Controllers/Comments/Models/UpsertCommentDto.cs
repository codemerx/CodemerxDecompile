using Squidex.Domain.Apps.Entities.Comments.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Comments.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class UpsertCommentDto
	{
		[LocalizedRequired]
		public string Text
		{
			get;
			set;
		}

		[Nullable(2)]
		public Uri Url
		{
			[NullableContext(2)]
			get;
			[NullableContext(2)]
			set;
		}

		public UpsertCommentDto()
		{
		}

		public CreateComment ToCreateCommand(DomainId commentsId)
		{
			CreateComment createComment = new CreateComment();
			createComment.set_CommentsId(commentsId);
			return SimpleMapper.Map<UpsertCommentDto, CreateComment>(this, createComment);
		}

		public UpdateComment ToUpdateComment(DomainId commentsId, DomainId commentId)
		{
			UpdateComment updateComment = new UpdateComment();
			updateComment.set_CommentsId(commentsId);
			updateComment.set_CommentId(commentId);
			return SimpleMapper.Map<UpsertCommentDto, UpdateComment>(this, updateComment);
		}
	}
}