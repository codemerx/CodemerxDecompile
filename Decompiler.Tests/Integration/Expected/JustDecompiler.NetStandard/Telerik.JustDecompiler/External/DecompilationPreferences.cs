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
				return;
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
				return;
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
			base();
			this.set_WriteDocumentation(true);
			this.set_WriteFullNames(false);
			this.renameInvalidMembers = true;
			this.set_WriteLargeNumbersInHex(true);
			this.set_DecompileDangerousResources(false);
			return;
		}
	}
}