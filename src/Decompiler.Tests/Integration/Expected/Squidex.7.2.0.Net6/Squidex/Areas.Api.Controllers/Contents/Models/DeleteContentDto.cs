using Microsoft.AspNetCore.Mvc;
using Squidex.Domain.Apps.Entities.Contents.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Reflection;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Contents.Models
{
	public sealed class DeleteContentDto
	{
		[FromQuery]
		public bool CheckReferrers
		{
			get;
			set;
		}

		[FromQuery]
		public bool Permanent
		{
			get;
			set;
		}

		public DeleteContentDto()
		{
		}

		[NullableContext(1)]
		public DeleteContent ToCommand(DomainId id)
		{
			DeleteContent deleteContent = new DeleteContent();
			deleteContent.set_ContentId(id);
			return SimpleMapper.Map<DeleteContentDto, DeleteContent>(this, deleteContent);
		}
	}
}