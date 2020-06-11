using System;
using System.Collections.Generic;
using Mono.Cecil;
using Telerik.JustDecompiler.Common;
using Mono.Cecil.AssemblyResolver;

namespace JustDecompile.Tools.MSBuildProjectBuilder.ProjectFileManagers
{
	public class TestNetCoreProjectFileManager : NetCoreProjectFileManager
	{
		private readonly FrameworkVersion defaultFrameworkVersion;

		public TestNetCoreProjectFileManager(AssemblyDefinition assembly, Dictionary<ModuleDefinition, Guid> modulesProjectsGuids, FrameworkVersion defaultFrameworkVersion) 
			: base(assembly, null, modulesProjectsGuids)
		{
			this.defaultFrameworkVersion = defaultFrameworkVersion;
		}

		protected override string GetTargetFrameworkVersion(ModuleDefinition module)
		{
			return this.defaultFrameworkVersion.ToString(includeVersionSign: false);
		}
	}
}
