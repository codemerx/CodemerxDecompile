using JustDecompile.SmartAssembly.Attributes;

namespace Telerik.JustDecompiler.External.Interfaces
{
	/// <summary>
	/// A class containing preferences for the decompilation.
	/// </summary>
	[DoNotObfuscateType]
	[DoNotPruneType]
	public interface IDecompilationPreferences 
	{
		/// <summary>
		/// Decides weather the XML documentation for the decompiled assembly should be included in the results (if there is documentation present alongside the assembly).
		/// </summary>
		bool WriteDocumentation { get; set; }

		/// <summary>
		/// Decided weather full names should always be used in the decompiled code or only when necesary to avoid colliding names.
		/// </summary>
		bool WriteFullNames { get; set; }

		/// <summary>
		/// Dediced weather to rename invalid member identifiers.
		/// </summary>
		bool RenameInvalidMembers { get; set; }

        /// <summary>
        /// Decides whether to write large number in hexadecimal format or not.
        /// </summary>
        bool WriteLargeNumbersInHex { get; set; }

        /// <summary>
        /// Decides whether to decompile resources which may contain malicious code. WARNING: Use with trusted assemblies only.
        /// </summary>
        bool DecompileDangerousResources { get; set; }
    }
}