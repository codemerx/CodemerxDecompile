using JustDecompile.SmartAssembly.Attributes;
using System;
using Telerik.JustDecompiler.External.Interfaces;

namespace Telerik.JustDecompiler.External
{
	[DoNotObfuscateType]
	[DoNotPruneType]
	public interface IFileGenerationNotifier
	{
		uint TotalFileCount
		{
			get;
			set;
		}

		void OnProjectFileGenerated(IFileGeneratedInfo args);
	}
}