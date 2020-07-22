using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JustDecompile.External.JustAssembly
{
	class FileGeneratedInfo : IFileGeneratedInfo
	{
		public string FullPath { get; private set; }

		public bool HasErrors { get; private set; }

		public FileGeneratedInfo(string fullPath, bool hasErrors)
		{
			this.FullPath = fullPath;
			this.HasErrors = hasErrors;
		}
	}
}
