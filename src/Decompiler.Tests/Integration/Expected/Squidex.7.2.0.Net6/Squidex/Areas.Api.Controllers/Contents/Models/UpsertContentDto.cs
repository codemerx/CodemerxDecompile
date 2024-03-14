using Microsoft.AspNetCore.Mvc;
using Squidex.Domain.Apps.Core.Contents;
using Squidex.Domain.Apps.Entities.Contents.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Reflection;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Contents.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public class UpsertContentDto
	{
		[FromBody]
		public ContentData Data
		{
			get;
			set;
		}

		[FromQuery]
		public bool Patch
		{
			get;
			set;
		}

		[FromQuery]
		[Obsolete("Use 'status' query string now.")]
		public bool Publish
		{
			get;
			set;
		}

		[FromQuery]
		public Squidex.Domain.Apps.Core.Contents.Status? Status
		{
			get;
			set;
		}

		public UpsertContentDto()
		{
		}

		public UpsertContent ToCommand(DomainId id)
		{
			UpsertContent upsertContent = new UpsertContent();
			upsertContent.set_ContentId(id);
			UpsertContent upsertContent1 = SimpleMapper.Map<UpsertContentDto, UpsertContent>(this, upsertContent);
			if (!upsertContent1.get_Status().HasValue && this.Publish)
			{
				upsertContent1.set_Status(new Squidex.Domain.Apps.Core.Contents.Status?(Squidex.Domain.Apps.Core.Contents.Status.Published));
			}
			return upsertContent1;
		}
	}
}