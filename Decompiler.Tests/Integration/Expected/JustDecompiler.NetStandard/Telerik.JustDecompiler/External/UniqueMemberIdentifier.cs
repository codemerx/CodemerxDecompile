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
				return get_MetadataToken();
			}
			set
			{
				set_MetadataToken(value);
			}
		}

		// <MetadataToken>k__BackingField
		private int u003cMetadataTokenu003ek__BackingField;

		public int get_MetadataToken()
		{
			return this.u003cMetadataTokenu003ek__BackingField;
		}

		private void set_MetadataToken(int value)
		{
			this.u003cMetadataTokenu003ek__BackingField = value;
			return;
		}

		public string ModuleFilePath
		{
			get
			{
				return get_ModuleFilePath();
			}
			set
			{
				set_ModuleFilePath(value);
			}
		}

		// <ModuleFilePath>k__BackingField
		private string u003cModuleFilePathu003ek__BackingField;

		public string get_ModuleFilePath()
		{
			return this.u003cModuleFilePathu003ek__BackingField;
		}

		private void set_ModuleFilePath(string value)
		{
			this.u003cModuleFilePathu003ek__BackingField = value;
			return;
		}

		public UniqueMemberIdentifier(string moduleFilePath, int metadataToken)
		{
			base();
			this.set_ModuleFilePath(moduleFilePath);
			this.set_MetadataToken(metadataToken);
			return;
		}
	}
}