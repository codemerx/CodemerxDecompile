using Squidex.Domain.Apps.Entities.Schemas.Commands;
using Squidex.Infrastructure.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Schemas.Models
{
	[Nullable(new byte[] { 0, 1, 1 })]
	[NullableContext(1)]
	public sealed class ConfigurePreviewUrlsDto : Dictionary<string, string>
	{
		public ConfigurePreviewUrlsDto()
		{
		}

		public ConfigurePreviewUrls ToCommand()
		{
			ConfigurePreviewUrls configurePreviewUrl = new ConfigurePreviewUrls();
			configurePreviewUrl.set_PreviewUrls(ReadonlyDictionary.ToReadonlyDictionary<string, string>(new Dictionary<string, string>(this)));
			return configurePreviewUrl;
		}
	}
}