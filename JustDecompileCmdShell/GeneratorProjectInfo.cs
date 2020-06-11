
using System;
using Telerik.JustDecompiler.Languages;
using Telerik.JustDecompiler.Languages.CSharp;
using JustDecompile.Tools.MSBuildProjectBuilder;
using Mono.Cecil.AssemblyResolver;

namespace JustDecompileCmdShell
{
    public class GeneratorProjectInfo
    {
        private FrameworkVersion frameworkVersion;

        public GeneratorProjectInfo(IProjectGenerationError error)
            : this(null, null, error)
        {
        }

        public GeneratorProjectInfo(string target, string @out)
            : this(target, @out, null)
        {
        }

        private GeneratorProjectInfo(string target, string @out, IProjectGenerationError error)
        {
            this.Target = target;
            this.Out = @out;

            this.Language = LanguageFactory.GetLanguage(CSharpVersion.V7);
            this.VisualStudioVersion = VisualStudioVersion.VS2017;
            this.frameworkVersion = FrameworkVersion.v4_7;
            this.IsDefaultFrameworkVersion = true;

            this.AddDocumentation = true;
            this.RenameInvalidMembers = true;
            this.WriteLargeNumbersInHex = true;
            this.DecompileDangerousResources = false;

            this.Error = error;
        }

        public string Target { get; private set; }

        public string Out { get; private set; }

        public ILanguage Language { get; set; }

        public VisualStudioVersion VisualStudioVersion { get; set; }

        public FrameworkVersion FrameworkVersion
        {
            get
            {
                return this.frameworkVersion;
            }

            set
            {
                this.frameworkVersion = value;
                this.IsDefaultFrameworkVersion = false;
            }
        }

        public bool IsDefaultFrameworkVersion { get; private set; }

		public bool AddDocumentation { get; set; }

		public bool RenameInvalidMembers { get; set; }

        public bool WriteLargeNumbersInHex { get; set; }

        public bool DecompileDangerousResources { get; set; }

        public IProjectGenerationError Error { get; set; }

        public bool IsHelpRequired
        {
            get
            {
                return this.Error is CommandLineHelpError;
            }
        }
    }
}
