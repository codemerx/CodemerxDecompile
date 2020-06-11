using System;
using JustDecompile.Tools.MSBuildProjectBuilder.Contracts;

namespace JustDecompile.Tools.MSBuildProjectBuilder.ProjectItemFileWriters
{
    class AppDefinitionItemWriter : BaseXamlFileWriter
    {
		private readonly IAppDefinitionProjectItemWriter itemWriter;

		public AppDefinitionItemWriter(string projectRootDirectory, string relativeXamlPath, string sourceExtension, IAppDefinitionProjectItemWriter itemWriter)
            :base(projectRootDirectory, relativeXamlPath, sourceExtension, itemWriter)
        {
			this.itemWriter = itemWriter;
		}

        public override void GenerateProjectItems()
        {
			base.GenerateProjectItems();
			this.itemWriter.WriteAppDefinitionXamlEntryProjectItem(base.relativeXamlPath);
        }
    }
}
