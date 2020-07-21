using System;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Languages;

namespace Telerik.JustDecompiler.Decompiler
{
	public class DecompilationContext
	{
		public AssemblySpecificContext AssemblyContext
		{
			get;
			set;
		}

		public bool IsStopped
		{
			get;
			private set;
		}

		public ILanguage Language
		{
			get;
			set;
		}

		public MethodSpecificContext MethodContext
		{
			get;
			set;
		}

		public ModuleSpecificContext ModuleContext
		{
			get;
			set;
		}

		public TypeSpecificContext TypeContext
		{
			get;
			set;
		}

		public DecompilationContext(MethodSpecificContext methodContext, TypeSpecificContext typeContext, ILanguage language)
		{
			this(methodContext, typeContext, new ModuleSpecificContext(), new AssemblySpecificContext(), language);
			return;
		}

		public DecompilationContext(MethodSpecificContext methodContext, TypeSpecificContext typeContext, ModuleSpecificContext moduleContext, AssemblySpecificContext assemblyContext, ILanguage language)
		{
			base();
			this.set_MethodContext(methodContext);
			this.set_TypeContext(typeContext);
			this.set_ModuleContext(moduleContext);
			this.set_AssemblyContext(assemblyContext);
			this.set_Language(language);
			this.set_IsStopped(false);
			return;
		}

		public DecompilationContext()
		{
			base();
			return;
		}

		public void StopPipeline()
		{
			this.set_IsStopped(true);
			return;
		}
	}
}