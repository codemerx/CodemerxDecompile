using Decompiler.Tests.Helpers;
using JustDecompile.Tools.MSBuildProjectBuilder;
using System.IO;
using Telerik.JustDecompiler.External;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Languages.CSharp;
using Xunit;

namespace Decompiler.Tests.Integration
{
    public class MSBuildProjectBuilderTests
    {
        private const string TargetFolderTemplate = @"Integration/Resources/{0}/{1}.dll";
        private const string OutputFolderTemplate = @"Result/{0}";
        private const string ExpectedFolderTemplate = @"../../../Integration/Expected/{0}";

        [Theory]
        [InlineData("JustDecompiler.NetStandard", "JustDecompiler.NetStandard")]
        [InlineData("JustDecompiler.NetStandard.Pdb", "JustDecompiler.NetStandard")]
        [InlineData("OrchardCore", "OrchardCore")]
        public void BuildProject_ShouldGenerateCorrectOutput(string assemblyFolder, string assemblyName)
        {
            // Arrange
            string targetFolder = string.Format(TargetFolderTemplate, assemblyFolder, assemblyName);
            string outputFolder = string.Format(OutputFolderTemplate, assemblyFolder);
            string expectedFolder = string.Format(ExpectedFolderTemplate, assemblyFolder);

            // Act
            this.BuildProject(targetFolder, outputFolder);

            // Assert
            TestHelper.AssertFoldersDiffRecursively(expectedFolder, outputFolder);
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
