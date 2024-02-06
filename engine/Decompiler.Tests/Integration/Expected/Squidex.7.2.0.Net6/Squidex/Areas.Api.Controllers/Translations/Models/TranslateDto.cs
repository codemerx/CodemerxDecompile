using Squidex.Infrastructure;
using Squidex.Infrastructure.Validation;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Translations.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class TranslateDto
	{
		public Language SourceLanguage
		{
			get;
			set;
		}

		[LocalizedRequired]
		public Language TargetLanguage
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

		public TranslateDto()
		{
		}
	}
}