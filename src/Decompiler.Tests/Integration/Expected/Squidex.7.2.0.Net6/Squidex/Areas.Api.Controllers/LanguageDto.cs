using Squidex.Infrastructure;
using Squidex.Infrastructure.Reflection;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class LanguageDto
	{
		[LocalizedRequired]
		public string EnglishName
		{
			get;
			set;
		}

		[LocalizedRequired]
		public string Iso2Code
		{
			get;
			set;
		}

		[LocalizedRequired]
		public string NativeName
		{
			get;
			set;
		}

		public LanguageDto()
		{
		}

		public static LanguageDto FromDomain(Language language)
		{
			return SimpleMapper.Map<Language, LanguageDto>(language, new LanguageDto());
		}
	}
}