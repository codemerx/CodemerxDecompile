namespace JustDecompileCmdShell
{
    public class CommandLineHelpError : IProjectGenerationError
    {
        public CommandLineHelpError() { }

        public string Message { get; private set; }

        public void PrintError()
        {
            CommandLineManager.PrintHelpText();
        }
    }
}