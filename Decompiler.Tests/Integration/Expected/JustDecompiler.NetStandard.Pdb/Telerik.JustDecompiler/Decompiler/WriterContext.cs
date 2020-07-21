using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Decompiler
{
	public class WriterContext
	{
		public AssemblySpecificContext AssemblyContext
		{
			get;
			set;
		}

		public Dictionary<string, Statement> DecompiledStatements
		{
			get;
			private set;
		}

		public Dictionary<string, MethodSpecificContext> MethodContexts
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

		public WriterContext(AssemblySpecificContext assemblyContext, ModuleSpecificContext moduleContext, TypeSpecificContext typeContext, Dictionary<string, MethodSpecificContext> methodContexts, Dictionary<string, Statement> decompiledStatements)
		{
			base();
			this.set_AssemblyContext(assemblyContext);
			this.set_ModuleContext(moduleContext);
			this.set_TypeContext(typeContext);
			this.set_MethodContexts(methodContexts);
			this.set_DecompiledStatements(decompiledStatements);
			return;
		}
	}
}