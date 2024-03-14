using Squidex.Domain.Apps.Entities.Apps.Commands;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Reflection;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Apps.Models
{
	public sealed class UpdateLanguageDto
	{
		[Nullable(new byte[] { 2, 1 })]
		public Language[] Fallback
		{
			[return: Nullable(new byte[] { 2, 1 })]
			get;
			set;
		}

		public bool? IsMaster
		{
			get;
			set;
		}

		public bool IsOptional
		{
			get;
			set;
		}

		public UpdateLanguageDto()
		{
		}

		[NullableContext(1)]
		public UpdateLanguage ToCommand(Language language)
		{
			UpdateLanguage updateLanguage = new UpdateLanguage();
			updateLanguage.set_Language(language);
			return SimpleMapper.Map<UpdateLanguageDto, UpdateLanguage>(this, updateLanguage);
		}
	}
}