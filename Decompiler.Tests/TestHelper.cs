using System.IO;
using Xunit;

namespace Decompiler.Tests
{
    internal static class TestHelper
    {
        internal static void AssertFoldersDiffRecursively(string expectedFolderPath, string actualFolderPath)
        {
            if (!Directory.Exists(actualFolderPath))
            {
                Fail($"Folder {actualFolderPath} not found");
            }

            string[] expectedFileNames = Directory.GetFiles(expectedFolderPath);
            string[] actualFileNames = Directory.GetFiles(actualFolderPath);
            Assert.Equal(expectedFileNames.Length, actualFileNames.Length);

            for (int i = 0; i < expectedFileNames.Length; i++)
            {
                string expectedFileName = expectedFileNames[i];
                if (expectedFileName.EndsWith(".dll"))
                {
                    continue;
                }

                string expectedFileContent = File.ReadAllText(expectedFileName);

                string actualFileName = actualFileNames[i];
                string actualFileContent = File.ReadAllText(actualFileName);

                Assert.Equal(GetFileName(expectedFileName, expectedFolderPath), GetFileName(actualFileName, actualFolderPath));

                Assert.Equal(expectedFileContent, actualFileContent);
            }

            string[] expectdeSubFolderNames = Directory.GetDirectories(expectedFolderPath);
            string[] actualSubFolderNames = Directory.GetDirectories(actualFolderPath);
            Assert.Equal(expectdeSubFolderNames.Length, actualSubFolderNames.Length);

            for (int i = 0; i < expectdeSubFolderNames.Length; i++)
            {
                string expectedSubFolderFullName = expectdeSubFolderNames[i];
                string actualSubFolderFullName = actualSubFolderNames[i];
                Assert.Equal(GetFileName(expectedSubFolderFullName, expectedFolderPath), GetFileName(actualSubFolderFullName, actualFolderPath));

                AssertFoldersDiffRecursively(expectedSubFolderFullName, actualSubFolderFullName);
            }
        }

        private static string GetFileName(string fullPath, string rootFolder)
        {
            return fullPath.Substring(rootFolder.Length);
        }

        private static void Fail(string message)
        {
            Assert.True(false, message);
        }
    }
}
