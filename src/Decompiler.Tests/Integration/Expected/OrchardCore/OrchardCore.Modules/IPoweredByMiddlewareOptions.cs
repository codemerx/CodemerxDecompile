using System;

namespace OrchardCore.Modules
{
	public interface IPoweredByMiddlewareOptions
	{
		bool Enabled
		{
			get;
			set;
		}

		string HeaderName
		{
			get;
		}

		string HeaderValue
		{
			get;
			set;
		}
	}
}