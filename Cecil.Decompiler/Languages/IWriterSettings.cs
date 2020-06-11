namespace Telerik.JustDecompiler.Languages
{
    public interface IWriterSettings
    {
        bool WriteExceptionsAsComments { get; }

        bool ShouldGenerateBlocks { get; }

        bool RenameInvalidMembers { get; }

        bool WriteFullyQualifiedNames { get; }

        bool WriteDocumentation { get; }

        bool ShowCompilerGeneratedMembers { get; }
        
        bool WriteLargeNumbersInHex { get; }

        bool WriteDangerousResources { get; }
    }
}
