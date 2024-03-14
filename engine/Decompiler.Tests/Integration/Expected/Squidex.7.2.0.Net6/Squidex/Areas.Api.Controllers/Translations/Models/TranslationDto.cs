using Squidex.Infrastructure.Reflection;
using Squidex.Text.Translations;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Translations.Models
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class TranslationDto
	{
		public TranslationResultCode Result
		{
			get;
			set;
		}

		public string Text
		{
			get;
			set;
		}

		public TranslationDto()
		{
		}

		[NullableContext(1)]
		public static TranslationDto FromDomain(TranslationResult translation)
		{
			return SimpleMapper.Map<TranslationResult, TranslationDto>(translation, new TranslationDto()
			{
				Result = translation.get_Code()
			});
		}
	}
}