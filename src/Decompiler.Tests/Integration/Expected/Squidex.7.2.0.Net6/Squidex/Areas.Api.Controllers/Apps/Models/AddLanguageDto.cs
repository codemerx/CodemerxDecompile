using Squidex.Domain.Apps.Entities.Apps.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Apps.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class AddLanguageDto
	{
		[LocalizedRequired]
		public Squidex.Infrastructure.Language Language
		{
			get;
			set;
		}

		public AddLanguageDto()
		{
		}

		public AddLanguage ToCommand()
		{
			return SimpleMapper.Map<AddLanguageDto, AddLanguage>(this, new AddLanguage());
		}
	}
}