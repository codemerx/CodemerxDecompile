using Squidex.Areas.Api.Controllers.Apps;
using Squidex.Domain.Apps.Core.Apps;
using Squidex.Domain.Apps.Entities;
using Squidex.Domain.Apps.Entities.Apps;
using Squidex.Infrastructure.Validation;
using Squidex.Web;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Apps.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class AppSettingsDto : Resource
	{
		[LocalizedRequired]
		public EditorDto[] Editors
		{
			get;
			set;
		}

		public bool HideDateTimeModeButton
		{
			get;
			set;
		}

		public bool HideScheduler
		{
			get;
			set;
		}

		[LocalizedRequired]
		public PatternDto[] Patterns
		{
			get;
			set;
		}

		public long Version
		{
			get;
			set;
		}

		public AppSettingsDto()
		{
		}

		private AppSettingsDto CreateLinks(Resources resources)
		{
			var variable = new { app = resources.get_App() };
			base.AddSelfLink(resources.Url<AppSettingsController>((AppSettingsController x) => "GetSettings", variable));
			if (resources.get_CanUpdateSettings())
			{
				base.AddPutLink("update", resources.Url<AppSettingsController>((AppSettingsController x) => "PutSettings", variable), null);
			}
			return this;
		}

		public static AppSettingsDto FromDomain(IAppEntity app, Resources resources)
		{
			AppSettings settings = app.get_Settings();
			AppSettingsDto appSettingsDto = new AppSettingsDto()
			{
				Editors = settings.get_Editors().Select<Editor, EditorDto>(new Func<Editor, EditorDto>(EditorDto.FromDomain)).ToArray<EditorDto>(),
				HideDateTimeModeButton = settings.get_HideDateTimeModeButton(),
				HideScheduler = settings.get_HideScheduler(),
				Patterns = settings.get_Patterns().Select<Pattern, PatternDto>(new Func<Pattern, PatternDto>(PatternDto.FromPattern)).ToArray<PatternDto>(),
				Version = app.get_Version()
			};
			return appSettingsDto.CreateLinks(resources);
		}
	}
}