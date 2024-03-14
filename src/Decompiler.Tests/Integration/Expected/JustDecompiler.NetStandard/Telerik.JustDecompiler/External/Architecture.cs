using JustDecompile.SmartAssembly.Attributes;
using System;

namespace Telerik.JustDecompiler.External
{
	[DoNotObfuscateType]
	[DoNotPrune]
	public enum Architecture
	{
		I386,
		AMD64,
		IA64,
		AnyCPU,
		ARMv7
	}
}