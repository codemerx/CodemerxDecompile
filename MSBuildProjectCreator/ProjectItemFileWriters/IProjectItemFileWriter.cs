using System;
using System.IO;

namespace JustDecompile.Tools.MSBuildProjectBuilder.ProjectItemFileWriters
{
    public interface IProjectItemFileWriter
    {
        void GenerateProjectItems();
        void CreateProjectSourceFile(Stream stream);
        string FullSourceFilePath { get; }
    }
}
