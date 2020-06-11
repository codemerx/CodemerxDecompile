using Telerik.JustDecompiler.External.Interfaces;

namespace Telerik.JustDecompiler.External
{
	public class FileGeneratedInfo : IFileGeneratedInfo
	{
		/// <summary>
		/// The full filepath of the created file.
		/// </summary>
		public string FullPath { get; private set; }

		/// <summary>
		/// Signals if there was error during the generation of the file.
		/// </summary>
		public bool HasErrors { get; private set; }

		/// <summary>
		/// Creates a FileGeneratedInfo instance.
		/// </summary>
		/// <param name="fullPath">The full filepath of the created file.</param>
		/// <param name="hasErrors">Signals if there was error during the generation of the file.</param>
		public FileGeneratedInfo(string fullPath, bool hasErrors)
		{
			this.FullPath = fullPath;
			this.HasErrors = hasErrors;
		}
	}
}