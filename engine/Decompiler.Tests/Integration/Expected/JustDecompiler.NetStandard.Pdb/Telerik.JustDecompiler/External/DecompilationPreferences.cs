using System;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.External.Interfaces;

namespace Telerik.JustDecompiler.External
{
	public class DecompilationPreferences : IDecompilationPreferences
	{
		private bool renameInvalidMembers;

		public bool DecompileDangerousResources
		{
			get;
			set;
		}

		public bool RenameInvalidMembers
		{
			get
			{
				return this.renameInvalidMembers;
			}
			set
			{
				this.renameInvalidMembers = value;
			}
		}

		public bool RenameInvalidNamespaces
		{
			get
			{
				return this.renameInvalidMembers;
			}
			set
			{
				this.renameInvalidMembers = value;
			}
		}

		public bool WriteDocumentation
		{
			get;
			set;
		}

		public bool WriteFullNames
		{
			get;
			set;
		}

		public bool WriteLargeNumbersInHex
		{
			get;
			set;
		}

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