using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Statistics.Models
{
	[Nullable(0)]
	[NullableContext(2)]
	public sealed class LogDownloadDto
	{
		public string DownloadUrl
		{
			get;
			set;
		}

		public LogDownloadDto()
		{
		}
	}
}