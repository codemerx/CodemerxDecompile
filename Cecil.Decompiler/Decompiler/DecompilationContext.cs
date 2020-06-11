using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Decompiler
{
	public class DecompilationContext
	{
		public MethodSpecificContext MethodContext { get; set; }

		public TypeSpecificContext TypeContext { get; set; }

		public ModuleSpecificContext ModuleContext { get; set; }

		public AssemblySpecificContext AssemblyContext { get; set; }

        public ILanguage Language{ get; set; }

        public bool IsStopped { get; private set; }

        public DecompilationContext(MethodSpecificContext methodContext, TypeSpecificContext typeContext, ILanguage language)
			: this(methodContext, typeContext, new ModuleSpecificContext(), new AssemblySpecificContext(), language)
		{
		}

		public DecompilationContext(MethodSpecificContext methodContext, TypeSpecificContext typeContext, ModuleSpecificContext moduleContext, AssemblySpecificContext assemblyContext, ILanguage language)
		{
			this.MethodContext = methodContext;
			this.TypeContext = typeContext;
			this.ModuleContext = moduleContext;
			this.AssemblyContext = assemblyContext;
            this.Language = language;
            this.IsStopped = false;
		}

		public DecompilationContext() { }

        public void StopPipeline()
        {
            this.IsStopped = true;
        }
	}
}