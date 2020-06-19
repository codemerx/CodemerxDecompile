using System;
using System.IO;

namespace JustDecompile.Tools.MSBuildProjectBuilder.ProjectItemFileWriters
{
    abstract class BaseProjectItemFileWriter : IProjectItemFileWriter
    {
        protected string fullPath;

        public string FullSourceFilePath
        {
            get
            {
                return fullPath;
            }
        }

        public abstract void GenerateProjectItems();

        public void CreateProjectSourceFile(System.IO.Stream stream)
        {
            string dirPath = Path.GetDirectoryName(fullPath);
            if(!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }

            stream.Seek(0, SeekOrigin.Begin);
            using (FileStream fileStream = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(fileStream);
            }
        }
    }

}
