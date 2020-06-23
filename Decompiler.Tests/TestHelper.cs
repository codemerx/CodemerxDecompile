using System.IO;
using Xunit;

namespace Decompiler.Tests
{
    internal static class TestHelper
    {
        internal static void AssertFoldersDiffRecursively(string originFolderPath, string resultFolderPath)
        {
            if (!Directory.Exists(resultFolderPath))
            {
                Fail($"Folder {resultFolderPath} not found");
            }

            string[] originFiles = Directory.GetFiles(originFolderPath);
            string[] resultFiles = Directory.GetFiles(resultFolderPath);
            Assert.Equal(originFiles.Length, resultFiles.Length);

            for (int i = 0; i < originFiles.Length; i++)
            {
                string originFileName = originFiles[i];
                if (originFileName.EndsWith(".dll"))
                {
                    continue;
                }

                string originFileContent = File.ReadAllText(originFileName);

                string resultFileName = resultFiles[i];
                string resultFileContent = File.ReadAllText(resultFileName);

                //Assert.Equal(originFileName, resultFileName);

                Assert.Equal(originFileContent, resultFileContent);
            }

            string[] originSubFolders = Directory.GetDirectories(originFolderPath);
            string[] resultSubFolders = Directory.GetDirectories(resultFolderPath);
            Assert.Equal(originSubFolders.Length, resultSubFolders.Length);

            for (int i = 0; i < originSubFolders.Length; i++)
            {
                string originSubFolderFullName = originSubFolders[i];
                string resultSubFolderFullName = resultSubFolders[i];
                //Assert.Equal(originSubFolderFullName, resultSubFolderFullName);

                AssertFoldersDiffRecursively(originSubFolderFullName, resultSubFolderFullName);
            }
        }

        private static void Fail(string message)
        {
            Assert.True(false, message);
        }
    }
}
