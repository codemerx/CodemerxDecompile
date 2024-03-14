using Microsoft.AspNetCore.Mvc;
using Squidex.Domain.Apps.Core.Contents;
using Squidex.Domain.Apps.Entities.Contents.Commands;
using Squidex.Infrastructure;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Contents.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public class CreateContentDto
	{
		[FromBody]
		public ContentData Data
		{
			get;
			set;
		}

		[FromQuery]
		public DomainId? Id
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

		public CreateContentDto()
		{
		}

		public CreateContent ToCommand()
		{
			CreateContent createContent = new CreateContent();
			createContent.set_Data(this.Data);
			CreateContent createContent1 = createContent;
			if (this.Id.HasValue)
			{
				DomainId value = this.Id.Value;
				DomainId domainId = new DomainId();
				if (value != domainId)
				{
					domainId = this.Id.Value;
					if (!string.IsNullOrWhiteSpace(domainId.ToString()))
					{
						createContent1.set_ContentId(this.Id.Value);
					}
				}
			}
			if (this.Status.HasValue)
			{
				createContent1.set_Status(this.Status);
			}
			else if (this.Publish)
			{
				createContent1.set_Status(new Squidex.Domain.Apps.Core.Contents.Status?(Squidex.Domain.Apps.Core.Contents.Status.Published));
			}
			return createContent1;
		}
	}
}