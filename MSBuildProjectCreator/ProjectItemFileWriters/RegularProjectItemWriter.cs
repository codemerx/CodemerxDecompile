using System;
using System.IO;
using JustDecompile.Tools.MSBuildProjectBuilder.Contracts;

namespace JustDecompile.Tools.MSBuildProjectBuilder.ProjectItemFileWriters
{
    class RegularProjectItemWriter : BaseProjectItemFileWriter
    {
        private readonly string relativePath;
		private readonly IRegularProjectItemWriter itemWriter;

		public RegularProjectItemWriter(string projectRootDirectory, string relativeFilePath, IRegularProjectItemWriter itemWriter)
        {
            this.relativePath = relativeFilePath;
			this.itemWriter = itemWriter;
			this.fullPath = Path.Combine(projectRootDirectory, this.relativePath);
        }
    
        public override void GenerateProjectItems()
        {
			this.itemWriter.WriteRegularProjectItem(this.relativePath);
        }
    }
}
