using JustDecompileCmdShell;
using System.Reflection;
using Xunit;

namespace Decompiler.Tests.Integration
{
    public class ProjectGenerationTests
    {
        [Fact]
        public void CmdShel_ShouldGenerateCorrectOutput()
        {
            // Arrange
            string[] testArgs = { @"/target:Actual/JustDecompiler.NetStandard.dll", @"/out", @"result" };
            GeneratorProjectInfo generatorProjectInfo = CommandLineManager.Parse(testArgs);

            CmdShell shell = new CmdShell();

            // Act
            shell.Run(generatorProjectInfo);

            // Assert
            TestHelper.AssertFoldersDiffRecursively(@"Integration/Expected/JustDecompiler.NetStandard", "result");
        }
    }
}
