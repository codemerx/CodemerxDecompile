using System;
using System.Runtime.CompilerServices;

namespace OrchardCore.Modules
{
	internal class PoweredByMiddlewareOptions : IPoweredByMiddlewareOptions
	{
		private const string PoweredByHeaderName = "X-Powered-By";

		private const string PoweredByHeaderValue = "OrchardCore";

		public bool Enabled { get; set; } = true;

		public string HeaderName
		{
			get
			{
				return "X-Powered-By";
			}
		}

		public string HeaderValue { get; set; } = "OrchardCore";

		public PoweredByMiddlewareOptions()
		{
		}
	}
}