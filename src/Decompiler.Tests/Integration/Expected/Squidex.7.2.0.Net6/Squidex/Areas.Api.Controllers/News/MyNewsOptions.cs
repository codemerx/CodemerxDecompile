using System;
using System.Runtime.CompilerServices;

namespace Squidex.Areas.Api.Controllers.News
{
	[Nullable(0)]
	[NullableContext(1)]
	public sealed class MyNewsOptions
	{
		public string AppName
		{
			get;
			set;
		}

		public string ClientId
		{
			get;
			set;
		}

		public string ClientSecret
		{
			get;
			set;
		}

		public MyNewsOptions()
		{
		}

		public bool IsConfigured()
		{
			if (string.IsNullOrWhiteSpace(this.AppName) || string.IsNullOrWhiteSpace(this.ClientId))
			{
				return false;
			}
			return !string.IsNullOrWhiteSpace(this.ClientSecret);
		}
	}
}