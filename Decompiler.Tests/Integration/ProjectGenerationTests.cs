using Decompiler.Tests.Helpers;
using JustDecompile.Tools.MSBuildProjectBuilder;
using System.IO;
using Telerik.JustDecompiler.External;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Languages.CSharp;
using Xunit;

namespace Decompiler.Tests.Integration
{
    public class ProjectGenerationTests
    {

        [Fact]
        public void CmdShel_ShouldGenerateCorrectOutput()
        {
            // Arrange
            string outputFolder = @"result";
            string targetFolder = @"Integration/Actual/JustDecompiler.NetStandard.dll";

            // Act
            this.BuildProject(targetFolder, outputFolder);

            // Assert
            TestHelper.AssertFoldersDiffRecursively(@"../../../Integration/Expected/JustDecompiler.NetStandard", outputFolder);
        }

        private void BuildProject(string target, string output)
        {
            if (Directory.Exists(output))
            {
                Directory.Delete(output, true);
            }

            Directory.CreateDirectory(output);

            MSBuildProjectBuilder projectBuilder = CreateProjectBuilder(target, output);
            projectBuilder.BuildProject();
        }

        private MSBuildProjectBuilder CreateProjectBuilder(string target, string output)
        {
            DecompilationPreferences preferences = new DecompilationPreferences();
            preferences.WriteFullNames = false;
            preferences.WriteDocumentation = true;
            preferences.RenameInvalidMembers = true;
            preferences.WriteLargeNumbersInHex = true;
            preferences.DecompileDangerousResources = false;

            ILanguage language = LanguageFactory.GetLanguage(CSharpVersion.None);
            string projFilePath = Path.Combine(output, Path.GetFileNameWithoutExtension(target) + language.VSProjectFileExtension);

            TestMSBuildProjectBuilder result = new TestMSBuildProjectBuilder(target, projFilePath, language, preferences);

            return result;
        }
    }
}
