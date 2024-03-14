using JustDecompileCmdShell;

namespace ConsoleRunner
{
    public class App
    {
        public static void Main(string[] args)
        {
            GeneratorProjectInfo generatorProjectInfo = CommandLineManager.Parse(args);
            CmdShell shell = new CmdShell();
            shell.Run(generatorProjectInfo);
        }
    }
}
