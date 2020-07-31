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

using Microsoft.XmlDiffPatch;
using System;
using System.IO;
using System.Xml;
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

                string actualFileName = actualFileNames[i];
                Assert.Equal(GetRelativeFilePath(expectedFileName, expectedFolderPath), GetRelativeFilePath(actualFileName, actualFolderPath));

                AssertFileContent(expectedFileName, actualFileName, actualFolderPath);
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

        private static void AssertFileContent(string expectedFileName, string actualFileName, string actualFolderPath)
        {
            if (actualFileName.EndsWith(".sln"))
            {
                // TODO Handle .sln files with different Guids
                return;
            }

            try
            {
                if (actualFileName.EndsWith(".csproj"))
                {
                    XmlDocument expectedDocument = LoadXml(expectedFileName);
                    XmlDocument actualDocument = LoadXml(actualFileName);

                    XmlDiff xmldiff = new XmlDiff(XmlDiffOptions.IgnoreChildOrder |
                                        XmlDiffOptions.IgnoreNamespaces |
                                        XmlDiffOptions.IgnorePrefixes);

                    Assert.True(xmldiff.Compare(expectedDocument, actualDocument));
                }
                else
                {
                    string expectedFileText = File.ReadAllText(expectedFileName);
                    string actualFileText = File.ReadAllText(actualFileName);

                    Assert.Equal(expectedFileText, actualFileText);
                }
            }
            catch (Exception e)
            {
                throw new ContentAssertException($"Content assert failed for file: {GetRelativeFilePath(actualFileName, actualFolderPath)}", e);
            }
        }

        private static XmlDocument LoadXml(string fileName, bool skipGuid = true)
        {
            XmlDocument document = new XmlDocument();
            document.Load(fileName);

            if (skipGuid)
            {
                for (int i = 0; i < document.GetElementsByTagName("ProjectGuid").Count; i++)
                {
                    XmlNode projectElementNode = document.GetElementsByTagName("ProjectGuid").Item(i);
                    projectElementNode.InnerText = string.Empty;
                }
            }

            return document;
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
