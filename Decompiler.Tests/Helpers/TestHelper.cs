using System;
using System.IO;
using Xunit;

namespace Decompiler.Tests.Helpers
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

                string expectedFileContent = GetFileContent(expectedFileName);

                string actualFileName = actualFileNames[i];
                string actualFileContent = GetFileContent(actualFileName);

                Assert.Equal(GetRelativeFilePath(expectedFileName, expectedFolderPath), GetRelativeFilePath(actualFileName, actualFolderPath));

                try
                {
                    Assert.Equal(expectedFileContent, actualFileContent);
                }
                catch (Exception e)
                {
                    throw new ContentAssertException($"Content assert failed for file: {GetRelativeFilePath(actualFileName, actualFolderPath)}", e);
                }
            }

            string[] expectdeSubFolderNames = Directory.GetDirectories(expectedFolderPath);
            string[] actualSubFolderNames = Directory.GetDirectories(actualFolderPath);
            Assert.Equal(expectdeSubFolderNames.Length, actualSubFolderNames.Length);

            for (int i = 0; i < expectdeSubFolderNames.Length; i++)
            {
                string expectedSubFolderFullName = expectdeSubFolderNames[i];
                string actualSubFolderFullName = actualSubFolderNames[i];
                Assert.Equal(GetRelativeFilePath(expectedSubFolderFullName, expectedFolderPath), GetRelativeFilePath(actualSubFolderFullName, actualFolderPath));

                AssertFoldersDiffRecursively(expectedSubFolderFullName, actualSubFolderFullName);
            }
        }

        private static string GetFileContent(string fileName)
        {
            if (fileName.EndsWith(".csproj") || fileName.EndsWith(".sln"))
            {
                // TODO Ignore ProjectGuid from file content comparison.
                //XmlDocument doc = new XmlDocument();
                //doc.Load(fileName);

                //for (int i = 0; i < doc.GetElementsByTagName("ProjectGuid").Count; i++)
                //{
                //    XmlNode projectElementNode = doc.GetElementsByTagName("ProjectGuid").Item(i);
                //    projectElementNode.InnerText = string.Empty;
                //}

                // TODO Add handling for .csproj and .sln files to compare the file content regardless of the lines order.
                return string.Empty;
            }
            else
            {
                return File.ReadAllText(fileName);
            }
        }

        private static string GetRelativeFilePath(string fullPath, string rootFolder)
        {
            return fullPath.Substring(rootFolder.Length);
        }

        private static void Fail(string message)
        {
            Assert.True(false, message);
        }
    }
}
