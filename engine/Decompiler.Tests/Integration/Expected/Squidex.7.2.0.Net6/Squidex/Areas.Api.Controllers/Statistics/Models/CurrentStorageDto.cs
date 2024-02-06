using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Statistics.Models
{
	public sealed class CurrentStorageDto
	{
		public long MaxAllowed
		{
			get;
			set;
		}

		public long Size
		{
			get;
			set;
		}

		public CurrentStorageDto()
		{
		}
	}
}