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
            string outpuFolder = @"result";
            string[] testArgs = { @"/target:Integration/Actual/JustDecompiler.NetStandard.dll", @"/out", outpuFolder };
            GeneratorProjectInfo generatorProjectInfo = CommandLineManager.Parse(testArgs);

            CmdShell shell = new CmdShell();

            // Act
            shell.Run(generatorProjectInfo);

            // Assert
            TestHelper.AssertFoldersDiffRecursively(@"../../../Integration/Expected/JustDecompiler.NetStandard", outpuFolder);
        }
    }
}
