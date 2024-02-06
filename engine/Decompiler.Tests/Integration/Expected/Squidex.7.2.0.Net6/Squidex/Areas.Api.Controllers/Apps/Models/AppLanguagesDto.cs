using Squidex.Areas.Api.Controllers.Apps;
using Squidex.Domain.Apps.Core.Apps;
using Squidex.Domain.Apps.Entities.Apps;
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
	public sealed class AppLanguagesDto : Resource
	{
		[LocalizedRequired]
		public AppLanguageDto[] Items
		{
			get;
			set;
		}

		public AppLanguagesDto()
		{
		}

		private AppLanguagesDto CreateLinks(Resources resources)
		{
			var variable = new { app = resources.get_App() };
			base.AddSelfLink(resources.Url<AppLanguagesController>((AppLanguagesController x) => "GetLanguages", variable));
			if (resources.get_CanCreateLanguage())
			{
				base.AddPostLink("create", resources.Url<AppLanguagesController>((AppLanguagesController x) => "PostLanguage", variable), null);
			}
			return this;
		}

		public static AppLanguagesDto FromDomain(IAppEntity app, Resources resources)
		{
			LanguagesConfig languages = app.get_Languages();
			return (new AppLanguagesDto()
			{
				Items = (
					from x in languages.get_Languages()
					select AppLanguageDto.FromDomain(x.Key, x.Value, languages) into x
					select x.CreateLinks(resources, app) into x
					orderby x.IsMaster descending, x.Iso2Code
					select x).ToArray<AppLanguageDto>()
			}).CreateLinks(resources);
		}
	}
}