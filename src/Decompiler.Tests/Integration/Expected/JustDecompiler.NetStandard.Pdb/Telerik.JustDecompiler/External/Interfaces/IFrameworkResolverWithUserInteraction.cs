using JustDecompile.SmartAssembly.Attributes;
using Mono.Cecil.AssemblyResolver;
using System;

namespace Telerik.JustDecompiler.External.Interfaces
{
	[DoNotObfuscateType]
	[DoNotPrune]
	public interface IFrameworkResolverWithUserInteraction : IFrameworkResolver
	{
		FrameworkVersion GetDefaultFallbackFramework4Version(string message);
	}
}