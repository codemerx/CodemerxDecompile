using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.UI.Models
{
	public sealed class UISettingsDto
	{
		public bool CanCreateApps
		{
			get;
			set;
		}

		public bool CanCreateTeams
		{
			get;
			set;
		}

		public UISettingsDto()
		{
		}
	}
}