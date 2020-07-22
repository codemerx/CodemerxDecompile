using System;
using System.IO;
using JustDecompile.Tools.MSBuildProjectBuilder.Contracts;

namespace JustDecompile.Tools.MSBuildProjectBuilder.ProjectItemFileWriters
{
    class WinFormsItemWriter : BaseProjectItemFileWriter
    {
        private readonly string relativeResourcePath;
		private readonly string relativeWinFormPath;
		private readonly IWinFormsProjectItemWriter itemWriter;

		public WinFormsItemWriter(string projectRootDirectory, string relativeResourcePath, string sourceExtension, IWinFormsProjectItemWriter itemWriter)
        {
			this.relativeWinFormPath = Path.ChangeExtension(relativeResourcePath, sourceExtension);
			this.relativeResourcePath = relativeResourcePath;
            this.fullPath = Path.Combine(projectRootDirectory, relativeWinFormPath);
			this.itemWriter = itemWriter;
		}

        public override void GenerateProjectItems()
        {
			this.itemWriter.WriteWinFormsEntryProjectItem(this.relativeWinFormPath);

			this.itemWriter.WriteWinFormsResourceProjectItem(this.relativeResourcePath, this.relativeWinFormPath);
        }
    }
}
