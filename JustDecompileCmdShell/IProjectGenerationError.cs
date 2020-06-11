namespace JustDecompileCmdShell
{
    public interface IProjectGenerationError
    {
        string Message { get; }

        void PrintError();
    }
}