using NodaTime;
using Squidex.Domain.Apps.Core.Contents;
using Squidex.Domain.Apps.Entities.Contents.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Contents.Models
{
	public sealed class ChangeStatusDto
	{
		public bool CheckReferrers
		{
			get;
			set;
		}

		public Instant? DueTime
		{
			get;
			set;
		}

		[LocalizedRequired]
		public Squidex.Domain.Apps.Core.Contents.Status Status
		{
			get;
			set;
		}

		public ChangeStatusDto()
		{
		}

		[NullableContext(1)]
		public ChangeContentStatus ToCommand(DomainId id)
		{
			ChangeContentStatus changeContentStatu = new ChangeContentStatus();
			changeContentStatu.set_ContentId(id);
			return SimpleMapper.Map<ChangeStatusDto, ChangeContentStatus>(this, changeContentStatu);
		}
	}
}