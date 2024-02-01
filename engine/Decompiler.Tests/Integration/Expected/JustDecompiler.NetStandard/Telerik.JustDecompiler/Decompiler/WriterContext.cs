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
			this.AssemblyContext = assemblyContext;
			this.ModuleContext = moduleContext;
			this.TypeContext = typeContext;
			this.MethodContexts = methodContexts;
			this.DecompiledStatements = decompiledStatements;
		}
	}
}