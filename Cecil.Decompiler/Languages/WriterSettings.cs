using System;

namespace Telerik.JustDecompiler.Languages
{
    public class WriterSettings : IWriterSettings
    {
        public bool WriteExceptionsAsComments { get; private set; }

        public bool ShouldGenerateBlocks { get; private set; }

        public bool RenameInvalidMembers { get; private set; }

        public bool WriteFullyQualifiedNames { get; private set; }

        public bool WriteDocumentation { get; private set; }

        public bool ShowCompilerGeneratedMembers { get; private set; }

        public bool WriteLargeNumbersInHex { get; private set; }

        public bool WriteDangerousResources { get; private set; }

        public WriterSettings(bool writeExceptionsAsComments = false,
                              bool shouldGenerateBlocks = false,
                              bool renameInvalidMembers = false,
                              bool writeFullyQualifiedNames = false,
                              bool writeDocumentation = false,
                              bool showCompilerGeneratedMembers = false,
                              bool writeLargeNumbersInHex = true,
                              bool writeDangerousResources = false)
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
