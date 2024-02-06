using Squidex.Domain.Apps.Core.Apps;
using Squidex.Domain.Apps.Entities.Apps.Commands;
using Squidex.Infrastructure.Collections;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Apps.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class UpdateAppSettingsDto
	{
		[Required]
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

		[Required]
		public PatternDto[] Patterns
		{
			get;
			set;
		}

		public UpdateAppSettingsDto()
		{
		}

		public UpdateAppSettings ToCommand()
		{
			ReadonlyList<Editor> readonlyList;
			ReadonlyList<Pattern> readonlyList1;
			UpdateAppSettings updateAppSetting = new UpdateAppSettings();
			AppSettings appSetting = new AppSettings();
			EditorDto[] editors = this.Editors;
			if (editors != null)
			{
				readonlyList = ReadonlyList.ToReadonlyList<Editor>(
					from x in (IEnumerable<EditorDto>)editors
					select x.ToEditor());
			}
			else
			{
				readonlyList = null;
			}
			appSetting.set_Editors(readonlyList);
			appSetting.set_HideScheduler(this.HideScheduler);
			appSetting.set_HideDateTimeModeButton(this.HideDateTimeModeButton);
			PatternDto[] patterns = this.Patterns;
			if (patterns != null)
			{
				readonlyList1 = ReadonlyList.ToReadonlyList<Pattern>(
					from x in (IEnumerable<PatternDto>)patterns
					select x.ToPattern());
			}
			else
			{
				readonlyList1 = null;
			}
			appSetting.set_Patterns(readonlyList1);
			updateAppSetting.set_Settings(appSetting);
			return updateAppSetting;
		}
	}
}