using Telerik.JustDecompiler.External.Interfaces;

namespace Telerik.JustDecompiler.External
{
	public class DecompilationPreferences : IDecompilationPreferences
	{
		private bool renameInvalidMembers;
		/// <summary>
		/// Decides weather the XML documentation for the decompiled assembly should be included in the results (if there is documentation present alongside the assembly).
		/// </summary>
		public bool WriteDocumentation { get; set; }

		/// <summary>
		/// Decides weather full names should always be used in the decompiled code or only when necesary to avoid colliding names.
		/// </summary>
		public bool WriteFullNames { get; set; }

		/// <summary>
		/// Dedices weather to rename invalid member identifiers.
		/// </summary>
		public bool RenameInvalidMembers
		{ 
			get 
			{ 
				return renameInvalidMembers; 
			} 
			set 
			{ 
				renameInvalidMembers = value; 
			} 
		}

		/// <summary>
		/// Decides weather to rename invalid namespaces.
		/// </summary>
		public bool RenameInvalidNamespaces
		{
			get
			{
				return renameInvalidMembers;
			}
			set
			{
				renameInvalidMembers = value;
			}
		}

        /// <summary>
        /// Decides whether to write large number in hexadecimal format or not.
        /// </summary>
        public bool WriteLargeNumbersInHex { get; set; }

        /// <summary>
        /// Decides whether to decompile resources which may contain malicious code. WARNING: Use with trusted assemblies only.
        /// </summary>
        public bool DecompileDangerousResources { get; set; }

        /// <summary>
        /// Creates a new DecompilationPreferences object and sets its properties to the default values.
        /// </summary>
        public DecompilationPreferences()
		{
			this.WriteDocumentation = true;
			this.WriteFullNames = false;
			this.renameInvalidMembers = true;
            this.WriteLargeNumbersInHex = true;
            this.DecompileDangerousResources = false;
		}
	}
}
