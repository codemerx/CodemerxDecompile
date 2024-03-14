using System;
using System.IO;
using JustDecompile.Tools.MSBuildProjectBuilder.Contracts;

namespace JustDecompile.Tools.MSBuildProjectBuilder.ProjectItemFileWriters
{
    abstract class BaseXamlFileWriter : BaseProjectItemFileWriter
    {
        protected readonly string relativeXamlPath;
		private readonly IXamlProjectItemWriter itemWriter;
		private readonly string relativeCodePath;

		public BaseXamlFileWriter(string projectRootDirectory, string relativeXamlPath, string sourceExtension, IXamlProjectItemWriter itemWriter)
        {
            this.relativeXamlPath = relativeXamlPath;
			this.itemWriter = itemWriter;
			this.relativeCodePath = relativeXamlPath + sourceExtension;
            this.fullPath = Path.Combine(projectRootDirectory, relativeCodePath);
        }

        public override void GenerateProjectItems()
        {
			this.itemWriter.WriteXamlCodeEntryProjectItem(this.relativeCodePath, this.relativeXamlPath);
        }
    }
}
