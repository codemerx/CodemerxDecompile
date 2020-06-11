using System;
using JustDecompile.SmartAssembly.Attributes;
using Telerik.JustDecompiler.External.Interfaces;

namespace Telerik.JustDecompiler.External
{
	[DoNotObfuscateType]
	[DoNotPruneType]
	public class UniqueMemberIdentifier : IUniqueMemberIdentifier
	{

		public UniqueMemberIdentifier(string moduleFilePath, int metadataToken)
		{
			this.ModuleFilePath = moduleFilePath;
			this.MetadataToken = metadataToken;
		}

		public string ModuleFilePath { get; private set; }

		public int MetadataToken { get; private set; }
	}
}
