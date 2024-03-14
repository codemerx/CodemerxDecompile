using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.Assets.Models
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class AssetMeta
	{
		public string IsDuplicate
		{
			get;
			set;
		}

		public AssetMeta()
		{
		}
	}
}