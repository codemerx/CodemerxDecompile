using System;
using JustDecompile.Tools.MSBuildProjectBuilder.Contracts;

namespace JustDecompile.Tools.MSBuildProjectBuilder.ProjectItemFileWriters
{
    class XamlPageItemWriter : BaseXamlFileWriter
    {
		private readonly IXamlPageProjectItemWriter itemWriter;

		public XamlPageItemWriter(string projectRootDirectory, string relativeXamlPath, string sourceExtension, IXamlPageProjectItemWriter itemWriter)
            :base(projectRootDirectory, relativeXamlPath, sourceExtension, itemWriter)
        {
			this.itemWriter = itemWriter;
		}

        public override void GenerateProjectItems()
        {
			base.GenerateProjectItems();
			this.itemWriter.WriteXamlPageProjectItem(base.relativeXamlPath);
        }
    }
}
