using System;

namespace Telerik.JustDecompiler.Languages
{
	public interface IWriterSettings
	{
		bool RenameInvalidMembers
		{
			get;
		}

		bool ShouldGenerateBlocks
		{
			get;
		}

		bool ShowCompilerGeneratedMembers
		{
			get;
		}

		bool WriteDangerousResources
		{
			get;
		}

		bool WriteDocumentation
		{
			get;
		}

		bool WriteExceptionsAsComments
		{
			get;
		}

		bool WriteFullyQualifiedNames
		{
			get;
		}

		bool WriteLargeNumbersInHex
		{
			get;
		}
	}
}