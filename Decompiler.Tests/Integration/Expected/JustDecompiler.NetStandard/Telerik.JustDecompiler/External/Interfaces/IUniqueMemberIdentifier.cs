using JustDecompile.SmartAssembly.Attributes;
using System;

namespace Telerik.JustDecompiler.External.Interfaces
{
	[DoNotObfuscateType]
	[DoNotPruneType]
	public interface IUniqueMemberIdentifier
	{
		int MetadataToken
		{
			get;
		}

		string ModuleFilePath
		{
			get;
		}
	}
}