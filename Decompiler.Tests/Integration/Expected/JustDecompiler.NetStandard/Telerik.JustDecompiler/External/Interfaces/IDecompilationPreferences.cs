using JustDecompile.SmartAssembly.Attributes;
using System;

namespace Telerik.JustDecompiler.External.Interfaces
{
	[DoNotObfuscateType]
	[DoNotPruneType]
	public interface IDecompilationPreferences
	{
		bool DecompileDangerousResources
		{
			get;
			set;
		}

		bool RenameInvalidMembers
		{
			get;
			set;
		}

		bool WriteDocumentation
		{
			get;
			set;
		}

		bool WriteFullNames
		{
			get;
			set;
		}

		bool WriteLargeNumbersInHex
		{
			get;
			set;
		}
	}
}