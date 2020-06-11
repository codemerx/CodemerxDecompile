using JustDecompile.EngineInfrastructure;
using JustDecompile.Tools.MSBuildProjectBuilder;
using JustDecompileCmd;
using Mono.Cecil;
using Mono.Cecil.AssemblyResolver;
using Telerik.JustDecompiler.External;
using Telerik.JustDecompiler.External.Interfaces;
using Telerik.JustDecompiler.Languages;

namespace JustDecompileCmdShell
{
    public class TestCmdShell : CmdShell
    {
        public override void Run(GeneratorProjectInfo projectInfo)
        {
            AssemblyDefinition assembly = Telerik.JustDecompiler.Decompiler.Utilities.GetAssembly(projectInfo.Target);
            ProjectGenerationSettings settings = base.GetSettings(projectInfo);
            base.CreateOutputDirectory(projectInfo);
            base.RunInternal(assembly, projectInfo, settings);
        }

        protected override void AttachProjectBuilderEventHandlers(BaseProjectBuilder projectBuilder)
        {
        }

        protected override void DetachProjectBuilderEventHandlers(BaseProjectBuilder projectBuilder)
        {
        }
    }
}
