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
				return get_FullPath();
			}
			set
			{
				set_FullPath(value);
			}
		}

		// <FullPath>k__BackingField
		private string u003cFullPathu003ek__BackingField;

		public string get_FullPath()
		{
			return this.u003cFullPathu003ek__BackingField;
		}

		private void set_FullPath(string value)
		{
			this.u003cFullPathu003ek__BackingField = value;
			return;
		}

		public bool HasErrors
		{
			get
			{
				return get_HasErrors();
			}
			set
			{
				set_HasErrors(value);
			}
		}

		// <HasErrors>k__BackingField
		private bool u003cHasErrorsu003ek__BackingField;

		public bool get_HasErrors()
		{
			return this.u003cHasErrorsu003ek__BackingField;
		}

		private void set_HasErrors(bool value)
		{
			this.u003cHasErrorsu003ek__BackingField = value;
			return;
		}

		public FileGeneratedInfo(string fullPath, bool hasErrors)
		{
			base();
			this.set_FullPath(fullPath);
			this.set_HasErrors(hasErrors);
			return;
		}
	}
}