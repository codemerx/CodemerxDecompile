using System;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.External.Interfaces;

namespace Telerik.JustDecompiler.External
{
	public class FileGeneratedInfo : IFileGeneratedInfo
	{
		public string FullPath
		{
			get
			{
				return JustDecompileGenerated_get_FullPath();
			}
			set
			{
				JustDecompileGenerated_set_FullPath(value);
			}
		}

		private string JustDecompileGenerated_FullPath_k__BackingField;

		public string JustDecompileGenerated_get_FullPath()
		{
			return this.JustDecompileGenerated_FullPath_k__BackingField;
		}

		private void JustDecompileGenerated_set_FullPath(string value)
		{
			this.JustDecompileGenerated_FullPath_k__BackingField = value;
		}

		public bool HasErrors
		{
			get
			{
				return JustDecompileGenerated_get_HasErrors();
			}
			set
			{
				JustDecompileGenerated_set_HasErrors(value);
			}
		}

		private bool JustDecompileGenerated_HasErrors_k__BackingField;

		public bool JustDecompileGenerated_get_HasErrors()
		{
			return this.JustDecompileGenerated_HasErrors_k__BackingField;
		}

		private void JustDecompileGenerated_set_HasErrors(bool value)
		{
			this.JustDecompileGenerated_HasErrors_k__BackingField = value;
		}

		public FileGeneratedInfo(string fullPath, bool hasErrors)
		{
			this.FullPath = fullPath;
			this.HasErrors = hasErrors;
		}
	}
}