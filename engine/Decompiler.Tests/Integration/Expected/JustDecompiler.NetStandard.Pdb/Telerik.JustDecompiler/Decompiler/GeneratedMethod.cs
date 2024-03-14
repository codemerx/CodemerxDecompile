using Mono.Cecil;
using System;
using System.Runtime.CompilerServices;
using Telerik.JustDecompiler.Ast.Statements;

namespace Telerik.JustDecompiler.Decompiler
{
	public class GeneratedMethod
	{
		public Statement Body
		{
			get;
			private set;
		}

		public MethodSpecificContext Context
		{
			get;
			private set;
		}

		public MethodDefinition Method
		{
			get;
			private set;
		}

		public GeneratedMethod(MethodDefinition method, Statement body, MethodSpecificContext context)
		{
			this.Method = method;
			this.Body = body;
			this.Context = context;
		}
	}
}