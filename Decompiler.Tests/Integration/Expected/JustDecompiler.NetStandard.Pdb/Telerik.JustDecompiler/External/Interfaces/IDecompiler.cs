using JustDecompile.SmartAssembly.Attributes;
using Mono.Cecil.AssemblyResolver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Telerik.JustDecompiler;
using Telerik.JustDecompiler.External;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.External.Interfaces
{
	[DoNotObfuscateType]
	[DoNotPrune]
	public interface IDecompiler
	{
		Dictionary<MemberIdentifier, string> Generate(string target, string sourcesRoot, string pdbPath, SupportedLanguages language, IFrameworkResolver resolver, IFileGenerationNotifier notifier, IDecompilationPreferences decompilationPreferences, CancellationToken cancellationToken, ITargetPlatformResolver targetPlatformResolver);

		Dictionary<MemberIdentifier, string> Generate(string target, string sourcesRoot, string pdbPath, SupportedLanguages language, IFrameworkResolver resolver, IFileGenerationNotifier notifier, IDecompilationPreferences decompilationPreferences, CancellationToken cancellationToken, Dictionary<MemberIdentifier, string> existingClassesMap, ITargetPlatformResolver targetPlatformResolver);

		AssemblyIdentifier? TryFindAssembly(string strongName, Architecture architecture, string additionalSearchPath);

		void WriteMemberBody(MemberIdentifier member, StringWriter writer, SupportedLanguages language, IDecompilationPreferences decompilationPreferences);

		Dictionary<MemberIdentifier, CodeSpan> WriteType(MemberIdentifier type, StringWriter writer, SupportedLanguages language, IDecompilationPreferences decompilationPreferences);
	}
}