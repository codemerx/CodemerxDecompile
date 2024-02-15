//    Copyright CodeMerx 2020
//    This file is part of CodemerxDecompile.

//    CodemerxDecompile is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as published by
//    the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.

//    CodemerxDecompile is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//    GNU Affero General Public License for more details.

//    You should have received a copy of the GNU Affero General Public License
//    along with CodemerxDecompile.  If not, see<https://www.gnu.org/licenses/>.

using Decompiler.Tests.Helpers;
using JustDecompile.Tools.MSBuildProjectBuilder;
using System;
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
        [InlineData("JustDecompiler.NetStandard", "JustDecompiler.NetStandard", true)]
        [InlineData("JustDecompiler.NetStandard.Pdb", "JustDecompiler.NetStandard", true)]
        [InlineData("OrchardCore", "OrchardCore", true)]
        [InlineData("Mix.Cms.Lib", "Mix.Cms.Lib", true)]
        [InlineData("Piranha", "Piranha", true)]
        //[InlineData("Sample3", "Sample3", false)]
        //[InlineData("ConsoleApp2", "ConsoleApp2", false)]
        [InlineData("Squidex.7.2.0.Net6", "Squidex", false)]
        [InlineData("coolstore.ShoppingCart.Net6", "ShoppingCart", false)]
        [InlineData("clean-architecture-manga.accounts-api.Net7", "Application", false)]
        [InlineData("NorthwindTraders.NetStandard2.1", "Northwind.Application", false)]
        public void BuildProject_ShouldGenerateCorrectOutput(string assemblyFolder, string assemblyName, bool windowsOnly)
        {
            // Skip Windows Only tests
            if (windowsOnly && !OperatingSystem.IsWindows())
            {
                return;
            }

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

            ILanguage language = LanguageFactory.GetLanguage(CSharpVersion.V7);
            string projFilePath = Path.Combine(output, Path.GetFileNameWithoutExtension(target) + language.VSProjectFileExtension);

            TestMSBuildProjectBuilder result = new TestMSBuildProjectBuilder(target, projFilePath, language, preferences);

            return result;
        }
    }
}
