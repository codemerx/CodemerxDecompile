using JustDecompile.SmartAssembly.Attributes;
using Mono.Cecil.AssemblyResolver;

namespace Telerik.JustDecompiler.External.Interfaces
{
	[DoNotObfuscateType]
	[DoNotPrune]
	public interface IFrameworkResolver
	{
		FrameworkVersion GetDefaultFallbackFramework4Version();
	}
}