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
				return get_RenameInvalidMembers();
			}
			set
			{
				set_RenameInvalidMembers(value);
			}
		}

		// <RenameInvalidMembers>k__BackingField
		private bool u003cRenameInvalidMembersu003ek__BackingField;

		public bool get_RenameInvalidMembers()
		{
			return this.u003cRenameInvalidMembersu003ek__BackingField;
		}

		private void set_RenameInvalidMembers(bool value)
		{
			this.u003cRenameInvalidMembersu003ek__BackingField = value;
			return;
		}

		public bool ShouldGenerateBlocks
		{
			get
			{
				return get_ShouldGenerateBlocks();
			}
			set
			{
				set_ShouldGenerateBlocks(value);
			}
		}

		// <ShouldGenerateBlocks>k__BackingField
		private bool u003cShouldGenerateBlocksu003ek__BackingField;

		public bool get_ShouldGenerateBlocks()
		{
			return this.u003cShouldGenerateBlocksu003ek__BackingField;
		}

		private void set_ShouldGenerateBlocks(bool value)
		{
			this.u003cShouldGenerateBlocksu003ek__BackingField = value;
			return;
		}

		public bool ShowCompilerGeneratedMembers
		{
			get
			{
				return get_ShowCompilerGeneratedMembers();
			}
			set
			{
				set_ShowCompilerGeneratedMembers(value);
			}
		}

		// <ShowCompilerGeneratedMembers>k__BackingField
		private bool u003cShowCompilerGeneratedMembersu003ek__BackingField;

		public bool get_ShowCompilerGeneratedMembers()
		{
			return this.u003cShowCompilerGeneratedMembersu003ek__BackingField;
		}

		private void set_ShowCompilerGeneratedMembers(bool value)
		{
			this.u003cShowCompilerGeneratedMembersu003ek__BackingField = value;
			return;
		}

		public bool WriteDangerousResources
		{
			get
			{
				return get_WriteDangerousResources();
			}
			set
			{
				set_WriteDangerousResources(value);
			}
		}

		// <WriteDangerousResources>k__BackingField
		private bool u003cWriteDangerousResourcesu003ek__BackingField;

		public bool get_WriteDangerousResources()
		{
			return this.u003cWriteDangerousResourcesu003ek__BackingField;
		}

		private void set_WriteDangerousResources(bool value)
		{
			this.u003cWriteDangerousResourcesu003ek__BackingField = value;
			return;
		}

		public bool WriteDocumentation
		{
			get
			{
				return get_WriteDocumentation();
			}
			set
			{
				set_WriteDocumentation(value);
			}
		}

		// <WriteDocumentation>k__BackingField
		private bool u003cWriteDocumentationu003ek__BackingField;

		public bool get_WriteDocumentation()
		{
			return this.u003cWriteDocumentationu003ek__BackingField;
		}

		private void set_WriteDocumentation(bool value)
		{
			this.u003cWriteDocumentationu003ek__BackingField = value;
			return;
		}

		public bool WriteExceptionsAsComments
		{
			get
			{
				return get_WriteExceptionsAsComments();
			}
			set
			{
				set_WriteExceptionsAsComments(value);
			}
		}

		// <WriteExceptionsAsComments>k__BackingField
		private bool u003cWriteExceptionsAsCommentsu003ek__BackingField;

		public bool get_WriteExceptionsAsComments()
		{
			return this.u003cWriteExceptionsAsCommentsu003ek__BackingField;
		}

		private void set_WriteExceptionsAsComments(bool value)
		{
			this.u003cWriteExceptionsAsCommentsu003ek__BackingField = value;
			return;
		}

		public bool WriteFullyQualifiedNames
		{
			get
			{
				return get_WriteFullyQualifiedNames();
			}
			set
			{
				set_WriteFullyQualifiedNames(value);
			}
		}

		// <WriteFullyQualifiedNames>k__BackingField
		private bool u003cWriteFullyQualifiedNamesu003ek__BackingField;

		public bool get_WriteFullyQualifiedNames()
		{
			return this.u003cWriteFullyQualifiedNamesu003ek__BackingField;
		}

		private void set_WriteFullyQualifiedNames(bool value)
		{
			this.u003cWriteFullyQualifiedNamesu003ek__BackingField = value;
			return;
		}

		public bool WriteLargeNumbersInHex
		{
			get
			{
				return get_WriteLargeNumbersInHex();
			}
			set
			{
				set_WriteLargeNumbersInHex(value);
			}
		}

		// <WriteLargeNumbersInHex>k__BackingField
		private bool u003cWriteLargeNumbersInHexu003ek__BackingField;

		public bool get_WriteLargeNumbersInHex()
		{
			return this.u003cWriteLargeNumbersInHexu003ek__BackingField;
		}

		private void set_WriteLargeNumbersInHex(bool value)
		{
			this.u003cWriteLargeNumbersInHexu003ek__BackingField = value;
			return;
		}

		public WriterSettings(bool writeExceptionsAsComments = false, bool shouldGenerateBlocks = false, bool renameInvalidMembers = false, bool writeFullyQualifiedNames = false, bool writeDocumentation = false, bool showCompilerGeneratedMembers = false, bool writeLargeNumbersInHex = true, bool writeDangerousResources = false)
		{
			base();
			this.set_WriteExceptionsAsComments(writeExceptionsAsComments);
			this.set_ShouldGenerateBlocks(shouldGenerateBlocks);
			this.set_RenameInvalidMembers(renameInvalidMembers);
			this.set_WriteFullyQualifiedNames(writeFullyQualifiedNames);
			this.set_WriteDocumentation(writeDocumentation);
			this.set_ShowCompilerGeneratedMembers(showCompilerGeneratedMembers);
			this.set_WriteLargeNumbersInHex(writeLargeNumbersInHex);
			this.set_WriteDangerousResources(writeDangerousResources);
			return;
		}
	}
}