using Squidex.Domain.Apps.Entities.Apps.Commands;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Apps.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class CreateClientDto
	{
		[LocalizedRegularExpression("^[a-z0-9]+(\\-[a-z0-9]+)*$")]
		[LocalizedRequired]
		public string Id
		{
			get;
			set;
		}

		public CreateClientDto()
		{
		}

		public AttachClient ToCommand()
		{
			return SimpleMapper.Map<CreateClientDto, AttachClient>(this, new AttachClient());
		}
	}
}