using System;
namespace JustDecompileCmdShell
{
    public class CommandLineError : IProjectGenerationError
    {
        public CommandLineError(string message)
        {
            this.Message = message;
        }

        public string Message { get; private set; }

        public void PrintError()
        {
            CommandLineManager.WriteLine();
            CommandLineManager.WriteLineColor(ConsoleColor.Red, Message);
            CommandLineManager.PrintHelpText();
        }
    }
}