using Squidex.Areas.Api.Controllers.Apps;
using Squidex.Domain.Apps.Core.Apps;
using Squidex.Domain.Apps.Entities.Apps;
using Squidex.Infrastructure;
using Squidex.Infrastructure.Validation;
using Squidex.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Apps.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class AppLanguageDto : Resource
	{
		[LocalizedRequired]
		public string EnglishName
		{
			get;
			set;
		}

		[LocalizedRequired]
		public Language[] Fallback
		{
			get;
			set;
		}

		public bool IsMaster
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

		public bool IsOptional
		{
			get;
			set;
		}

		public AppLanguageDto()
		{
		}

		public AppLanguageDto CreateLinks(Resources resources, IAppEntity app)
		{
			var variable = new { app = resources.get_App(), language = this.Iso2Code };
			if (!this.IsMaster)
			{
				if (resources.get_CanUpdateLanguage())
				{
					base.AddPutLink("update", resources.Url<AppLanguagesController>((AppLanguagesController x) => "PutLanguage", variable), null);
				}
				if (resources.get_CanDeleteLanguage() && app.get_Languages().get_Languages().Count > 1)
				{
					base.AddDeleteLink("delete", resources.Url<AppLanguagesController>((AppLanguagesController x) => "DeleteLanguage", variable), null);
				}
			}
			return this;
		}

		public static AppLanguageDto FromDomain(Language language, LanguageConfig config, LanguagesConfig languages)
		{
			return new AppLanguageDto()
			{
				EnglishName = language.get_EnglishName(),
				IsMaster = languages.IsMaster(language),
				IsOptional = languages.IsOptional(language),
				Iso2Code = language.get_Iso2Code(),
				Fallback = config.get_Fallbacks().ToArray<Language>()
			};
		}
	}
}