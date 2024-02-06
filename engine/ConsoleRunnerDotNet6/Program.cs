using JustDecompileCmdShell;

namespace ConsoleRunnerDotNet6
{
    public class Program
    {
        public static void Main(string[] args)
        {
            GeneratorProjectInfo generatorProjectInfo = CommandLineManager.Parse(args);
            CmdShell shell = new CmdShell();
            shell.Run(generatorProjectInfo);
        }
    }
}