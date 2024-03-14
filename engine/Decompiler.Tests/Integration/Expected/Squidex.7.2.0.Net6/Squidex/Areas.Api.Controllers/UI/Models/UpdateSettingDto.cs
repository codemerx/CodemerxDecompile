using Squidex.Infrastructure.Json.Objects;
using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.UI.Models
{
	public sealed class UpdateSettingDto
	{
		public JsonValue Value
		{
			get;
			set;
		}

		public UpdateSettingDto()
		{
		}
	}
}