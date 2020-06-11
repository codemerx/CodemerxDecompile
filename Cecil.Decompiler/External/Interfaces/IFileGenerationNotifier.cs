using System;
using JustDecompile.SmartAssembly.Attributes;
using Telerik.JustDecompiler.External.Interfaces;

namespace Telerik.JustDecompiler.External
{
	[DoNotObfuscateType]
	[DoNotPruneType]
	public interface IFileGenerationNotifier 
	{
		/// <summary>
		/// Notifies that a file was created. Additional information about the type of the file is contained in the args.
		/// </summary>
		/// <param name="args">Contains specific information about the item in the newly created file.</param>
		void OnProjectFileGenerated(IFileGeneratedInfo args);

		/// <summary>
		/// Contains the number of all files that will be generated. Will be set correctly before the first call to OnProjectFileGenerated and will not be changed throought
		/// the life of the object anymore.
		/// </summary>
		uint TotalFileCount { get; set; }
	}
}
