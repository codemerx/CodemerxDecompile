using System;
using System.Collections.Generic;

namespace JustDecompile.Tools.MSBuildProjectBuilder.Constants
{
	// UAP is a superset of WinRT ( WinRT, Windows 8 and WinRT runtime )
	internal class UAPConstants
	{
		public const string UAPPlatformIdentifier = "UAP";
		public static readonly Version DefaultUAPVersion = new Version(10, 0, 10240, 0);

		#region DefaultUAPReferences
		// The default UAP references for 10.0.10240.0
		public static readonly HashSet<string> DefaultUAPReferences = new HashSet<string>()
		{
			"Windows.Foundation.FoundationContract",
			"Windows.Foundation.UniversalApiContract",
			"Windows.Networking.Connectivity.WwanContract"
		};
		#endregion
	}
}
