using JustDecompile.SmartAssembly.Attributes;

namespace Telerik.JustDecompiler.External.Interfaces
{
	[DoNotObfuscateType]
	[DoNotPruneType]
	public interface IFileGeneratedInfo 
	{
		string FullPath { get; }

		bool HasErrors { get; }
	}
}