using Squidex.Domain.Apps.Core.Comments;
using Squidex.Domain.Apps.Entities.Comments;
using Squidex.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Comments.Models
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class CommentsDto
	{
		[Nullable(new byte[] { 2, 1 })]
		public CommentDto[] CreatedComments
		{
			[return: Nullable(new byte[] { 2, 1 })]
			get;
			set;
		}

		public List<DomainId> DeletedComments
		{
			get;
			set;
		}

		[Nullable(new byte[] { 2, 1 })]
		public CommentDto[] UpdatedComments
		{
			[return: Nullable(new byte[] { 2, 1 })]
			get;
			set;
		}

		public long Version
		{
			get;
			set;
		}

		public CommentsDto()
		{
		}

		[NullableContext(1)]
		public static CommentsDto FromDomain(CommentsResult comments)
		{
			CommentsDto commentsDto = new CommentsDto()
			{
				CreatedComments = comments.get_CreatedComments().Select<Comment, CommentDto>(new Func<Comment, CommentDto>(CommentDto.FromDomain)).ToArray<CommentDto>(),
				UpdatedComments = comments.get_UpdatedComments().Select<Comment, CommentDto>(new Func<Comment, CommentDto>(CommentDto.FromDomain)).ToArray<CommentDto>(),
				DeletedComments = comments.get_DeletedComments(),
				Version = comments.get_Version()
			};
			return commentsDto;
		}
	}
}