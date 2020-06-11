using System;

namespace JustDecompile.Tools.MSBuildProjectBuilder.Contracts
{
	public interface IProjectItemWriter : IRegularProjectItemWriter, IResXDesignerProjectItemWriter, IResourceProjectItemWriter
	{
	}
}
