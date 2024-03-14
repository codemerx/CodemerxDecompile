using JustDecompile.SmartAssembly.Attributes;
using System;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.External.Interfaces;

namespace Telerik.JustDecompiler.External
{
	[DoNotObfuscateType]
	[DoNotPruneType]
	public class UniqueMemberIdentifier : IUniqueMemberIdentifier
	{
		public int MetadataToken
		{
			get
			{
				return JustDecompileGenerated_get_MetadataToken();
			}
			set
			{
				JustDecompileGenerated_set_MetadataToken(value);
			}
		}

		private int JustDecompileGenerated_MetadataToken_k__BackingField;

		public int JustDecompileGenerated_get_MetadataToken()
		{
			return this.JustDecompileGenerated_MetadataToken_k__BackingField;
		}

		private void JustDecompileGenerated_set_MetadataToken(int value)
		{
			this.JustDecompileGenerated_MetadataToken_k__BackingField = value;
		}

		public string ModuleFilePath
		{
			get
			{
				return JustDecompileGenerated_get_ModuleFilePath();
			}
			set
			{
				JustDecompileGenerated_set_ModuleFilePath(value);
			}
		}

		private string JustDecompileGenerated_ModuleFilePath_k__BackingField;

		public string JustDecompileGenerated_get_ModuleFilePath()
		{
			return this.JustDecompileGenerated_ModuleFilePath_k__BackingField;
		}

		private void JustDecompileGenerated_set_ModuleFilePath(string value)
		{
			this.JustDecompileGenerated_ModuleFilePath_k__BackingField = value;
		}

		public UniqueMemberIdentifier(string moduleFilePath, int metadataToken)
		{
			this.ModuleFilePath = moduleFilePath;
			this.MetadataToken = metadataToken;
		}
	}
}