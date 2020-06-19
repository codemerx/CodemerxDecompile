using System;
using System.Collections.Generic;

namespace JustDecompile.Tools.MSBuildProjectBuilder.Constants
{
	internal class WinRTConstants
	{
		public const string WindowsStoreAppGUID = "{BC8A1FFA-BEE3-4634-8014-F334798102B3}";
		public const string PortableClassLibraryGUID = "{786C830F-07A1-408B-BD7F-6EE04809D6DB}";
		public const string WindowsPhoneAppGUID = "{76F1466A-8B6D-4E39-A767-685A06062A39}";

		public const string UWPProjectGUID = "{A5A43C5B-DE2A-4C0C-9213-0A381AF9435A}";

		#region NetCoreFrameworkAssemblies
		public static readonly HashSet<string> NetCoreFrameworkAssemblies = new HashSet<string>()
		{
			"Microsoft.CSharp",
			"Microsoft.VisualBasic",
			"Microsoft.Win32.Primitives",
			"System.AppContext",
			"System.Collections.Concurrent",
			"System.Collections",
			"System.Collections.Immutable",
			"System.Collections.NonGeneric",
			"System.Collections.Specialized",
			"System.ComponentModel.Annotations",
			"System.ComponentModel",
			"System.ComponentModel.EventBasedAsync",
			"System.Data.Common",
			"System.Diagnostics.Contracts",
			"System.Diagnostics.Debug",
			"System.Diagnostics.StackTrace",
			"System.Diagnostics.Tools",
			"System.Diagnostics.Tracing",
			"System.Dynamic.Runtime",
			"System.Globalization.Calendars",
			"System.Globalization",
			"System.Globalization.Extensions",
			"System.IO.Compression",
			"System.IO.Compression.ZipFile",
			"System.IO",
			"System.IO.FileSystem",
			"System.IO.FileSystem.Primitives",
			"System.IO.IsolatedStorage",
			"System.IO.UnmanagedMemoryStream",
			"System.Linq",
			"System.Linq.Expressions",
			"System.Linq.Parallel",
			"System.Linq.Queryable",
			"System.Net.Http",
			"System.Net.Http.Rtc",
			"System.Net.NetworkInformation",
			"System.Net.Primitives",
			"System.Net.Requests",
			"System.Net.Sockets",
			"System.Net.WebHeaderCollection",
			"System.Numerics.Vectors",
			"System.Numerics.Vectors.WindowsRuntime",
			"System.ObjectModel",
			"System.Private.DataContractSerialization",
			"System.Private.Networking",
			"System.Private.ServiceModel",
			"System.Private.Uri",
			"System.Reflection.Context",
			"System.Reflection.DispatchProxy",
			"System.Reflection",
			"System.Reflection.Extensions",
			"System.Reflection.Metadata",
			"System.Reflection.Primitives",
			"System.Reflection.TypeExtensions",
			"System.Resources.ResourceManager",
			"System.Runtime",
			"System.Runtime.Extensions",
			"System.Runtime.Handles",
			"System.Runtime.InteropServices",
			"System.Runtime.InteropServices.WindowsRuntime",
			"System.Runtime.Numerics",
			"System.Runtime.Serialization.Json",
			"System.Runtime.Serialization.Primitives",
			"System.Runtime.Serialization.Xml",
			"System.Runtime.WindowsRuntime",
			"System.Runtime.WindowsRuntime.UI.Xaml",
			"System.Security.Claims",
			"System.Security.Principal",
			"System.ServiceModel.Duplex",
			"System.ServiceModel.Http",
			"System.ServiceModel.NetTcp",
			"System.ServiceModel.Primitives",
			"System.ServiceModel.Security",
			"System.Text.Encoding.CodePages",
			"System.Text.Encoding",
			"System.Text.Encoding.Extensions",
			"System.Text.RegularExpressions",
			"System.Threading",
			"System.Threading.Overlapped",
			"System.Threading.Tasks.Dataflow",
			"System.Threading.Tasks",
			"System.Threading.Tasks.Parallel",
			"System.Threading.Timer",
			"System.Xml.ReaderWriter",
			"System.Xml.XDocument",
			"System.Xml.XmlDocument",
			"System.Xml.XmlSerializer"
		};
		#endregion
	}
}
