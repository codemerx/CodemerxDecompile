using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using JustDecompile.SmartAssembly.Attributes;
using Telerik.JustDecompiler.Languages;
using Mono.Cecil.AssemblyResolver;

#if !NET35
using System.Threading;
#endif

namespace Telerik.JustDecompiler.External.Interfaces
{
    /// <summary>
    /// This is intended to be the single entry point to all clients that use JustDecompile engine
    /// </summary>
    [DoNotPrune]
    [DoNotObfuscateType]
    public interface IDecompiler
    {
        /// <summary>
        /// Looks for an assembly specified by its strong name in a number of predefined locations - GAC, framework folders, common files, etc.
        /// </summary>
        /// <param name="strongName">Sample format "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"</param>
        /// <param name="architecture">The target architecture of the assembly searched for. An assembly by the same strong name might exist in multiple versions each targeting a diferent architecture.</param>
        /// <param name="additionalSearchPath">JustDecompile will search for the assembly at this location too. Normally that'd be an app running folder or installation folder. Can be empty string.</param>
        /// <returns></returns>
        AssemblyIdentifier? TryFindAssembly(string strongName, Architecture architecture, string additionalSearchPath);

        /// <summary>
        /// Writes just the body of a given class member. Used by JC.
        /// </summary>
        /// <param name="member">The identifier of the method who's body should be written.</param>
        /// <param name="writer">The writer in which the produced code should be placed.</param>
        /// <param name="language">The language of the decompiled code.</param>
		/// <param name="decompilationPreferences">Contains preferences for the decompilation.</param>
		void WriteMemberBody(MemberIdentifier member, StringWriter writer, SupportedLanguages language, IDecompilationPreferences decompilationPreferences);
        
		/// <summary>
        /// Writes a given type. If the type is nested, the method writes the outermost type (i.e. if you ask it to write the type Namespace1.Type1.Type2, it will write the whole Namespace1.Type1).
        /// </summary>
        /// <param name="type">The type identifier of the type you want to write.</param>
        /// <param name="writer">A string writer in which the type will be written.</param>
        /// <param name="language">The language of the decompiled code.</param>
		/// <param name="decompilationPreferences">Contains preferences for the decompilation.</param>
		/// <returns>Returns a dictionary, mapping a code identifier to the place in the output code in <paramref name="writer"/> where the declaration of the code item begins.</returns>
		Dictionary<MemberIdentifier, CodeSpan> WriteType(MemberIdentifier type, StringWriter writer, SupportedLanguages language, IDecompilationPreferences decompilationPreferences);

#if !NET35
		/// <summary>
		/// Generates a PDB file dor a given assembly.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="sourcesRoot"></param>
		/// <param name="pdbPath"></param>
		/// <param name="language">The language of the decompiled code.</param>
		/// <param name="resolver">It is not always possible to distinguish between .net 4.0 and .net 4.5 frameworks (assemblies without references to mscorlib are possible). The users of this method
		/// should provide an implementation of IFrameworkResolver that specifies a framework version (either a default one or one taken from user input) should this point be reached.</param>
		/// <param name="notifier">A notifier to be used each time a file is created.</param>
		/// <param name="decompilationPreferences">Contains preferences for the decompilation. Only the preference for renaming invalid members is taken into account at this point.</param>
		Dictionary<MemberIdentifier, string> Generate(string target, string sourcesRoot, string pdbPath, SupportedLanguages language, IFrameworkResolver resolver, IFileGenerationNotifier notifier, IDecompilationPreferences decompilationPreferences, CancellationToken cancellationToken, ITargetPlatformResolver targetPlatformResolver);

		/// <summary>
		/// Generates a PDB file dor a given assembly.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="sourcesRoot"></param>
		/// <param name="pdbPath"></param>
		/// <param name="language">The language of the decompiled code.</param>
		/// <param name="resolver">It is not always possible to distinguish between .net 4.0 and .net 4.5 frameworks (assemblies without references to mscorlib are possible). The users of this method
		/// should provide an implementation of IFrameworkResolver that specifies a framework version (either a default one or one taken from user input) should this point be reached.</param>
		/// <param name="notifier">A notifier to be used each time a file is created.</param>
		/// <param name="decompilationPreferences">Contains preferences for the decompilation. Only the preference for renaming invalid members is taken into account at this point.</param>
		/// <param name="existingClassesMap"> A mapping between types that already have generated files for them and the corresponding filepath. No filepath in this dictionary should be in a subdirectory/directory of <paramref name="sourcesRoot"/></param>
		// The files in existingClassesMap will be used during the PDB generation instead of the ones generated by the project builder.
		Dictionary<MemberIdentifier, string> Generate(string target, string sourcesRoot, string pdbPath, SupportedLanguages language, IFrameworkResolver resolver, IFileGenerationNotifier notifier, IDecompilationPreferences decompilationPreferences, CancellationToken cancellationToken, Dictionary<MemberIdentifier, string> existingClassesMap, ITargetPlatformResolver targetPlatformResolver);
#else
		/// <summary>
		/// Generates a PDB file dor a given assembly.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="sourcesRoot"></param>
		/// <param name="pdbPath"></param>
		/// <param name="language">The language of the decompiled code.</param>
		/// <param name="resolver">It is not always possible to distinguish between .net 4.0 and .net 4.5 frameworks (assemblies without references to mscorlib are possible). The users of this method
		/// should provide an implementation of IFrameworkResolver that specifies a framework version (either a default one or one taken from user input) should this point be reached.</param>
		/// <param name="notifier">A notifier to be used each time a file is created.</param>
		/// <param name="decompilationPreferences">Contains preferences for the decompilation. Only the preference for renaming invalid members is taken into account at this point.</param>
		Dictionary<MemberIdentifier, string> Generate(string target, string sourcesRoot, string pdbPath, SupportedLanguages language, IFrameworkResolver resolver, IFileGenerationNotifier notifier, IDecompilationPreferences decompilationPreferences);

		/// <summary>
		/// Generates a PDB file dor a given assembly.
		/// </summary>
		/// <param name="target"></param>
		/// <param name="sourcesRoot"></param>
		/// <param name="pdbPath"></param>
		/// <param name="language">The language of the decompiled code.</param>
		/// <param name="resolver">It is not always possible to distinguish between .net 4.0 and .net 4.5 frameworks (assemblies without references to mscorlib are possible). The users of this method
		/// should provide an implementation of IFrameworkResolver that specifies a framework version (either a default one or one taken from user input) should this point be reached.</param>
		/// <param name="notifier">A notifier to be used each time a file is created.</param>
		/// <param name="decompilationPreferences">Contains preferences for the decompilation. Only the preference for renaming invalid members is taken into account at this point.</param>
		/// <param name="existingClassesMap"> A mapping between types that already have generated files for them and the corresponding filepath. No filepath in this dictionary should be in a subdirectory/directory of <paramref name="sourcesRoot"/></param>
		// The files in existingClassesMap will be used during the PDB generation instead of the ones generated by the project builder.
		Dictionary<MemberIdentifier, string> Generate(string target, string sourcesRoot, string pdbPath, SupportedLanguages language, IFrameworkResolver resolver, IFileGenerationNotifier notifier, IDecompilationPreferences decompilationPreferences, Dictionary<MemberIdentifier, string> existingClassesMap);
#endif
    }
}
