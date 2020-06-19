using System;
using System.IO;
using JustDecompile.Tools.MSBuildProjectBuilder.FilePathsServices;
using JustDecompile.Tools.MSBuildProjectBuilder.Contracts;

namespace JustDecompile.Tools.MSBuildProjectBuilder.ProjectItemFileWriters
{
    class ResXDesignerWriter : BaseProjectItemFileWriter
    {
        private readonly string relativeResourcePath;
		private readonly IResXDesignerProjectItemWriter itemWriter;
		private readonly string relativeDesignerPath;

        public ResXDesignerWriter(string projectRootDirectory, string relativeResourcePath, IResXDesignerProjectItemWriter itemWriter, IFilePathsService filePathsService)
        {
            this.relativeResourcePath = relativeResourcePath;
			this.itemWriter = itemWriter;
			this.relativeDesignerPath = Path.Combine(Path.GetDirectoryName(relativeResourcePath),
				filePathsService.GetResourceDesignerRelativePath(Path.GetFileNameWithoutExtension(relativeResourcePath)));

            this.fullPath = Path.Combine(projectRootDirectory, this.relativeDesignerPath);
        }

        public override void GenerateProjectItems()
        {
			this.itemWriter.WriteResXDesignerSourceEntryProjectItem(this.relativeDesignerPath, this.relativeResourcePath);

			this.itemWriter.WriteResXDesignerResourceProjectItem(this.relativeResourcePath, this.relativeDesignerPath);
        }
    }
}
