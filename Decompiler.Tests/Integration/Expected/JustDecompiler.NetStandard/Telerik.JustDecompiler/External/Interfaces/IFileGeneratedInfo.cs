using JustDecompile.SmartAssembly.Attributes;
using System;

namespace Telerik.JustDecompiler.External.Interfaces
{
	[DoNotObfuscateType]
	[DoNotPruneType]
	public interface IFileGeneratedInfo
	{
		string FullPath
		{
			get;
		}

		bool HasErrors
		{
			get;
		}
	}
}