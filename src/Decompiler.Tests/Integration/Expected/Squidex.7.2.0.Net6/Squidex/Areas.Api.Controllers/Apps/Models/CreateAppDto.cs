using Squidex.Domain.Apps.Entities.Apps.Commands;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Apps.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class CreateAppDto
	{
		[LocalizedRegularExpression("^[a-z0-9]+(\\-[a-z0-9]+)*$")]
		[LocalizedRequired]
		public string Name
		{
			get;
			set;
		}

		[Nullable(2)]
		public string Template
		{
			[NullableContext(2)]
			get;
			[NullableContext(2)]
			set;
		}

		public CreateAppDto()
		{
		}

		public CreateApp ToCommand()
		{
			return SimpleMapper.Map<CreateAppDto, CreateApp>(this, new CreateApp());
		}
	}
}