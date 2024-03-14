using NodaTime;
using Squidex.Domain.Apps.Core.Comments;
using Squidex.Domain.Apps.Entities;
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
	public sealed class CommentDto
	{
		public DomainId Id
		{
			get;
			set;
		}

		[LocalizedRequired]
		public string Text
		{
			get;
			set;
		}

		[LocalizedRequired]
		public Instant Time
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

		[LocalizedRequired]
		public RefToken User
		{
			get;
			set;
		}

		public CommentDto()
		{
		}

		public static CommentDto FromDomain(Comment comment)
		{
			return SimpleMapper.Map<Comment, CommentDto>(comment, new CommentDto());
		}

		public static CommentDto FromDomain(CreateComment command)
		{
			Instant currentInstant = SystemClock.get_Instance().GetCurrentInstant();
			return SimpleMapper.Map<CreateComment, CommentDto>(command, new CommentDto()
			{
				Id = command.get_CommentId(),
				User = command.get_Actor(),
				Time = currentInstant
			});
		}
	}
}