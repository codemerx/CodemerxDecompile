using System;
using System.Runtime.CompilerServices;

namespace Telerik.JustDecompiler.Languages
{
	public class WriterSettings : IWriterSettings
	{
		public bool RenameInvalidMembers
		{
			get
			{
				return JustDecompileGenerated_get_RenameInvalidMembers();
			}
			set
			{
				JustDecompileGenerated_set_RenameInvalidMembers(value);
			}
		}

		private bool JustDecompileGenerated_RenameInvalidMembers_k__BackingField;

		public bool JustDecompileGenerated_get_RenameInvalidMembers()
		{
			return this.JustDecompileGenerated_RenameInvalidMembers_k__BackingField;
		}

		private void JustDecompileGenerated_set_RenameInvalidMembers(bool value)
		{
			this.JustDecompileGenerated_RenameInvalidMembers_k__BackingField = value;
		}

		public bool ShouldGenerateBlocks
		{
			get
			{
				return JustDecompileGenerated_get_ShouldGenerateBlocks();
			}
			set
			{
				JustDecompileGenerated_set_ShouldGenerateBlocks(value);
			}
		}

		private bool JustDecompileGenerated_ShouldGenerateBlocks_k__BackingField;

		public bool JustDecompileGenerated_get_ShouldGenerateBlocks()
		{
			return this.JustDecompileGenerated_ShouldGenerateBlocks_k__BackingField;
		}

		private void JustDecompileGenerated_set_ShouldGenerateBlocks(bool value)
		{
			this.JustDecompileGenerated_ShouldGenerateBlocks_k__BackingField = value;
		}

		public bool ShowCompilerGeneratedMembers
		{
			get
			{
				return JustDecompileGenerated_get_ShowCompilerGeneratedMembers();
			}
			set
			{
				JustDecompileGenerated_set_ShowCompilerGeneratedMembers(value);
			}
		}

		private bool JustDecompileGenerated_ShowCompilerGeneratedMembers_k__BackingField;

		public bool JustDecompileGenerated_get_ShowCompilerGeneratedMembers()
		{
			return this.JustDecompileGenerated_ShowCompilerGeneratedMembers_k__BackingField;
		}

		private void JustDecompileGenerated_set_ShowCompilerGeneratedMembers(bool value)
		{
			this.JustDecompileGenerated_ShowCompilerGeneratedMembers_k__BackingField = value;
		}

		public bool WriteDangerousResources
		{
			get
			{
				return JustDecompileGenerated_get_WriteDangerousResources();
			}
			set
			{
				JustDecompileGenerated_set_WriteDangerousResources(value);
			}
		}

		private bool JustDecompileGenerated_WriteDangerousResources_k__BackingField;

		public bool JustDecompileGenerated_get_WriteDangerousResources()
		{
			return this.JustDecompileGenerated_WriteDangerousResources_k__BackingField;
		}

		private void JustDecompileGenerated_set_WriteDangerousResources(bool value)
		{
			this.JustDecompileGenerated_WriteDangerousResources_k__BackingField = value;
		}

		public bool WriteDocumentation
		{
			get
			{
				return JustDecompileGenerated_get_WriteDocumentation();
			}
			set
			{
				JustDecompileGenerated_set_WriteDocumentation(value);
			}
		}

		private bool JustDecompileGenerated_WriteDocumentation_k__BackingField;

		public bool JustDecompileGenerated_get_WriteDocumentation()
		{
			return this.JustDecompileGenerated_WriteDocumentation_k__BackingField;
		}

		private void JustDecompileGenerated_set_WriteDocumentation(bool value)
		{
			this.JustDecompileGenerated_WriteDocumentation_k__BackingField = value;
		}

		public bool WriteExceptionsAsComments
		{
			get
			{
				return JustDecompileGenerated_get_WriteExceptionsAsComments();
			}
			set
			{
				JustDecompileGenerated_set_WriteExceptionsAsComments(value);
			}
		}

		private bool JustDecompileGenerated_WriteExceptionsAsComments_k__BackingField;

		public bool JustDecompileGenerated_get_WriteExceptionsAsComments()
		{
			return this.JustDecompileGenerated_WriteExceptionsAsComments_k__BackingField;
		}

		private void JustDecompileGenerated_set_WriteExceptionsAsComments(bool value)
		{
			this.JustDecompileGenerated_WriteExceptionsAsComments_k__BackingField = value;
		}

		public bool WriteFullyQualifiedNames
		{
			get
			{
				return JustDecompileGenerated_get_WriteFullyQualifiedNames();
			}
			set
			{
				JustDecompileGenerated_set_WriteFullyQualifiedNames(value);
			}
		}

		private bool JustDecompileGenerated_WriteFullyQualifiedNames_k__BackingField;

		public bool JustDecompileGenerated_get_WriteFullyQualifiedNames()
		{
			return this.JustDecompileGenerated_WriteFullyQualifiedNames_k__BackingField;
		}

		private void JustDecompileGenerated_set_WriteFullyQualifiedNames(bool value)
		{
			this.JustDecompileGenerated_WriteFullyQualifiedNames_k__BackingField = value;
		}

		public bool WriteLargeNumbersInHex
		{
			get
			{
				return JustDecompileGenerated_get_WriteLargeNumbersInHex();
			}
			set
			{
				JustDecompileGenerated_set_WriteLargeNumbersInHex(value);
			}
		}

		private bool JustDecompileGenerated_WriteLargeNumbersInHex_k__BackingField;

		public bool JustDecompileGenerated_get_WriteLargeNumbersInHex()
		{
			return this.JustDecompileGenerated_WriteLargeNumbersInHex_k__BackingField;
		}

		private void JustDecompileGenerated_set_WriteLargeNumbersInHex(bool value)
		{
			this.JustDecompileGenerated_WriteLargeNumbersInHex_k__BackingField = value;
		}

		public WriterSettings(bool writeExceptionsAsComments = false, bool shouldGenerateBlocks = false, bool renameInvalidMembers = false, bool writeFullyQualifiedNames = false, bool writeDocumentation = false, bool showCompilerGeneratedMembers = false, bool writeLargeNumbersInHex = true, bool writeDangerousResources = false)
		{
			this.WriteExceptionsAsComments = writeExceptionsAsComments;
			this.ShouldGenerateBlocks = shouldGenerateBlocks;
			this.RenameInvalidMembers = renameInvalidMembers;
			this.WriteFullyQualifiedNames = writeFullyQualifiedNames;
			this.WriteDocumentation = writeDocumentation;
			this.ShowCompilerGeneratedMembers = showCompilerGeneratedMembers;
			this.WriteLargeNumbersInHex = writeLargeNumbersInHex;
			this.WriteDangerousResources = writeDangerousResources;
		}
	}
}